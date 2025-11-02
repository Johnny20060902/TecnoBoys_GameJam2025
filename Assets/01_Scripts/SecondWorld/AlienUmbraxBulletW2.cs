using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AlienUmbraxBulletW2 : MonoBehaviour
{
    public float lifetime = 3f;
    public float damage = 3f;

    void Start()
    {
        Destroy(gameObject, lifetime);
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            ITakeDamage damageable = collision.GetComponent<ITakeDamage>();
            if (damageable != null)
            {
                damageable.TakeDamage(damage);
            }
            Destroy(gameObject);
        }
    }
}
