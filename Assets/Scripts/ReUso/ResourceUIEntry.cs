using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ResourceUIEntry : MonoBehaviour
{
    [Header("Config")]
    [SerializeField] private ResourceType type = ResourceType.Wood;

    [Tooltip("Formato para el número. Ej: \"x{0}\", \"{0}\" o \"{0} u.\"")]
    [SerializeField] private string numberFormat = "{0}";

    [Header("Refs (asígnalas en el prefab/escena)")]
    [SerializeField] private TMP_Text amountText;   // TextMeshProUGUI donde va el número
    [SerializeField] private Image icon;            // Image para el icono
    [SerializeField] private Sprite iconSprite;     // Sprite opcional (si lo pones aquí, lo aplica a 'icon')

    [Header("Opcional")]
    [Tooltip("Si lo asignas, usará este ResourceManager en vez del Instance/Manager.")]
    [SerializeField] private ResourceManager resourceManagerOverride;

    private ResourceManager rm;

    // ---------- Ciclo ----------
    private void OnEnable()
    {
        rm = ResolveRM();
        if (rm == null)
        {
#if UNITY_EDITOR
            Debug.LogWarning($"[ResourceUIEntry] No se encontró ResourceManager para {name}");
#endif
            return;
        }

        rm.OnResourceChanged += HandleChanged;

        // Aplica el icono si lo diste por inspector
        if (icon && iconSprite)
        {
            icon.sprite = iconSprite;
            icon.enabled = true;
            icon.preserveAspect = true;
        }

        Refresh(); // pinta valor actual al habilitar
    }

    private void OnDisable()
    {
        if (rm != null)
            rm.OnResourceChanged -= HandleChanged;
        rm = null;
    }

    // ---------- Eventos ----------
    private void HandleChanged(ResourceType changed, int newValue)
    {
        if (changed != type || amountText == null) return;
        amountText.text = string.Format(numberFormat, newValue);
    }

    // ---------- Utilidades ----------
    private void Refresh()
    {
        if (rm == null || amountText == null) return;
        amountText.text = string.Format(numberFormat, rm.Get(type));
    }

    private ResourceManager ResolveRM()
    {
        if (resourceManagerOverride != null) return resourceManagerOverride;
        // Si usas el Manager fijo:
        var current = ResourceManager.Current; // (o ResourceManager.Instance si prefieres)
        if (current != null) return current;

        // Último intento: busca por nombre "Manager"
        var go = GameObject.Find("Manager");
        if (go != null) return go.GetComponent<ResourceManager>();

        return null;
    }

    // ---------- API pública (por si quieres configurar por código) ----------
    public void SetType(ResourceType newType)
    {
        type = newType;
        Refresh();
    }

    public void SetIcon(Sprite s)
    {
        iconSprite = s;
        if (icon)
        {
            icon.sprite = s;
            icon.enabled = s != null;
        }
    }

    public void SetFormat(string fmt)
    {
        numberFormat = string.IsNullOrEmpty(fmt) ? "{0}" : fmt;
        Refresh();
    }
}
