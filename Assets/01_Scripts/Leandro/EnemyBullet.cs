using UnityEngine;

[RequireComponent(typeof(Rigidbody2D), typeof(Collider2D))]
public class StraightBullet : MonoBehaviour
{
    [Header("Propiedades de la bala")]
    public float speed = 12f;             // velocidad constante
    public float lifetime = 4f;           // duración antes de autodestruirse
    public float spinSpeed = 0f;          // rotación visual opcional
    public bool destroyOnImpact = true;   // si se destruye al colisionar
    public LayerMask hitMask;             // capas que puede golpear (Player, Ground, etc.)

    private Rigidbody2D rb;
    private Vector2 direction;
    private bool launched = false;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.gravityScale = 0f;
        rb.drag = 0f;
        rb.angularDrag = 0f;
        rb.collisionDetectionMode = CollisionDetectionMode2D.Continuous;
        rb.interpolation = RigidbodyInterpolation2D.Interpolate;
    }

    private void Start()
    {
        Destroy(gameObject, lifetime);
    }

    private void Update()
    {
        // 🌀 Rotación visual (decorativa, sin afectar trayectoria)
        if (spinSpeed != 0f)
            transform.Rotate(0, 0, spinSpeed * Time.deltaTime);
    }

    // =========================================================
    // 🚀 DISPARO INICIAL (llamado por el boss)
    // =========================================================
    public void Launch(Vector2 velocity)
    {
        if (launched) return; // evita doble lanzamiento
        launched = true;
        rb.velocity = velocity;
        direction = velocity.normalized;

        // Apunta visualmente según dirección
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0, 0, angle);
    }

    // =========================================================
    // 💥 COLISIONES
    // =========================================================
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (((1 << other.gameObject.layer) & hitMask) == 0)
            return; // ignora capas no relevantes

        if (destroyOnImpact)
            Destroy(gameObject);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (((1 << collision.gameObject.layer) & hitMask) == 0)
            return;

        if (destroyOnImpact)
            Destroy(gameObject);
    }

    private void OnBecameInvisible()
    {
        // Limpia balas que se van fuera de cámara
        Destroy(gameObject);
    }
}
