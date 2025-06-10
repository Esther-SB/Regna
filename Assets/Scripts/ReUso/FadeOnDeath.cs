using UnityEngine;
using System.Collections;

public class FadeOnDeath : MonoBehaviour
{
    [SerializeField] private float fadeDuration = 0.5f;
    [SerializeField] private bool destroyAfterFade = false;

    private SpriteRenderer spriteRenderer;
    private bool isFading = false;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer == null)
            Debug.LogError("FadeOnDeath requiere un SpriteRenderer.");
    }

    public void TriggerFade()
    {
        if (!isFading && spriteRenderer != null)
        {
            StartCoroutine(FadeOut());
        }
    }

    private IEnumerator FadeOut()
    {
        isFading = true;

        Color originalColor = spriteRenderer.color;
        float elapsed = 0f;

        while (elapsed < fadeDuration)
        {
            float alpha = Mathf.Lerp(1f, 0f, elapsed / fadeDuration);
            spriteRenderer.color = new Color(originalColor.r, originalColor.g, originalColor.b, alpha);

            elapsed += Time.deltaTime;
            yield return null;
        }

        spriteRenderer.color = new Color(originalColor.r, originalColor.g, originalColor.b, 0f);

        if (destroyAfterFade)
        {
            Destroy(gameObject);
        }
    }
}
