using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class ResourceUIEntry : MonoBehaviour
{
    [Header("Config")]
    [SerializeField] private ResourceType type;

    [Header("Refs")]
    [SerializeField] private TMP_Text amountText;   // arrastra un TextMeshProUGUI
    [SerializeField] private Image icon;            // opcional (para poner sprite)

    private void OnEnable()
    {
        if (ResourceManager.Instance != null)
        {
            ResourceManager.Instance.OnResourceChanged += HandleChanged;
            // pintar valor actual al habilitar
            amountText.text = ResourceManager.Instance.Get(type).ToString();
        }
    }

    private void OnDisable()
    {
        if (ResourceManager.Instance != null)
            ResourceManager.Instance.OnResourceChanged -= HandleChanged;
    }

    private void HandleChanged(ResourceType changed, int newValue)
    {
        if (changed == type && amountText != null)
            amountText.text = newValue.ToString();
    }
}