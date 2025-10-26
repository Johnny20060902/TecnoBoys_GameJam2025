using UnityEngine;

public class BulletElarion : MonoBehaviour
{
    public float speed = 12f;
    public float life = 3f;
    public float damage = 5f;

    void Start()
    {
        Destroy(gameObject, life);
    }

    void Update()
    {
        transform.Translate(Vector2.right * speed * Time.deltaTime);
    }

    void OnTriggerEnter2D(Collider2D col)
    {
        // 💥 Daño a objetos destructibles
        var sp = col.GetComponent<DestructibleSpawner>();
        if (sp != null)
        {
            sp.TakeDamage(damage);
            Destroy(gameObject);
            return;
        }

        // 💥 Daño a enemigos con HealthSystem (nuevo)
        var hs = col.GetComponent<HealthSystem>();
        if (hs != null && !col.CompareTag("Player"))
        {
            hs.TakeDamage(damage);
            Destroy(gameObject);
            return;
        }

        // 💥 Colisión con el suelo
        if (col.gameObject.layer == LayerMask.NameToLayer("Ground"))
        {
            Destroy(gameObject);
        }
    }
}
