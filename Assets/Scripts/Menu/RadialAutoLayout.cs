using UnityEngine;

[ExecuteAlways]
public class RadialAutoLayout : MonoBehaviour
{
    [SerializeField] private float radius = 150f;

    private void OnEnable()
    {
        UpdateLayout();
    }

#if UNITY_EDITOR
    private void OnValidate()
    {
        UpdateLayout();
    }
#endif

    public void UpdateLayout()
    {
        int count = transform.childCount;
        if (count == 0) return;

        float step = 360f / count;

        for (int i = 0; i < count; i++)
        {
            // Índice 0 = ARRIBA, sentido horario
            float angle = step * i;
            float rad = angle * Mathf.Deg2Rad;

            Vector2 pos = new Vector2(
                Mathf.Sin(rad),   // 👈 X
                Mathf.Cos(rad)    // 👈 Y
            ) * radius;

            RectTransform rt = transform.GetChild(i) as RectTransform;
            if (rt != null)
                rt.anchoredPosition = pos;
            else
                transform.GetChild(i).localPosition = pos;
        }
    }
}
