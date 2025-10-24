using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AlienBulletW2 : MonoBehaviour
{
    public static float moveSpeed = 5;
    public float timeToDestroy = 5;
    public static float damage = 1f;
    //public GameObject explosion;
    void Start()
    {
        Destroy(gameObject, timeToDestroy);
    }

    // Update is called once per frame
    void Update()
    {
        transform.Translate(Vector2.up * moveSpeed * Time.deltaTime);
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Boss"))
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

        if (collision.gameObject.CompareTag("SoldierAlien") || collision.gameObject.CompareTag("SoldierGunAlien") || collision.gameObject.CompareTag("SoldierStrongAlien"))
        {
            Destroy(gameObject);
            //Instantiate(explosion, transform.position, transform.rotation);
            ITakeDamage damageable = collision.gameObject.GetComponent<ITakeDamage>();
            if (damageable != null)
            {
                damageable.TakeDamage(damage);
                Destroy(gameObject);
            }
        }
    }
}
