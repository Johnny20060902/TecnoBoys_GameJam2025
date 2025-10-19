using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletBoss : MonoBehaviour
{
    public static float moveSpeed = 5;
    public float timeToDestroy = 5;
    public static float damage = 1f;
    // Start is called before the first frame update
    void Start()
    {
        Destroy(gameObject, timeToDestroy);
    }

    // Update is called once per frame


    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            Destroy(gameObject);
            //Instantiate(explosion, transform.position, transform.rotation);
            //ITakeDamage damageable = collision.gameObject.GetComponent<ITakeDamage>();
            //if (damageable != null)
            //{
            //    damageable.TakeDamage(damage);
            //    Destroy(gameObject);
            //}
        }
    }
}
