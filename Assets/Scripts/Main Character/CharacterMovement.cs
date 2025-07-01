using UnityEngine;
using System.Collections;

public class CharacterMovement : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float runSpeed = 10f;
    [SerializeField] private float jumpHeight = 2f;
    [SerializeField] private float jumpTime = 0.5f;
    [SerializeField] private float gravityScale = 1f;
    [SerializeField] private float acceleration = 10f;
    [SerializeField] private float jumpCooldown = 0.2f;
    [SerializeField] private BoxCollider2D groundTrigger;
    [SerializeField] private Transform visualTransform; // Asigna aquí "Square (5)" en el Inspector
    [SerializeField] private float turnDelay = 0.2f; // Duración simulada de la animación de giro

    private Rigidbody2D rb;
    private bool isGrounded;
    private bool jumpRequested;
    private float moveDirection;
    private float currentSpeed;
    private float lastJumpTime;
    private bool isRunning;
    private bool isFacingRight = true;
    private bool isTurning = false;

    private Animator animator;

    private void Awake()
    {
        rb = GetComponentInChildren<Rigidbody2D>();
        animator = GetComponentInChildren<Animator>();
        rb.gravityScale = gravityScale;
    }

    private void Start()
    {
        if (groundTrigger != null)
        {
            // Añade un componente dinámicamente al objeto GroundCheck que reenvía los triggers a este script
            GroundCheckDispatcher dispatcher = groundTrigger.gameObject.AddComponent<GroundCheckDispatcher>();
            dispatcher.Init(this);
        }
    }

    private void Update()
    {
        float input = Input.GetAxis("Horizontal");

        // Si estamos girando, bloquear cambio de dirección contraria
        if (isTurning)
        {
            moveDirection = 0f;
        }
        else
        {
            // Cambio de dirección detectado
            if (input > 0.01f && !isFacingRight)
            {
                StartCoroutine(Turn(true));
                moveDirection = 0f;
                return;
            }
            else if (input < -0.01f && isFacingRight)
            {
                StartCoroutine(Turn(false));
                moveDirection = 0f;
                return;
            }

            moveDirection = input;
        }

        if (Input.GetKeyDown(KeyCode.LeftShift) || Input.GetKeyDown(KeyCode.RightShift))
        {
            isRunning = !isRunning;
        }

        if (Input.GetButton("Jump") && isGrounded && Time.time >= lastJumpTime + jumpCooldown)
        {
            jumpRequested = true;
            lastJumpTime = Time.time;
        }

        UpdateAnimator();
    }

    private void FixedUpdate()
    {
        float targetSpeed = moveDirection * (isRunning ? runSpeed : moveSpeed);
        currentSpeed = Mathf.MoveTowards(currentSpeed, targetSpeed, acceleration * Time.fixedDeltaTime);
        rb.velocity = new Vector2(currentSpeed, rb.velocity.y);

        if (jumpRequested)
        {
            float jumpVelocity = CalculateJumpVelocity(jumpHeight, jumpTime);
            rb.AddForce(new Vector2(0f, jumpVelocity), ForceMode2D.Impulse);
            jumpRequested = false;
        }
    }

    private void UpdateAnimator()
    {
        if (animator != null)
        {
            //añadir idle
            animator.SetFloat("Speed", Mathf.Abs(moveDirection));
            animator.SetBool("IsJumping", !isGrounded);
        }
    }

    private float CalculateJumpVelocity(float height, float time)
    {
        return (2 * height) / time;
    }

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

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.CompareTag("Ground") && collision.contacts.Length > 0)
        {
            ContactPoint2D contact = collision.contacts[0];
            if (contact.normal.y < 0)
            {
                Physics2D.IgnoreCollision(collision.collider, GetComponentInChildren<Collider2D>(), true);
            }
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.collider.CompareTag("Ground"))
        {
            Physics2D.IgnoreCollision(collision.collider, GetComponentInChildren<Collider2D>(), false);
        }
    }

    private IEnumerator Turn(bool faceRight)
    {
        isTurning = true;

        // Aquí es donde puedes activar la animación de giro
        if (animator != null)
        {
            animator.SetTrigger("Turn"); // Usa este trigger si tienes una animación de giro
        }

        // Esperamos el tiempo de animación de giro
        yield return new WaitForSeconds(turnDelay);

        isFacingRight = faceRight;
        float xScale = faceRight ? 1f : -1f;
        visualTransform.localScale = new Vector3(xScale, visualTransform.localScale.y, visualTransform.localScale.z);

        isTurning = false;
    }

    public void SetGrounded(bool grounded)
    {
        isGrounded = grounded;
    }
}






// Clase auxiliar modular, reutilizable
public class GroundCheckDispatcher : MonoBehaviour
{
    private CharacterMovement characterMovement;

    public void Init(CharacterMovement movement)
    {
        characterMovement = movement;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Ground"))
        {
            characterMovement.SetGrounded(true);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Ground"))
        {
            characterMovement.SetGrounded(false);
        }
    }
}