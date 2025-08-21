using UnityEngine;

[ExecuteAlways]
public class CameraFollow2D : MonoBehaviour
{
    [Header("Target")]
    [SerializeField] private Transform target;      // Asigna tu Player
    [SerializeField] private Rigidbody2D targetRb;  // Opcional: si lo asignas, look-ahead usa velocidad real

    [Header("Offsets")]
    [SerializeField] private Vector3 baseOffset = new Vector3(0f, 2f, -10f);
    [SerializeField] private float horizontalLookAhead = 2f;  // adelanto en X
    [SerializeField] private float lookAheadDamp = 8f;        // suavizado del look-ahead

    [Header("Seguimiento")]
    [SerializeField] private float smoothSpeed = 6f;          // 4–10 suele ir bien
    [SerializeField] private float hardCatchupDistanceX = 3.5f; // si el player se aleja más que esto en X -> snap
    [SerializeField] private float hardCatchupDistanceY = 2.5f; // idem en Y -> snap

    [Header("Límites del mundo (opcional)")]
    [SerializeField] private bool clampToWorld = false;
    [SerializeField] private Rect worldBounds = new Rect(-50, -10, 100, 30); // x,y = esquina inferior izquierda

    private Camera cam;
    private float lastFacing = 1f;     // 1 = derecha, -1 = izquierda
    private float currentLookAheadX = 0f;

    private void Reset()
    {
        cam = GetComponent<Camera>();
        if (cam == null) cam = Camera.main;
        if (target == null)
        {
            var p = GameObject.FindGameObjectWithTag("Player");
            if (p) target = p.transform;
            targetRb = p ? p.GetComponent<Rigidbody2D>() : null;
        }
    }

    private void Awake()
    {
        if (!cam) cam = GetComponent<Camera>();
        if (!cam) cam = Camera.main;
    }

    private void LateUpdate()
    {
        if (target == null) return;

        // 1) Determinar dirección horizontal del player
        float dirX = 0f;
        if (targetRb)
        {
            dirX = Mathf.Sign(targetRb.velocity.x);
            if (Mathf.Abs(targetRb.velocity.x) < 0.01f) dirX = 0f; // parado
        }
        else
        {
            // fallback por escala (si flippeas el sprite con scale.x)
            dirX = Mathf.Sign(Mathf.Abs(target.localScale.x) < 0.0001f ? lastFacing : target.localScale.x);
        }

        if (dirX != 0f) lastFacing = dirX;

        // 2) Suavizar look-ahead
        float targetLookAheadX = horizontalLookAhead * lastFacing;
        currentLookAheadX = Mathf.Lerp(currentLookAheadX, targetLookAheadX, lookAheadDamp * Time.deltaTime);

        // 3) Posición deseada
        Vector3 desired = target.position + baseOffset + new Vector3(currentLookAheadX, 0f, 0f);

        // 4) Catch-up duro si el jugador se “sale”
        Vector3 pos = transform.position;
        float dx = desired.x - pos.x;
        float dy = desired.y - pos.y;

        bool snapX = Mathf.Abs(dx) > hardCatchupDistanceX;
        bool snapY = Mathf.Abs(dy) > hardCatchupDistanceY;

        float t = smoothSpeed * Time.deltaTime;

        float newX = snapX ? desired.x : Mathf.Lerp(pos.x, desired.x, t);
        float newY = snapY ? desired.y : Mathf.Lerp(pos.y, desired.y, t);
        float newZ = baseOffset.z != 0 ? baseOffset.z : (cam ? -10f : pos.z);

        Vector3 newPos = new Vector3(newX, newY, newZ);

        // 5) Clamp a límites del mundo (si se activa)
        if (clampToWorld && cam && cam.orthographic)
        {
            float halfH = cam.orthographicSize;
            float halfW = halfH * cam.aspect;

            float minX = worldBounds.xMin + halfW;
            float maxX = worldBounds.xMax - halfW;
            float minY = worldBounds.yMin + halfH;
            float maxY = worldBounds.yMax - halfH;

            // Si el mundo es más pequeño que la cámara, centra
            if (minX > maxX) { float cx = (worldBounds.xMin + worldBounds.xMax) * 0.5f; minX = maxX = cx; }
            if (minY > maxY) { float cy = (worldBounds.yMin + worldBounds.yMax) * 0.5f; minY = maxY = cy; }

            newPos.x = Mathf.Clamp(newPos.x, minX, maxX);
            newPos.y = Mathf.Clamp(newPos.y, minY, maxY);
        }

        transform.position = newPos;
    }

    private void OnDrawGizmosSelected()
    {
        if (target == null) return;

        // Punto base
        Vector3 basePoint = target.position + baseOffset;
        // Look-ahead derecha/izquierda
        Vector3 rightPoint = basePoint + Vector3.right * Mathf.Abs(horizontalLookAhead);
        Vector3 leftPoint = basePoint - Vector3.right * Mathf.Abs(horizontalLookAhead);

        Gizmos.color = Color.cyan;
        Gizmos.DrawLine(target.position, basePoint);
        Gizmos.DrawWireCube(rightPoint, Vector3.one);
        Gizmos.DrawWireCube(leftPoint, Vector3.one);

        // Dibujar límites del mundo
        if (clampToWorld)
        {
            Gizmos.color = Color.yellow;
            Vector3 bl = new Vector3(worldBounds.xMin, worldBounds.yMin, 0f);
            Vector3 br = new Vector3(worldBounds.xMax, worldBounds.yMin, 0f);
            Vector3 tr = new Vector3(worldBounds.xMax, worldBounds.yMax, 0f);
            Vector3 tl = new Vector3(worldBounds.xMin, worldBounds.yMax, 0f);
            Gizmos.DrawLine(bl, br); Gizmos.DrawLine(br, tr); Gizmos.DrawLine(tr, tl); Gizmos.DrawLine(tl, bl);
        }
    }
}
