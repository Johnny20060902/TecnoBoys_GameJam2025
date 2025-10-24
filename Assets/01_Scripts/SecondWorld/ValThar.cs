using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ValThar : MonoBehaviour
{
    [Header("Stats")]
    public float life = 50f;
    public float moveSpeed = 6f;
    public float stopDistance = 5f;
    public float dashSpeed = 7f;
    public float dashTime = 0.6f;
    public float shootInterval = 2f;
    public int bulletsPerBurst = 3;
    public float burstDelay = 0.2f;

    [Header("References")]
    public GameObject bulletPrefab;
    public Transform shootPoint;

    private Transform player;
    private Rigidbody2D rb;
    private bool canShoot = true;
    private bool canDash = true;
    public bool isActive = false;

    void Start()
    {
        string scene = UnityEngine.SceneManagement.SceneManager.GetActiveScene().name;
        if (scene == "Raul_SecondWorldLevel5")
        {
            rb = GetComponent<Rigidbody2D>();
            player = GameObject.FindGameObjectWithTag("Player")?.transform;
        }

    }

    void Update()
    {

        if (!isActive || player == null) return;

        float distance = Vector2.Distance(transform.position, player.position);

        if (distance > stopDistance)
        {
            MoveTowardsPlayer();
        }
        else
        {
            rb.velocity = Vector2.zero;

            // Apuntar al jugador
            Vector2 direction = (player.position - transform.position).normalized;
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.Euler(0, 0, angle - 90f);

            // Ataques
            if (canShoot)
                StartCoroutine(ShootBurst());

            if (canDash && distance < 4f)
                StartCoroutine(DashTowardsPlayer());
        }
    }

    void MoveTowardsPlayer()
    {
        Vector2 direction = (player.position - transform.position).normalized;
        rb.velocity = direction * moveSpeed;
    }

    IEnumerator ShootBurst()
    {
        canShoot = false;

        for (int i = 0; i < bulletsPerBurst; i++)
        {
            Shoot();
            yield return new WaitForSeconds(burstDelay);
        }

        yield return new WaitForSeconds(shootInterval);
        canShoot = true;
    }

    void Shoot()
    {
        if (bulletPrefab == null || shootPoint == null) return;

        GameObject bullet = Instantiate(bulletPrefab, shootPoint.position, shootPoint.rotation);
        Rigidbody2D rbBullet = bullet.GetComponent<Rigidbody2D>();

        if (rbBullet != null && player != null)
        {
            Vector2 dir = (player.position - shootPoint.position).normalized;
            rbBullet.velocity = dir * 8f; // velocidad de bala
        }
    }

    IEnumerator DashTowardsPlayer()
    {
        canDash = false;

        Vector2 direction = (player.position - transform.position).normalized;
        float elapsed = 0f;

        while (elapsed < dashTime)
        {
            rb.velocity = direction * dashSpeed;
            elapsed += Time.deltaTime;
            yield return null;
        }

        rb.velocity = Vector2.zero;

        // Cooldown antes del próximo dash
        yield return new WaitForSeconds(2f);
        canDash = true;
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
            // Aquí puedes agregar animación de muerte o evento de victoria
        }
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, stopDistance);
    }


}
