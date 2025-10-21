using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoldierAlienWithGun : MonoBehaviour, ITakeDamage
{
    [Header("Stats")]
    public float life = 3f;
    public float moveSpeed = 2f;
    public float stopDistance = 6f;
    public float shootInterval = 1.5f; 
    public float bulletSpeed = 8f;

    [Header("References")]
    public GameObject AlienbulletPrefab;
    public Transform shootPoint;

    private Transform player;
    private Rigidbody2D rb;
    private bool canShoot = true;
    public bool isActive = false;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        player = GameObject.FindGameObjectWithTag("Player")?.transform;

        string scene = UnityEngine.SceneManagement.SceneManager.GetActiveScene().name;
        if (scene == "Raul_SecondWorldLevel4")
        {
            isActive = true;
        }
    }

    void Update()
    {
        if (!isActive || player == null) return;

        float distance = Vector2.Distance(transform.position, player.position);

        if (distance > stopDistance)
        {
            Vector2 direction = (player.position - transform.position).normalized;
            rb.velocity = direction * moveSpeed;
        }
        else
        {
            rb.velocity = Vector2.zero;

            Vector2 direction = (player.position - transform.position).normalized;
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.Euler(0, 0, angle - 90f);

            if (canShoot)
                StartCoroutine(ShootRoutine());
        }
    }

    IEnumerator ShootRoutine()
    {
        canShoot = false;
        Shoot();
        yield return new WaitForSeconds(shootInterval);
        canShoot = true;
    }

    void Shoot()
    {
        if (AlienbulletPrefab == null || shootPoint == null) return;

        GameObject bullet = Instantiate(AlienbulletPrefab, shootPoint.position, shootPoint.rotation);
        Rigidbody2D rbBullet = bullet.GetComponent<Rigidbody2D>();

        if (rbBullet != null && player != null)
        {
            Vector2 dir = (player.position - shootPoint.position).normalized;
            rbBullet.velocity = dir * bulletSpeed;
        }
    }

    public void EnableAttack(bool enable)
    {
        isActive = enable;
    }

    public void TakeDamage(float dmg)
    {
        life -= dmg;
        if (life <= 0)
        {
            Destroy(gameObject);
        }
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, stopDistance);
    }
}
