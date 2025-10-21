using UnityEngine;

public class BulletUmbrax : MonoBehaviour
{
    public float speed = 6f;
    public float damage = 8f;
    public float lifetime = 4f;
    public LayerMask hitMask;

    Rigidbody2D rb;
    SpriteRenderer sr;
    float timer;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        sr = GetComponent<SpriteRenderer>();
        if (rb != null) rb.gravityScale = 0;
        if (sr != null) sr.color = new Color(0.05f, 0.05f, 0.05f, 1f);
        timer = lifetime;
    }

    void Update()
    {
        rb.velocity = transform.right * speed;
        timer -= Time.deltaTime;
        if (timer <= 0) Destroy(gameObject);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Enemy")) return;

        var hs = other.GetComponent<HealthSystem>();
        if (hs != null) hs.TakeDamage(damage);

        Destroy(gameObject);
    }
}
