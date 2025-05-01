using UnityEngine;

//He retocado algo...
public class CharacterMovement : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float runSpeed = 10f; // Velocidad al correr
    [SerializeField] private float jumpHeight = 2f;
    [SerializeField] private float jumpTime = 0.5f; // Tiempo para alcanzar la altura m�xima del salto
    [SerializeField] private float gravityScale = 1f; // Escala de gravedad para el personaje
    [SerializeField] private float acceleration = 10f; // Aceleraciónn del movimiento horizontal
    [SerializeField] private float jumpCooldown = 0.2f; // Tiempo entre saltos
    [SerializeField] private BoxCollider2D groundTrigger; // Collider en los pies en modo trigger

    private Rigidbody2D rb;
    private bool isGrounded;
    private bool jumpRequested;
    private float moveDirection;
    private float currentSpeed;
    private float lastJumpTime;

    private bool isRunning;

    // Referencia al componente Animator
    private Animator animator;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();

        // Obtener la referencia al componente Animator
        animator = GetComponent<Animator>();

        // Configurar la escala de gravedad del Rigidbody2D
        rb.gravityScale = gravityScale;
    }

    private void Update()
    {
        // Obtener entrada del jugador
        moveDirection = Input.GetAxis("Horizontal");

        // Verificar si el jugador presiona el bot�n de correr (Shift)
        if (Input.GetKeyDown(KeyCode.LeftShift) || Input.GetKeyDown(KeyCode.RightShift))
        {
            isRunning = !isRunning; // Alternar el estado de correr
        }

        if (Input.GetButton("Jump") && isGrounded && Time.time >= lastJumpTime + jumpCooldown)
        {
            jumpRequested = true;
            lastJumpTime = Time.time;
        }

        // Actualizar par�metros del Animator
        UpdateAnimator();
    }

    private void FixedUpdate()
    {
        // Movimiento horizontal con aceleraci�n
        float targetSpeed = moveDirection * (isRunning ? runSpeed : moveSpeed);
        currentSpeed = Mathf.MoveTowards(currentSpeed, targetSpeed, acceleration * Time.fixedDeltaTime);
        rb.velocity = new Vector2(currentSpeed, rb.velocity.y);

        // Salto
        if (jumpRequested)
        {
            float jumpVelocity = CalculateJumpVelocity(jumpHeight, jumpTime);
            rb.AddForce(new Vector2(0f, jumpVelocity), ForceMode2D.Impulse);
            jumpRequested = false;
        }
    }

    // M�todo para actualizar los par�metros del Animator
    private void UpdateAnimator()
    {
        // Comprobar si el componente Animator est� asignado
        if (animator != null)
        {
            // Establecer el par�metro de velocidad
            animator.SetFloat("Speed", Mathf.Abs(moveDirection));

            // Establecer el par�metro de salto
            animator.SetBool("IsJumping", !isGrounded);
        }
    }

    // M�todo para calcular la velocidad de salto necesaria
    private float CalculateJumpVelocity(float height, float time)
    {
        // La f�rmula de la velocidad inicial de salto es derived de las ecuaciones de movimiento: v = 2 * altura / tiempo
        return (2 * height) / time;
    }

    // Verificar si el personaje est� en el suelo utilizando triggers
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Ground"))
        {
            isGrounded = true;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Ground"))
        {
            isGrounded = false;
        }
    }

    // M�todo para manejar colisiones con la cabeza del personaje
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.CompareTag("Ground") && collision.contacts.Length > 0)
        {
            ContactPoint2D contact = collision.contacts[0];

            // Verificar si el contacto es en la parte superior del personaje
            if (contact.normal.y < 0)
            {
                // Ignorar colisi�n en la parte superior para evitar quedarse atascado
                Physics2D.IgnoreCollision(collision.collider, GetComponent<Collider2D>(), true);
            }
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.collider.CompareTag("Ground"))
        {
            // Rehabilitar colisiones cuando se salga de la colisi�n
            Physics2D.IgnoreCollision(collision.collider, GetComponent<Collider2D>(), false);
        }
    }
}
