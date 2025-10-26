using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D), typeof(Collider2D))]
public class PlayerLeandro : MonoBehaviour
{
    [Header("Movimiento")]
    [Tooltip("Velocidad horizontal del jugador.")]
    public float moveSpeed = 6f;

    [Tooltip("Fuerza del salto.")]
    public float jumpForce = 12f;

    [Tooltip("Capas que se consideran 'suelo'.")]
    public LayerMask groundMask;

    [Tooltip("Transform que marca el punto para detectar el suelo.")]
    public Transform groundCheck;

    [Tooltip("Tamaño del área de detección del suelo.")]
    public Vector2 groundCheckSize = new Vector2(0.9f, 0.1f);

    [Header("Configuración Portal")]
    [Tooltip("Duración (en segundos) durante la cual el control del jugador se bloquea tras salir de un portal.")]
    public float portalControlLock = 0.25f;

    [Header("Calidad de salto")]
    [Tooltip("Tiempo extra para saltar tras dejar el suelo (coyote time).")]
    public float coyoteTime = 0.08f;

    [Tooltip("Tiempo que se recuerda el input de salto (jump buffer).")]
    public float jumpBuffer = 0.1f;

    [Tooltip("Factor de corte de salto al soltar antes (0.5 = cae más rápido).")]
    [Range(0.1f, 1f)] public float jumpCutMultiplier = 0.5f;

    [HideInInspector] public int facing = 1; // 1 = derecha, -1 = izquierda

    // Componentes
    private Rigidbody2D rb;
    private bool grounded;
    private float lastGroundedTime;
    private float lastJumpPressedTime;

    // Momentum post-portal
    private bool fromPortal = false;
    private float portalLockTimer = 0f;
    private Vector2 preservedVelocity = Vector2.zero;

    // Física general
    private float defaultGravity;
    private const float minFallVelocity = -250f;
    private const float maxFallVelocity = 350f;

    // Input cache
    private float inputX;
    private bool jumpPressed;
    private bool jumpReleased;

    // Reuso para detección de suelo
    private Collider2D[] groundHits = new Collider2D[2];

    // ==========================================================
    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        defaultGravity = rb.gravityScale;
        rb.interpolation = RigidbodyInterpolation2D.Interpolate;
        rb.collisionDetectionMode = CollisionDetectionMode2D.Continuous;
    }

    void Update()
    {
        // 🔹 Leer input en Update
        inputX = Input.GetAxisRaw("Horizontal");
        if (inputX != 0) facing = (int)Mathf.Sign(inputX);

        if (Input.GetKeyDown(KeyCode.Space))
        {
            jumpPressed = true;
            lastJumpPressedTime = jumpBuffer;
        }
        if (Input.GetKeyUp(KeyCode.Space))
            jumpReleased = true;

        // 🔹 Timers de coyote y buffer
        if (grounded) lastGroundedTime = coyoteTime;
        else lastGroundedTime -= Time.deltaTime;

        if (lastJumpPressedTime > 0f) lastJumpPressedTime -= Time.deltaTime;
    }

    void FixedUpdate()
    {
        DetectGround();
        HandleMovement();
        HandleJump();
        ClampVelocity();

        // Reset flags cada frame físico
        jumpPressed = false;
        jumpReleased = false;
    }

    // ==========================================================
    private void DetectGround()
    {
        grounded = false;
        if (!groundCheck) return;

        int count = Physics2D.OverlapBoxNonAlloc(groundCheck.position, groundCheckSize, 0f, groundHits, groundMask);
        grounded = count > 0;
    }

    // ==========================================================
    private void HandleMovement()
    {
        // 🔹 Si acaba de salir de un portal, mantener momentum temporalmente
        if (fromPortal)
        {
            portalLockTimer -= Time.fixedDeltaTime;
            rb.velocity = Vector2.Lerp(rb.velocity, preservedVelocity, Time.fixedDeltaTime * 15f);

            if (portalLockTimer <= 0f)
                fromPortal = false;

            return;
        }

        // 🔹 Movimiento normal
        float targetVx = inputX * moveSpeed;
        float vx = Mathf.Lerp(rb.velocity.x, targetVx, 0.2f);
        rb.velocity = new Vector2(vx, rb.velocity.y);
    }

    // ==========================================================
    private void HandleJump()
    {
        bool canJump = (lastGroundedTime > 0f && lastJumpPressedTime > 0f);

        if (canJump)
        {
            lastGroundedTime = 0f;
            lastJumpPressedTime = 0f;
            rb.velocity = new Vector2(rb.velocity.x, 0f);
            rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
        }

        // 🔹 Corte de salto (permite saltos suaves)
        if (jumpReleased && rb.velocity.y > 0f)
            rb.velocity = new Vector2(rb.velocity.x, rb.velocity.y * jumpCutMultiplier);
    }

    // ==========================================================
    private void ClampVelocity()
    {
        // 🔹 Límite de caída
        if (rb.velocity.y < minFallVelocity)
            rb.velocity = new Vector2(rb.velocity.x, minFallVelocity);

        // 🔹 Límite de velocidad total
        if (rb.velocity.sqrMagnitude > maxFallVelocity * maxFallVelocity)
            rb.velocity = rb.velocity.normalized * maxFallVelocity;
    }

    // ==========================================================
    // 🚀 Evento al salir de un portal
    // ==========================================================
    public void OnPortalExit(Vector2 exitVelocity, float lockTime)
    {
        fromPortal = true;
        portalLockTimer = Mathf.Max(0f, lockTime);
        preservedVelocity = exitVelocity;

        rb.velocity = exitVelocity;

        // 🔹 Ajuste: si el portal apunta horizontalmente, la gravedad no se corta
        bool horizontalExit = Mathf.Abs(exitVelocity.x) > Mathf.Abs(exitVelocity.y);

        if (!horizontalExit && exitVelocity.y > 0.5f)
        {
            StartCoroutine(RestoreGravityAfterDelay(0.1f));
        }
        else
        {
            rb.gravityScale = defaultGravity;
        }
    }

    // ==========================================================
    private IEnumerator RestoreGravityAfterDelay(float delay)
    {
        float originalGravity = defaultGravity;
        rb.gravityScale = 0f;
        yield return new WaitForSeconds(delay);
        rb.gravityScale = originalGravity;
    }

    // ==========================================================
    void OnDrawGizmosSelected()
    {
        if (!groundCheck) return;
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireCube(groundCheck.position, groundCheckSize);
    }
}
