using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[RequireComponent(typeof(Collider2D))]
public class DamageDealer : MonoBehaviour
{
    [Header("Damage")]
    [SerializeField] private float damage = 10f;
    [SerializeField] private float activeDuration = 0.5f;

    private Collider2D hitbox;
    private readonly HashSet<Health> damagedTargets = new();

    private Coroutine damageRoutine;
    private ToolData currentTool;

    private void Awake()
    {
        hitbox = GetComponent<Collider2D>();
        hitbox.isTrigger = true;
        hitbox.enabled = false;
    }

    // === Llamado desde PlayerToolUser ===
    public void Activate(ToolData tool)
    {
        currentTool = tool;

        if (damageRoutine != null)
            StopCoroutine(damageRoutine);

        damagedTargets.Clear();
        damageRoutine = StartCoroutine(ActivateDamage());
    }

    private IEnumerator ActivateDamage()
    {
        hitbox.enabled = true;
        yield return new WaitForSeconds(activeDuration);
        hitbox.enabled = false;

        currentTool = null;
        damageRoutine = null;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        // ---------- Recursos ----------
        var resource = other.GetComponent<BreakableResource>();
        if (resource != null)
        {
            if (currentTool != null && resource.CanBeBrokenBy(currentTool))
            {
                // ---------- Entidades con vida ----------
                var healthTarget = other.GetComponent<Health>();
                if (healthTarget != null && !damagedTargets.Contains(healthTarget))
                {
                    Debug.Log($"Daño aplicado a: {healthTarget.name}");
                    healthTarget.TakeDamage(damage);
                    damagedTargets.Add(healthTarget);
                }
            }
            return;
        }

        // ---------- Entidades con vida ----------
        var healthEntity = other.GetComponent<Health>();
        if (healthEntity != null && !damagedTargets.Contains(healthEntity))
        {
            Debug.Log($"Daño aplicado a: {healthEntity.name}");
            healthEntity.TakeDamage(damage);
            damagedTargets.Add(healthEntity);
        }
    }
}
