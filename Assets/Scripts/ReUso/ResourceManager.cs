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
    // --- Singleton simple ---
    public static ResourceManager Instance { get; private set; }

    // --- Eventos ---
    /// <summary>Llamado cuando cambia un recurso concreto: (tipo, nuevoValor)</summary>
    public event Action<ResourceType, int> OnResourceChanged;
    /// <summary>Llamado cuando cambia cualquier recurso</summary>
    public event Action OnAnyChanged;

    // --- Configuración inicial ---
    [Header("Valores iniciales")]
    [Tooltip("Valores al iniciar la partida/escena.")]
    [SerializeField]
    private List<ResourceAmount> startingValues = new List<ResourceAmount>()
    {
        new ResourceAmount{ type = ResourceType.Wood, amount = 0 },
        new ResourceAmount{ type = ResourceType.Stone, amount = 0 },
        new ResourceAmount{ type = ResourceType.Food, amount = 0 },
        new ResourceAmount{ type = ResourceType.Fish, amount = 0 },
    };

    // --- Almacenamiento interno ---
    private readonly Dictionary<ResourceType, int> amounts = new Dictionary<ResourceType, int>();

    // -------------------- Ciclo de vida --------------------
    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        // Si quieres mantenerlo entre escenas, descomenta:
        // DontDestroyOnLoad(gameObject);

        InitStartingValues();
    }

    private void InitStartingValues()
    {
        amounts.Clear();
        foreach (ResourceType t in Enum.GetValues(typeof(ResourceType)))
            amounts[t] = 0;

        foreach (var entry in startingValues)
            Set(entry.type, entry.amount, silent: true);

        // Dispara eventos iniciales para que la UI se sincronice si hace falta
        foreach (var kv in amounts)
            OnResourceChanged?.Invoke(kv.Key, kv.Value);
        OnAnyChanged?.Invoke();
    }

    // -------------------- API pública --------------------

    /// <summary>Devuelve el valor actual (0 si no existe).</summary>
    public int Get(ResourceType type) => amounts.TryGetValue(type, out var v) ? v : 0;

    /// <summary>Asigna un valor concreto (emite eventos). negative -> se recorta a 0.</summary>
    public void Set(ResourceType type, int value, bool silent = false)
    {
        int newVal = Mathf.Max(0, value);
        amounts[type] = newVal;
        if (!silent)
        {
            OnResourceChanged?.Invoke(type, newVal);
            OnAnyChanged?.Invoke();
        }
    }

    /// <summary>Suma una cantidad (solo positivos). Emite eventos.</summary>
    public void Add(ResourceType type, int delta)
    {
        if (delta <= 0) return;
        Set(type, Get(type) + delta);
    }

    /// <summary>Intenta gastar cantidad. Devuelve true si pudo.</summary>
    public bool Spend(ResourceType type, int delta)
    {
        if (delta <= 0) return true;
        int current = Get(type);
        if (current < delta) return false;
        Set(type, current - delta);
        return true;
    }

    /// <summary>¿Se pueden pagar TODOS los costes indicados?</summary>
    public bool CanAfford(IEnumerable<ResourceAmount> costs)
    {
        foreach (var c in costs)
            if (Get(c.type) < c.amount) return false;
        return true;
    }

    /// <summary>Intenta gastar un conjunto de recursos atómicamente.</summary>
    public bool Spend(IEnumerable<ResourceAmount> costs)
    {
        // Comprobación previa
        if (!CanAfford(costs)) return false;

        // Ejecutar gasto
        foreach (var c in costs)
            Set(c.type, Get(c.type) - c.amount);

        return true;
    }

    /// <summary>Resetea todos los recursos a 0 o a los valores iniciales.</summary>
    public void ResetAll(bool toStartingValues = true)
    {
        if (!toStartingValues)
        {
            foreach (ResourceType t in Enum.GetValues(typeof(ResourceType)))
                Set(t, 0, silent: true);
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

    // -------------------- Utilidades de prueba (opcional) --------------------
#if UNITY_EDITOR
    [ContextMenu("DEBUG: +10 Wood")]
    private void Debug_AddWood() => Add(ResourceType.Wood, 10);

    [ContextMenu("DEBUG: Spend 5 Wood")]
    private void Debug_SpendWood() => Spend(ResourceType.Wood, 5);

    [ContextMenu("DEBUG: Reset to starting values")]
    private void Debug_Reset() => ResetAll(true);
#endif
}
