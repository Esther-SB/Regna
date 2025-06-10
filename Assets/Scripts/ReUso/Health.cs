using UnityEngine;
using UnityEngine.Events;
using System.Collections;

public class Health : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private float maxHealth = 100f;
    [SerializeField] private float deathDelay = 0.3f; // Tiempo antes de destruir el objeto

    [Header("Events")]
    public UnityEvent OnDamaged;
    public UnityEvent OnDeath;

    private float currentHealth;
    public bool IsDead { get; private set; }

    private void Awake()
    {
        currentHealth = maxHealth;
        IsDead = false;
    }

    public void TakeDamage(float amount)
    {
        if (IsDead) return;

        currentHealth -= amount;
        currentHealth = Mathf.Clamp(currentHealth, 0f, maxHealth);

        OnDamaged?.Invoke();

        if (currentHealth <= 0f)
        {
            Die();
        }
    }

    public void Heal(float amount)
    {
        if (IsDead) return;

        currentHealth += amount;
        currentHealth = Mathf.Clamp(currentHealth, 0f, maxHealth);
    }

    public void Die()
    {
        if (IsDead) return;

        IsDead = true;
        OnDeath?.Invoke();

        // Inicia destrucción tras el retardo
        //StartCoroutine(DelayedDestroy());
    }

    private IEnumerator DelayedDestroy()
    {
        yield return new WaitForSeconds(deathDelay);
        Destroy(gameObject);
    }

    public float GetCurrentHealth() => currentHealth;
    public float GetMaxHealth() => maxHealth;
}
