using UnityEngine;

public class DestructibleSpawner : MonoBehaviour
{
    public float hp = 10f;
    public GameObject destroyVfx; // opcional
    public EnemySpawner spawner;  // arrástralo (puede ser el mismo GO)

    public void TakeDamage(float d)
    {
        hp -= d;
        GetComponent<DamageFlash>()?.DoFlash();
        if (hp <= 0) DestroySelf();
    }

    void DestroySelf()
    {
        if (spawner != null) spawner.enabled = false;
        if (destroyVfx) Instantiate(destroyVfx, transform.position, Quaternion.identity);
        Destroy(gameObject); // elimina el spawner físicamente
    }
}
