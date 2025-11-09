using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeavyBullet : MonoBehaviour
{
    [Header("Propiedades del proyectil pesado")]
    public float damage = 25f;
    public float lifetime = 5f;
    public float explosionRadius = 1.6f;
    public GameObject explosionEffect; // opcional, asigná prefab de explosión
    public LayerMask damageMask; // asigná el layer de Player o destructibles

    private Rigidbody2D rb;
    private bool hasExploded = false;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    private void Start()
    {
        // Si algo falla y no se destruye, se limpia solo
        Destroy(gameObject, lifetime);
    }

    private void Update()
    {
        // 🔄 Rotación según trayectoria (efecto realista)
        if (rb != null && rb.velocity.sqrMagnitude > 0.1f)
        {
            float angle = Mathf.Atan2(rb.velocity.y, rb.velocity.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.Euler(0, 0, angle);
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (hasExploded) return;

        // 💥 Explota al primer impacto
        hasExploded = true;
        Explode();

        // Destruir el proyectil (después de un pequeño retardo)
        Destroy(gameObject, 0.05f);
    }

    private void Explode()
    {
        // 💨 Efecto visual
        if (explosionEffect != null)
        {
            Instantiate(explosionEffect, transform.position, Quaternion.identity);
        }

        // 🔥 Daño por área (explosión)
        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, explosionRadius, damageMask);
        foreach (Collider2D hit in hits)
        {
            if (hit.CompareTag("Player"))
            {
                // Si tu jugador tiene un script de vida, llamalo aquí
                // PlayerHealth ph = hit.GetComponent<PlayerHealth>();
                // if (ph != null)
                //     ph.TakeDamage((int)damage);
            }
        }

        // 💀 Efecto físico adicional (onda expansiva leve)
        foreach (Collider2D col in hits)
        {
            Rigidbody2D body = col.GetComponent<Rigidbody2D>();
            if (body != null)
            {
                Vector2 dir = (col.transform.position - transform.position).normalized;
                body.AddForce(dir * 3f, ForceMode2D.Impulse);
            }
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, explosionRadius);
    }
}
