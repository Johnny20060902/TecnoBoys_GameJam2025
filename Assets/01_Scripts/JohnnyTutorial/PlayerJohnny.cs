using UnityEngine;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(Rigidbody2D), typeof(Collider2D))]
public class PlayerJohnny : MonoBehaviour
{
    [Header("Movimiento")]
    public float moveSpeed = 6f;
    public float jumpForce = 16f;
    public LayerMask groundMask;
    public Transform groundCheck;
    public Vector2 groundCheckSize = new Vector2(0.8f, 0.12f);

    [HideInInspector] public int facing = 1;

    Rigidbody2D rb;
    bool grounded;
    bool jumpBuffered;
    PlayerCombatJohnny combat;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.freezeRotation = true;
        combat = GetComponent<PlayerCombatJohnny>();
    }

    void Start()
    {
        int currentScene = SceneManager.GetActiveScene().buildIndex;


        if (PlayerProgress.Instance != null && currentScene > 1)
            PlayerProgress.Instance.SetSwordObtained(true);

        if (PlayerProgress.Instance != null && (PlayerProgress.Instance.hasSword || currentScene > 1))
        {
            if (combat != null)
                combat.UnlockSword();
        }
    }

    void Update()
    {
        HandleMovement();
        HandleJump();
    }

    void HandleMovement()
    {
        float x = Input.GetAxisRaw("Horizontal");
        if (x != 0)
        {
            facing = (int)Mathf.Sign(x);
            Flip();
        }

        rb.velocity = new Vector2(x * moveSpeed, rb.velocity.y);
    }
    void Flip()
    {
        bool lookingRight = transform.localScale.x > 0;

        // Solo cuando realmente cambia la dirección
        if ((facing == 1 && !lookingRight) || (facing == -1 && lookingRight))
        {
            Vector3 s = transform.localScale;
            s.x *= -1;
            transform.localScale = s;

            // Reposicionar/rotar la espada según firePoint
            if (combat != null) combat.ApplyHandPose();
        }
    }


    void HandleJump()
    {
        grounded = Physics2D.OverlapBox(groundCheck.position, groundCheckSize, 0, groundMask);

        if (Input.GetKeyDown(KeyCode.Space))
            jumpBuffered = true;

        if (grounded && jumpBuffered)
        {
            rb.velocity = new Vector2(rb.velocity.x, 0);
            rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
            jumpBuffered = false;
        }

        if (Input.GetKeyUp(KeyCode.Space))
            jumpBuffered = false;
    }

    void OnDrawGizmosSelected()
    {
        if (groundCheck == null) return;
        Gizmos.color = grounded ? Color.green : Color.yellow;
        Gizmos.DrawWireCube(groundCheck.position, groundCheckSize);
    }
}
