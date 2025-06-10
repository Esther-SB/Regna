using UnityEngine;
using System.Collections.Generic;
using System.Collections;

public class DamageDealer : MonoBehaviour
{
    [SerializeField] private float damage = 10f;
    [SerializeField] private float activeDuration = 0.5f;


    private Collider2D hitbox;
    private HashSet<Health> damagedTargets = new HashSet<Health>();
    private Coroutine damageRoutine;

    private void Awake()
    {
        hitbox = GetComponent<Collider2D>();
        if (hitbox == null)
        {
            Debug.LogError("DamageDealer requiere un Collider2D.");
        }

        
    }

    public void Activate()
    {
        if (damageRoutine != null)
        {
            StopCoroutine(damageRoutine);
        }

        damagedTargets.Clear();
        damageRoutine = StartCoroutine(ActivateDamage());
    }

    private IEnumerator ActivateDamage()
    {
        hitbox.enabled = false;
        yield return new WaitForSeconds(activeDuration);
        hitbox.enabled = true;
        damageRoutine = null;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        Health target = other.GetComponent<Health>();
        if (target != null)
        {
            Debug.Log($"Daño aplicado a: {target.name}");
            target.TakeDamage(damage);
            damagedTargets.Add(target);


        }
    }
}