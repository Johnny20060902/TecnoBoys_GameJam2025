using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class XarkalMissile : MonoBehaviour
{
    public float speed = 5f;
    public float rotateSpeed = 200f;
    public float lifeTime = 5f;
    public GameObject explosionPrefab;
    public float explosionRadius = 2.5f;
    public float damage = 5f;

    private Rigidbody2D rb;
    private Transform target;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        target = GameObject.FindGameObjectWithTag("Player")?.transform;
        Invoke(nameof(Explode), lifeTime);
    }

    void FixedUpdate()
    {
        if (target == null) return;

        Vector2 direction = (Vector2)target.position - rb.position;
        direction.Normalize();

        float rotateAmount = Vector3.Cross(direction, transform.up).z;
        rb.angularVelocity = -rotateAmount * rotateSpeed;
        rb.velocity = transform.up * speed;
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") )
        {
            Explode();
        }
    }

    void Explode()
    {
        if (explosionPrefab != null)
            Instantiate(explosionPrefab, transform.position, Quaternion.identity);

        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, explosionRadius);
        foreach (var hit in hits)
        {
            if (hit.CompareTag("Player"))
            {
                Player playerScript = hit.GetComponent<Player>();
                if (playerScript != null)
                    playerScript.TakeDamage(damage);
            }
        }

        Destroy(gameObject);
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, explosionRadius);
    }
}
