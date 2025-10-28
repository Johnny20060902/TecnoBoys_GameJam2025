using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class XarkalSimpleBullet : MonoBehaviour
{
    public float speed = 12f;         
    public float lifeTime = 4f;         
    public float damage = 1f;           
    public GameObject hitEffect;       

    private Rigidbody2D rb;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        Destroy(gameObject, lifeTime);
    }

    // Lanza la bala en una dirección
    public void Shoot(Vector2 direction)
    {
        if (rb != null)
        {
            rb.velocity = direction.normalized * speed;
        }
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            Player player = collision.GetComponent<Player>();
            if (player != null)
                player.TakeDamage(damage);

            Destroy(gameObject);
        }

        if (hitEffect != null)
        {
            Instantiate(hitEffect, transform.position, Quaternion.identity);
        }
    }
}
