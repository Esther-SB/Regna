using System.Collections;
using UnityEngine;

public class FlashOnHit : MonoBehaviour
{
    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private Color flashColor = Color.red;
    [SerializeField] private float flashDuration = 0.1f;

    [SerializeField] Animator animator;
    [SerializeField] float minHitInterval = 0.15f; // anti–spam
    float lastHitTime;
    bool animLock; // para evitar solapamientos

    private Color originalColor;

    private void Awake()
    {
        originalColor = spriteRenderer.color;
    }

    public void Flash()
    {
        if (animLock) return;                        // ya está en anim de golpe
        if (Time.time - lastHitTime < minHitInterval) return; // rebote
        lastHitTime = Time.time;

        animator.ResetTrigger("Toque"); // por si quedó colgado
        animator.SetTrigger("Toque");
        animLock = true;
        StopAllCoroutines();
        StartCoroutine(FlashRoutine());
    }

    private IEnumerator FlashRoutine()
    {
        
        spriteRenderer.color = flashColor;
        yield return new WaitForSeconds(flashDuration);
        spriteRenderer.color = originalColor;
        animLock = false;
    }
}