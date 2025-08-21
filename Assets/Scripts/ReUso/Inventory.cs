using UnityEngine;

public class Inventory : MonoBehaviour
{
    public static Inventory Instance { get; private set; }

    [SerializeField] private int totalItems = 0;
    public int TotalItems => totalItems;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public void Add(int amount)
    {
        totalItems += Mathf.Max(0, amount);
         Debug.Log($"Items: {totalItems}");
        // Aquí podrías actualizar UI
    }
}