using System;
using System.Collections.Generic;
using UnityEngine;

public enum ResourceType
{
    Wood,
    Stone,
    Food,
    Fish,
    // Añade más aquí…
}

[Serializable]
public struct ResourceAmount
{
    public ResourceType type;
    public int amount;
}

public class ResourceManager : MonoBehaviour
{
    // ==========================
    //  CONFIG DE ESCENA
    // ==========================
    private const string ExpectedGOName = "Manager"; // Debe vivir en un GO llamado así

    // Referencia principal (no destruimos duplicados; solo avisamos)
    public static ResourceManager Instance { get; private set; }
    [Header("Runtime (solo lectura - depuración)")]
    [SerializeField] private List<ResourceAmount> runtimeSnapshot = new List<ResourceAmount>();

    /// <summary>
    /// Acceso estático seguro. Si no hay Instance, intenta buscar un GO "Manager"
    /// y coger su ResourceManager.
    /// </summary>
    public static ResourceManager Current
    {
        get
        {
            if (Instance != null) return Instance;

            var go = GameObject.Find(ExpectedGOName);
            if (go != null)
            {
                var rm = go.GetComponent<ResourceManager>();
                if (rm != null)
                {
                    Instance = rm;
#if UNITY_EDITOR
                    Debug.Log("[ResourceManager] Referencia enlazada por nombre \"Manager\".");
#endif
                    return Instance;
                }
#if UNITY_EDITOR
                Debug.LogError("[ResourceManager] El GameObject \"Manager\" no tiene ResourceManager.");
#endif
            }
#if UNITY_EDITOR
            else Debug.LogError("[ResourceManager] No se encontró un GameObject llamado \"Manager\" en la escena.");
#endif
            return null;
        }
    }

    // ==========================
    //  EVENTOS
    // ==========================
    public event Action<ResourceType, int> OnResourceChanged;
    public event Action OnAnyChanged;

    // ==========================
    //  INICIALIZACIÓN
    // ==========================
    [Header("Valores iniciales")]
    [SerializeField]
    private List<ResourceAmount> startingValues = new List<ResourceAmount>()
    {
        new ResourceAmount{ type = ResourceType.Wood,  amount = 0 },
        new ResourceAmount{ type = ResourceType.Stone, amount = 0 },
        new ResourceAmount{ type = ResourceType.Food,  amount = 0 },
        new ResourceAmount{ type = ResourceType.Fish,  amount = 0 },
    };

    // Almacenamiento
    private readonly Dictionary<ResourceType, int> amounts = new Dictionary<ResourceType, int>();

    private void Awake()
    {
        // Avisos de montaje
        if (gameObject.name != ExpectedGOName)
        {
#if UNITY_EDITOR
            Debug.LogWarning($"[ResourceManager] Este componente está en \"{gameObject.name}\". Se espera \"{ExpectedGOName}\".");
#endif
        }

        if (Instance != null && Instance != this)
        {
#if UNITY_EDITOR
            Debug.LogWarning("[ResourceManager] Ya hay otra referencia asignada. Mantengo la primera y continúo.");
#endif
        }
        else
        {
            Instance = this;
        }

        InitStartingValues();
    }

    private void InitStartingValues()
    {
        amounts.Clear();

        // Inicializa todas las claves del enum a 0
        foreach (ResourceType t in Enum.GetValues(typeof(ResourceType)))
            amounts[t] = 0;

        // Aplica starting values
        foreach (var entry in startingValues)
            InternalSet(entry.type, entry.amount, silent: true);

        // Notifica valores iniciales
        foreach (var kv in amounts)
            OnResourceChanged?.Invoke(kv.Key, kv.Value);
        OnAnyChanged?.Invoke();
    }

    // ==========================
    //  API PÚBLICA (instancia)
    // ==========================
    public int Get(ResourceType type)
    {
        EnsureKey(type);
        return amounts[type];
    }

    public void Set(ResourceType type, int value, bool silent = false)
    {
        InternalSet(type, value, silent);
    }

    public void Add(ResourceType type, int delta)
    {
        if (delta <= 0)
        {
#if UNITY_EDITOR
            Debug.LogWarning($"[ResourceManager] Add ignorado: delta <= 0 ({delta}) para {type}");
#endif
            return;
        }
        EnsureKey(type);
        InternalSet(type, amounts[type] + delta, silent: false);
#if UNITY_EDITOR
        Debug.Log($"[ResourceManager] +{delta} {type} => {amounts[type]}");
#endif
    }

    public bool Spend(ResourceType type, int delta)
    {
        if (delta <= 0) return true;
        EnsureKey(type);
        if (amounts[type] < delta) return false;
        InternalSet(type, amounts[type] - delta, silent: false);
        return true;
    }

    public bool CanAfford(IEnumerable<ResourceAmount> costs)
    {
        foreach (var c in costs)
        {
            EnsureKey(c.type);
            if (amounts[c.type] < c.amount) return false;
        }
        return true;
    }

    public bool Spend(IEnumerable<ResourceAmount> costs)
    {
        if (!CanAfford(costs)) return false;

        // Ejecuta el gasto de forma silenciosa
        foreach (var c in costs)
        {
            EnsureKey(c.type);
            amounts[c.type] -= c.amount;
        }

        // Notifica cambios de todos los tipos afectados (sencillo: notificamos todos)
        foreach (ResourceType t in Enum.GetValues(typeof(ResourceType)))
            OnResourceChanged?.Invoke(t, amounts[t]);
        OnAnyChanged?.Invoke();
        return true;
    }

    public void ResetAll(bool toStartingValues = true)
    {
        if (!toStartingValues)
        {
            foreach (ResourceType t in Enum.GetValues(typeof(ResourceType)))
                InternalSet(t, 0, silent: true);
        }
        else
        {
            InitStartingValues();
            return;
        }

        foreach (var kv in amounts)
            OnResourceChanged?.Invoke(kv.Key, kv.Value);
        OnAnyChanged?.Invoke();
    }

    // ==========================
    //  WRAPPERS ESTÁTICOS
    // ==========================
    public static int GetStatic(ResourceType type) => Current != null ? Current.Get(type) : 0;
    public static void SetStatic(ResourceType type, int value) => Current?.Set(type, value);
    public static void AddStatic(ResourceType type, int delta) => Current?.Add(type, delta);
    public static bool SpendStatic(ResourceType type, int v) => Current != null && Current.Spend(type, v);

    // ==========================
    //  INTERNOS
    // ==========================
    private void EnsureKey(ResourceType type)
    {
        if (!amounts.ContainsKey(type))
            amounts[type] = 0;
    }

    private void InternalSet(ResourceType type, int value, bool silent)
    {
        EnsureKey(type);
        int newVal = Mathf.Max(0, value);
        amounts[type] = newVal;

        if (!silent)
        {
            OnResourceChanged?.Invoke(type, newVal);
            OnAnyChanged?.Invoke();
        }
    }



#if UNITY_EDITOR
    private void OnValidate()
    {
        // Aviso amistoso si no está en "Manager"
        if (gameObject.name != ExpectedGOName)
            Debug.LogWarning($"[ResourceManager] Este componente debería estar en un GO llamado \"{ExpectedGOName}\".");

        // Aviso por duplicados en startingValues
        var seen = new HashSet<ResourceType>();
        for (int i = 0; i < startingValues.Count; i++)
        {
            if (seen.Contains(startingValues[i].type))
            {
                Debug.LogWarning($"[ResourceManager] Duplicado en Starting Values: {startingValues[i].type}.");
            }
            else seen.Add(startingValues[i].type);
        }
    }
#endif



}
