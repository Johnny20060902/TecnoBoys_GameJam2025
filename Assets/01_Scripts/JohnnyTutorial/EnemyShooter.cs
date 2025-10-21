using UnityEngine;

[RequireComponent(typeof(HealthSystem))]
public class EnemyShooter : MonoBehaviour
{
    [Header("Ataque a distancia")]
    public Transform firePoint;
    public GameObject bulletUmbraxPrefab;
    public float fireRate = 2f;
    public float bulletDamage = 3f;
    public Color bulletColor = Color.black;

    [Header("Detección del jugador")]
    public float detectRange = 6f; // reducido
    public LayerMask playerMask;

    private float nextFireTime;
    private HealthSystem health;
    private Transform player;

    void Start()
    {
        health = GetComponent<HealthSystem>();
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj) player = playerObj.transform;
    }

    void Update()
    {
        if (!player || health.currentHealth <= 0) return;

        HandleLookAtPlayer();
        HandleAttack();
    }

    void HandleLookAtPlayer()
    {
        if (!player) return;

        Vector2 dir = player.position - transform.position;
        float desired = Mathf.Sign(dir.x);

        Vector3 s = transform.localScale;
        s.x = Mathf.Abs(s.x) * desired;
        transform.localScale = s;
    }

    void HandleAttack()
    {
        float distance = Vector2.Distance(transform.position, player.position);

        if (distance <= detectRange && Time.time > nextFireTime)
        {
            Shoot();
            nextFireTime = Time.time + fireRate;
        }
    }

    void Shoot()
    {
        if (!bulletUmbraxPrefab || !firePoint) return;

        GameObject bullet = Instantiate(bulletUmbraxPrefab, firePoint.position, Quaternion.identity);
        Vector2 dir = (player.position - firePoint.position).normalized;
        bullet.transform.right = dir;

        var b = bullet.GetComponent<BulletElarion>();
        if (b != null) b.damage = bulletDamage;

        var br = bullet.GetComponent<SpriteRenderer>();
        if (br != null) br.color = bulletColor;
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, detectRange);
    }
}
