using UnityEngine;

public class BulletUmbrax : MonoBehaviour
{
    public float speed = 6f;
    public float damage = 8f;
    public float lifetime = 4f;
    public LayerMask hitMask;

    private Rigidbody2D rb;
    private Transform homingTarget;   // 🔹 objetivo autodirigido (solo para el Boss)
    private bool isHoming = false;
    private float rotateSpeed = 200f; // 🔹 velocidad de giro al seguir al jugador

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        if (rb != null) rb.gravityScale = 0;
        Destroy(gameObject, lifetime);
    }

    void Update()
    {
        if (!isHoming)
        {
            // comportamiento normal (para disparos comunes)
            rb.velocity = transform.right * speed;
        }
    }

    void FixedUpdate()
    {
        if (isHoming && homingTarget != null && rb != null)
        {
            // dirección hacia el jugador
            Vector2 direction = ((Vector2)homingTarget.position - rb.position).normalized;
            float rotateAmount = Vector3.Cross(direction, transform.right).z;
            rb.angularVelocity = -rotateAmount * rotateSpeed;
            rb.velocity = transform.right * speed;
        }
    }

    public void SetHomingTarget(Transform target)
    {
        homingTarget = target;
        isHoming = true;
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Enemy")) return;

        var hs = other.GetComponent<HealthSystem>();
        if (hs != null)
            hs.TakeDamage(damage);

        Destroy(gameObject);
    }
}
