using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement; // ✅ para reiniciar escena

[RequireComponent(typeof(Rigidbody2D), typeof(Collider2D))]
public class PlayerLeandro : MonoBehaviour
{
    [Header("Movimiento")]
    public float moveSpeed = 6f;
    public float jumpForce = 12f;
    public LayerMask groundMask;
    public Transform groundCheck;
    public Vector2 groundCheckSize = new Vector2(0.9f, 0.1f);

    [Header("Portal")]
    public float portalControlLock = 0.25f;

    [Header("Calidad de salto")]
    public float coyoteTime = 0.08f;
    public float jumpBuffer = 0.1f;
    [Range(0.1f, 1f)] public float jumpCutMultiplier = 0.5f;

    [HideInInspector] public int facing = 1; // 1 = derecha, -1 = izquierda

    private Rigidbody2D rb;
    private bool grounded;
    private float lastGroundedTime;
    private float lastJumpPressedTime;

    private bool fromPortal = false;
    private float portalLockTimer = 0f;
    private Vector2 preservedVelocity = Vector2.zero;
    private float defaultGravity;

    private const float minFallVelocity = -250f;
    private const float maxFallVelocity = 350f;

    private float inputX;
    private bool jumpPressed;
    private bool jumpReleased;
    private Collider2D[] groundHits = new Collider2D[2];

    // ==========================================================
    // 🩸 VIDA DEL JUGADOR
    // ==========================================================
    [Header("Vida del jugador")]
    public int maxHealth = 100;
    public int currentHealth;
    public float invincibilityTime = 1.2f; // tiempo invulnerable tras daño
    private bool invincible = false;
    private SpriteRenderer sprite;

    // ==========================================================
    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        defaultGravity = rb.gravityScale;
        rb.interpolation = RigidbodyInterpolation2D.Interpolate;
        rb.collisionDetectionMode = CollisionDetectionMode2D.Continuous;

        sprite = GetComponent<SpriteRenderer>();
        if (sprite == null)
        {
            sprite = gameObject.AddComponent<SpriteRenderer>();
            Debug.LogWarning("⚠️ Se añadió un SpriteRenderer por defecto.");
        }

        currentHealth = maxHealth;
    }

    void Update()
    {
        inputX = Input.GetAxisRaw("Horizontal");
        if (inputX != 0) facing = (int)Mathf.Sign(inputX);

        if (Input.GetKeyDown(KeyCode.Space))
        {
            jumpPressed = true;
            lastJumpPressedTime = jumpBuffer;
        }
        if (Input.GetKeyUp(KeyCode.Space))
            jumpReleased = true;

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

        jumpPressed = false;
        jumpReleased = false;
    }

    // ==========================================================
    // 🦶 DETECTAR SUELO
    // ==========================================================
    private void DetectGround()
    {
        grounded = false;
        if (!groundCheck) return;

        int count = Physics2D.OverlapBoxNonAlloc(groundCheck.position, groundCheckSize, 0f, groundHits, groundMask);
        grounded = count > 0;
    }

    private void HandleMovement()
    {
        if (fromPortal)
        {
            portalLockTimer -= Time.fixedDeltaTime;
            rb.velocity = Vector2.Lerp(rb.velocity, preservedVelocity, Time.fixedDeltaTime * 15f);

            if (portalLockTimer <= 0f)
                fromPortal = false;

            return;
        }

        float targetVx = inputX * moveSpeed;
        float vx = Mathf.Lerp(rb.velocity.x, targetVx, 0.2f);
        rb.velocity = new Vector2(vx, rb.velocity.y);
    }

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

        if (jumpReleased && rb.velocity.y > 0f)
            rb.velocity = new Vector2(rb.velocity.x, rb.velocity.y * jumpCutMultiplier);
    }

    private void ClampVelocity()
    {
        if (rb.velocity.y < minFallVelocity)
            rb.velocity = new Vector2(rb.velocity.x, minFallVelocity);

        if (rb.velocity.sqrMagnitude > maxFallVelocity * maxFallVelocity)
            rb.velocity = rb.velocity.normalized * maxFallVelocity;
    }

    // ==========================================================
    // 🚀 PORTAL
    // ==========================================================
    public void OnPortalExit(Vector2 exitVelocity, float lockTime)
    {
        fromPortal = true;
        portalLockTimer = Mathf.Max(0f, lockTime);
        preservedVelocity = exitVelocity;
        rb.velocity = exitVelocity;

        bool horizontalExit = Mathf.Abs(exitVelocity.x) > Mathf.Abs(exitVelocity.y);

        if (!horizontalExit && exitVelocity.y > 0.5f)
            StartCoroutine(RestoreGravityAfterDelay(0.1f));
        else
            rb.gravityScale = defaultGravity;
    }

    private IEnumerator RestoreGravityAfterDelay(float delay)
    {
        float originalGravity = defaultGravity;
        rb.gravityScale = 0f;
        yield return new WaitForSeconds(delay);
        rb.gravityScale = originalGravity;
    }

    // ==========================================================
    // 💥 DAÑO Y MUERTE
    // ==========================================================
    public void TakeDamage(int amount)
    {
        if (invincible) return;

        currentHealth -= amount;
        if (currentHealth <= 0)
        {
            currentHealth = 0;
            Die();
            return;
        }

        StartCoroutine(DamageFeedback());
        StartCoroutine(TemporaryInvincibility());
    }

    private void Die()
    {
        Debug.Log("💀 Player ha muerto — recargando nivel...");
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    private IEnumerator DamageFeedback()
    {
        Color original = sprite.color;
        sprite.color = Color.red;
        yield return new WaitForSeconds(0.1f);
        sprite.color = original;
    }

    private IEnumerator TemporaryInvincibility()
    {
        invincible = true;
        float elapsed = 0f;
        while (elapsed < invincibilityTime)
        {
            sprite.enabled = !sprite.enabled;
            yield return new WaitForSeconds(0.1f);
            elapsed += 0.1f;
        }
        sprite.enabled = true;
        invincible = false;
    }

    // ==========================================================
    // 💥 DETECTAR DAÑO DE ENEMIGOS
    // ==========================================================
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("EnemyBullet"))
        {
            TakeDamage(10); // daño por bala
            Destroy(collision.gameObject);
        }

        if (collision.CompareTag("Enemy"))
        {
            TakeDamage(20); // daño por colisión directa
        }
    }

    // ==========================================================
    void OnDrawGizmosSelected()
    {
        if (!groundCheck) return;
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireCube(groundCheck.position, groundCheckSize);
    }
}
