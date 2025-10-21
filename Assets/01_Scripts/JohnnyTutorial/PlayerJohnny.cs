using UnityEngine;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(Rigidbody2D), typeof(Collider2D))]
public class PlayerJohnny : MonoBehaviour
{
    [Header("Movimiento")]
    public float moveSpeed = 6f;
    public float jumpForce = 12f;
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

        // 🔹 Si estamos en nivel > 1, marcamos espada obtenida
        if (PlayerProgress.Instance != null && currentScene > 1)
            PlayerProgress.Instance.SetSwordObtained(true);

        // 🔹 Activar espada si ya la tenía o si es nivel > 1
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
        if (x != 0) facing = (int)Mathf.Sign(x);
        rb.velocity = new Vector2(x * moveSpeed, rb.velocity.y);
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
