using UnityEngine;

public class BulletPhysics : MonoBehaviour
{
    public float damage = 10f;
    public float lifeTime = 3f;
    public GameObject impactEffect;

    void Start()
    {
        Destroy(gameObject, lifeTime);
    }

    void OnCollisionEnter2D(Collision2D col)
    {
        if (col.gameObject.CompareTag("Enemy"))
        {
            // si el enemigo tiene método TakeDamage
            col.gameObject.SendMessage("TakeDamage", (int)damage, SendMessageOptions.DontRequireReceiver);
        }

        if (impactEffect)
            Instantiate(impactEffect, transform.position, Quaternion.identity);

        Destroy(gameObject);
    }
}
