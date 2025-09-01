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
    [SerializeField] private Transform visualTransform; // Asigna aqu� el transform visual
    [SerializeField] private float turnDelay = 0.2f;

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

    // --- NUEVO: sistema de bloqueo de movimiento (apilable) ---
    private int movementBlockCount = 0;
    public bool IsMovementBlocked => movementBlockCount > 0;

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
            GroundCheckDispatcher dispatcher = groundTrigger.gameObject.AddComponent<GroundCheckDispatcher>();
            dispatcher.Init(this);
        }
    }

    private void Update()
    {
        float input = Input.GetAxis("Horizontal");

        if (IsMovementBlocked)
        {
            // Ignora input y deja al personaje quieto en X mientras est� bloqueado
            moveDirection = 0f;
            jumpRequested = false;
            UpdateAnimator();
            return;
        }

        // Si estamos girando, bloquear cambio de direcci�n contraria
        if (isTurning)
        {
            moveDirection = 0f;
        }
        else
        {
            // Cambio de direcci�n detectado
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

        // Si est� bloqueado, forzamos targetSpeed a 0
        if (IsMovementBlocked) targetSpeed = 0f;

        currentSpeed = Mathf.MoveTowards(currentSpeed, targetSpeed, acceleration * Time.fixedDeltaTime);
        rb.velocity = new Vector2(currentSpeed, rb.velocity.y);

        if (jumpRequested && !IsMovementBlocked)
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
            animator.SetFloat("Speed", Mathf.Abs(currentSpeed));
            animator.SetBool("IsJumping", !isGrounded);
            Debug.Log(moveDirection);
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

        if (animator != null)
        {
            animator.SetTrigger("Turn");
        }

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

    // -------- NUEVO: API para bloquear/desbloquear --------
    public void PushMovementBlock()
    {
        movementBlockCount++;
        // Frena inmediatamente el desplazamiento horizontal
        moveDirection = 0f;
        currentSpeed = 0f;
        if (rb != null) rb.velocity = new Vector2(0f, rb.velocity.y);
    }

    public void PopMovementBlock()
    {
        movementBlockCount = Mathf.Max(0, movementBlockCount - 1);
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
