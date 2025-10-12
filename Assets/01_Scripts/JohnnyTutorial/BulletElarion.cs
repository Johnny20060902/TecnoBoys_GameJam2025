using UnityEngine;

public class BulletElarion : MonoBehaviour
{
    public float speed = 12f;
    public float life = 3f;
    public float damage = 1f;

    void Start() { Destroy(gameObject, life); }

    void Update() { transform.Translate(Vector2.right * speed * Time.deltaTime); }

    void OnTriggerEnter2D(Collider2D col)
    {
        var sp = col.GetComponent<DestructibleSpawner>();
        if (sp != null) { sp.TakeDamage(damage); Destroy(gameObject); return; }

        var e = col.GetComponent<Enemy>();
        if (e != null) { e.TakeDamage(damage); Destroy(gameObject); return; }

        if (col.gameObject.layer == LayerMask.NameToLayer("Ground"))
            Destroy(gameObject);
    }

}
