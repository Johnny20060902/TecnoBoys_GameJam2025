using UnityEngine;

[RequireComponent(typeof(Rigidbody2D), typeof(Collider2D))]
public class PlayerJohnny : MonoBehaviour
{
    [Header("Movimiento")]
    public float moveSpeed = 6f;
    public float jumpForce = 12f;
    public LayerMask groundMask;
    public Transform groundCheck;
    public Vector2 groundCheckSize = new Vector2(0.9f, 0.1f);

    [HideInInspector] public int facing = 1; // 1 derecha, -1 izquierda

    Rigidbody2D rb;
    bool grounded;

    void Awake() => rb = GetComponent<Rigidbody2D>();

    void Update()
    {
        // mover A/D o flechas
        float x = Input.GetAxisRaw("Horizontal");
        if (x != 0) facing = (int)Mathf.Sign(x);

        rb.velocity = new Vector2(x * moveSpeed, rb.velocity.y);

        // chequear suelo
        grounded = Physics2D.OverlapBox(groundCheck.position, groundCheckSize, 0, groundMask);

        // salto
        if (Input.GetKeyDown(KeyCode.Space) && grounded)
        {
            rb.velocity = new Vector2(rb.velocity.x, 0);
            rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
        }
    }

    void OnDrawGizmosSelected()
    {
        if (groundCheck == null) return;
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireCube(groundCheck.position, groundCheckSize);
    }
}
