using UnityEngine;

public class PlayerChop : MonoBehaviour
{
    [Header("Refs")]
    [SerializeField] private Animator animator;

    [Header("Ataque")]
    [SerializeField] private string triggerName = "Talar"; // nombre del Trigger en el Animator
    [SerializeField] private KeyCode key = KeyCode.E;
    [SerializeField] private DamageDealer axeHitbox; // arrastra tu hitbox con DamageDealer
    [SerializeField] private CharacterMovement movement; // referencia al script de movimiento

    private bool isChopping;

    private void Reset()
    {
        if (!animator) animator = GetComponentInChildren<Animator>();
    }

    private void Update()
    {
        if (isChopping) return;

        if (Input.GetKeyDown(key))
        {
            animator.ResetTrigger(triggerName); // por si quedó colgado
            animator.SetTrigger(triggerName);
            isChopping = true;
            movement?.PushMovementBlock();
        }
    }

    // === Llamado por un Animation Event al FINAL del clip "Talar" ===
    public void AE_TalarEnd()
    {
        // Aplica daño aquí (al terminar la animación):
        ApplyChopDamage();
        movement?.PopMovementBlock();
        // desbloquear para poder volver a atacar
        isChopping = false;
    }

    private void ApplyChopDamage()
    {
        if (axeHitbox != null)
            axeHitbox.Activate();
        Debug.Log("Daño aplicado tras terminar animación Talar.");
    }
}