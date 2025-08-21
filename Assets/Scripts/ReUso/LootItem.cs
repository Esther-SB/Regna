using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class LootItem : MonoBehaviour
{
    public enum LootState { Flying, Landed, Magnetizing }

    [Header("Recurso que entrega")]
    [SerializeField] private ResourceType lootType = ResourceType.Wood;
    [SerializeField] private int amount = 1;

    [Header("Target del jugador")]
    [Tooltip("Si lo asignas (p.ej. un hijo 'LootAnchor' del player), el �tem seguir� ESTE transform.")]
    [SerializeField] private Transform explicitTarget;
    [SerializeField] private string playerTag = "Player";
    [SerializeField] private float reFindInterval = 0.5f; // reintenta encontrar al player/anchor

    [Header("Suelo")]
    [SerializeField] private LayerMask groundMask;
    [SerializeField] private float groundRayDistance = 0.25f;
    [SerializeField] private Vector2 groundRayOffset = new Vector2(0f, 0.05f);

    [Header("Recogida / Im�n")]
    [SerializeField] private float pickupDelayAfterLanding = 0.2f;
    [SerializeField] private float magnetSpeed = 10f;          // velocidad de atracci�n
    [SerializeField] private float magnetResponsiveness = 12f; // lerp de la velocidad hacia el target
    [SerializeField] private float collectDistance = 0.25f;

    [Header("Vida �til")]
    [SerializeField] private float lifeTime = 30f;

    private Rigidbody2D rb;
    private Transform player;     // ra�z del player
    private Rigidbody2D playerRb; // para centro de masa
    private Collider2D playerCol; // para bounds.center
    private float spawnTime;
    private float reFindTimer;
    private LootState state = LootState.Flying;
    private bool canPickup = false;

    // Impulso inicial al instanciar (ll�malo desde el spawner)
    public void Init(Vector2 impulse)
    {
        if (!rb) rb = GetComponent<Rigidbody2D>();
        rb.velocity = Vector2.zero;
        rb.gravityScale = Mathf.Max(1f, rb.gravityScale);
        rb.AddForce(impulse, ForceMode2D.Impulse);
        state = LootState.Flying;
        canPickup = false;
    }

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        spawnTime = Time.time;
        ResolvePlayerRefs(); // primer intento
    }

    private void Update()
    {
        // Despawn
        if (Time.time - spawnTime > lifeTime) { Destroy(gameObject); return; }

        // Re-resolver referencias por si el player ha cambiado/respawneado
        reFindTimer += Time.deltaTime;
        if (reFindTimer >= reFindInterval || player == null || !player.gameObject.activeInHierarchy)
        {
            reFindTimer = 0f;
            ResolvePlayerRefs();
        }

        switch (state)
        {
            case LootState.Flying:
                if (IsGrounded() && Mathf.Abs(rb.velocity.y) < 0.05f)
                    OnFirstLanding();
                break;

            case LootState.Landed:
                if (canPickup)
                {
                    state = LootState.Magnetizing;
                    rb.gravityScale = 0f;      // quita gravedad
                    rb.velocity = Vector2.zero; // quita inercia acumulada
                }
                break;

            case LootState.Magnetizing:
                FollowTargetLive();
                break;
        }
    }

    private void FollowTargetLive()
    {
        // Recalcula la posici�n ACTUAL del target cada frame
        Vector2 targetPos = GetCurrentTargetPosition();
        Vector2 pos = rb.position;

        // �ya podemos recoger?
        if ((targetPos - pos).sqrMagnitude <= collectDistance * collectDistance)
        {
            Collect();
            return;
        }

        // Dir�gete al target con velocidad controlada y respuesta suave
        Vector2 desiredVel = (targetPos - pos).normalized * magnetSpeed;
        rb.velocity = Vector2.Lerp(rb.velocity, desiredVel, magnetResponsiveness * Time.deltaTime);
    }

    private Vector2 GetCurrentTargetPosition()
    {
        // Prioridad: explicitTarget > centro de masa RB > centro collider > transform
        if (explicitTarget != null) return explicitTarget.position;
        if (playerRb != null) return playerRb.worldCenterOfMass;
        if (playerCol != null) return playerCol.bounds.center;
        if (player != null) return player.position;
        return rb.position; // fallback
    }

    private bool IsGrounded()
    {
        Vector2 origin = (Vector2)transform.position + groundRayOffset;
        return Physics2D.Raycast(origin, Vector2.down, groundRayDistance, groundMask).collider != null;
    }

    private void OnFirstLanding()
    {
        state = LootState.Landed;
        Invoke(nameof(EnablePickup), pickupDelayAfterLanding);
    }

    private void EnablePickup() => canPickup = true;

    private void ResolvePlayerRefs()
    {
        // Si ya tienes un anchor expl�cito arrastrado, usa su ra�z como player
        if (explicitTarget != null) player = explicitTarget.root;

        if (player == null)
        {
            GameObject p = GameObject.FindGameObjectWithTag(playerTag);
            player = p ? p.transform : null;

            // Si encontramos un hijo llamado "LootAnchor" lo usamos autom�ticamente como explicitTarget
            if (player != null && explicitTarget == null)
            {
                Transform anchor = player.Find("LootAnchor");
                if (anchor != null) explicitTarget = anchor;
            }
        }

        playerRb = player ? player.GetComponentInChildren<Rigidbody2D>() : null;
        playerCol = player ? player.GetComponentInChildren<Collider2D>() : null;
    }

    private void Collect()
    {
        // SUMA DIRECTAMENTE AL RESOURCE MANAGER -> el HUD se actualizar� por evento
        if (ResourceManager.Instance != null)
            ResourceManager.Instance.Add(lootType, amount);

        // Aqu� puedes reproducir sonido/part�culas si quieres
        Destroy(gameObject);
    }

    // --- Utilidades p�blicas opcionales ---
    public void SetLoot(ResourceType type, int amt)
    {
        lootType = type;
        amount = amt;
    }
}
