using UnityEngine;

public class LootSpawner : MonoBehaviour
{
    [Header("Loot")]
    [SerializeField] private GameObject lootPrefab; // Prefab del ítem
    [SerializeField] private int minCount = 3;
    [SerializeField] private int maxCount = 6;

    [Header("Física del lanzamiento")]
    [SerializeField] private float minForce = 2f;
    [SerializeField] private float maxForce = 5f;
    [SerializeField] private float spreadAngle = 80f; // grados del abanico

    [Header("Offsets")]
    [SerializeField] private Vector2 spawnOffset = Vector2.zero;

    private bool spawned = false; // para no duplicar spawns

    /// Llama a esto cuando el objeto “muere”.
    public void SpawnLoot()
    {
        if (spawned || lootPrefab == null) return;
        spawned = true;

        int count = Random.Range(minCount, maxCount + 1);
        for (int i = 0; i < count; i++)
        {
            Vector3 pos = transform.position + (Vector3)spawnOffset;
            GameObject g = Instantiate(lootPrefab, pos, Quaternion.identity);

            // Fuerza aleatoria en un abanico
            float ang = Random.Range(-spreadAngle * 0.5f, spreadAngle * 0.5f);
            float force = Random.Range(minForce, maxForce);
            Vector2 dir = Quaternion.Euler(0, 0, ang) * Vector2.up; // hacia arriba con abanico
            Vector2 impulse = dir * force;

            // Inicializar el ítem con esa fuerza
            var loot = g.GetComponent<LootItem>();
            if (loot) loot.Init(impulse);
            else
            {
                // Fallback si no tiene LootItem (aplica impulso directo)
                var rb = g.GetComponent<Rigidbody2D>();
                if (rb) rb.AddForce(impulse, ForceMode2D.Impulse);
            }
        }
    }

    /// Útil si quieres que el propio spawner se destruya después de soltar loot.
    public void SpawnAndDestroy()
    {
        SpawnLoot();
        Destroy(gameObject);
    }
}
