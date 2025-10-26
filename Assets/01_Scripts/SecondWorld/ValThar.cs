using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ValThar : MonoBehaviour, ITakeDamage
{
    [Header("Stats")]
    public float life = 50f;
    public float moveSpeed = 5.5f;
    public float stopDistance = 5f;
    public float dashSpeed = 7f;
    public float dashTime = 0.6f;
    public float shootInterval = 2f;
    public int bulletsPerBurst = 3;
    public float burstDelay = 0.2f;

    [Header("Fan Shot Settings")]
    public int fanBullets = 5;         
    public float fanSpreadAngle = 45f; 

    [Header("References")]
    public GameObject bulletPrefab;
    public Transform shootPoint;

    private Transform player;
    private Rigidbody2D rb;
    private bool canShoot = true;
    private bool canDash = true;
    public bool isActive = false;

    public GameObject DialogueFinish;
    public GameObject alienPower;

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
            MoveTowardsPlayerWithZigZag();
        }
        else
        {
            rb.velocity = Vector2.zero;
            Vector2 direction = (player.position - transform.position).normalized;
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.Euler(0, 0, angle - 90f);

            if (canShoot)
                StartCoroutine(ShootFanBurst());

            if (canDash && distance < 4f)
                StartCoroutine(DashTowardsPlayer());
        }
    }

    void MoveTowardsPlayerWithZigZag()
    {
        Vector2 direction = (player.position - transform.position).normalized;

        Vector2 zigZag = new Vector2(Mathf.Sin(Time.time * 5f), 0);

        rb.velocity = (direction + zigZag).normalized * moveSpeed;
    }

    IEnumerator ShootFanBurst()
    {
        canShoot = false;

        for (int i = 0; i < bulletsPerBurst; i++)
        {
            ShootFan(fanBullets, fanSpreadAngle);
            yield return new WaitForSeconds(burstDelay);
        }

        yield return new WaitForSeconds(shootInterval);
        canShoot = true;
    }

    void ShootFan(int bullets, float spreadAngle)
    {
        if (bulletPrefab == null || shootPoint == null) return;

        Vector2 dir = (player.position - shootPoint.position).normalized;
        float baseAngle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;

        for (int i = 0; i < bullets; i++)
        {
            float angle = baseAngle - spreadAngle / 2 + (spreadAngle / (bullets - 1)) * i;
            Quaternion rot = Quaternion.Euler(0, 0, angle - 90f);
            GameObject bullet = Instantiate(bulletPrefab, shootPoint.position, rot);
            Rigidbody2D rbBullet = bullet.GetComponent<Rigidbody2D>();
            if (rbBullet != null)
                rbBullet.velocity = new Vector2(Mathf.Cos(angle * Mathf.Deg2Rad), Mathf.Sin(angle * Mathf.Deg2Rad)) * 8f;
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
            Instantiate(DialogueFinish, transform.position, Quaternion.identity);
            alienPower.SetActive(true);
            alienPower.transform.position = new Vector2(transform.position.x, transform.position.y);
            Destroy(gameObject);
        }
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, stopDistance);
    }

}
