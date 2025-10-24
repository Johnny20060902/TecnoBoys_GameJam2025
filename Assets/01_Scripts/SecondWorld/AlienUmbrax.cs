using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AlienUmbrax : MonoBehaviour, ITakeDamage
{
    [Header("Stats")]
    public float maxLife = 90f;
    private float life;
    public float moveSpeed = 4f;
    public float dashSpeed = 7f;
    public float dashTime = 0.3f;
    public float dashCooldown = 3f;

    [Header("Attack")]
    public GameObject bulletPrefab;
    public Transform shootPoint;
    public float shootInterval = 2f;
    public int bulletsPerBurst = 3;
    public float burstDelay = 0.2f;

    [Header("Player Reference")]
    public Player player;

    private Rigidbody2D rb;
    private Transform playerTransform;
    private bool canShoot = true;
    private bool canDash = true;
    public bool isActive = false;

    private Vector2 randomOffset;
    private float changeOffsetTime = 0.5f;
    private float offsetTimer = 0f;

    void Start()
    {
        life = maxLife;
        rb = GetComponent<Rigidbody2D>();
        playerTransform = GameObject.FindGameObjectWithTag("Player")?.transform;


        if (playerTransform != null)
            player = playerTransform.GetComponent<Player>();

        
    }

    void Update()
    {
        if (!isActive || playerTransform == null) return;

        MoveErratic();

        if (canShoot)
            StartCoroutine(ShootRandomBurst());

        if (canDash && Random.Range(0f, 1f) < 0.01f) 
            StartCoroutine(DashTowardsPlayer());
    }

    void MoveErratic()
    {
        offsetTimer += Time.deltaTime;
        if (offsetTimer >= changeOffsetTime)
        {
            offsetTimer = 0f;
            randomOffset = new Vector2(Random.Range(-1f, 1f), Random.Range(-1f, 1f));
        }

        Vector2 direction = (playerTransform.position - transform.position).normalized;
        rb.velocity = (direction + randomOffset).normalized * moveSpeed;
    }

    IEnumerator ShootRandomBurst()
    {
        canShoot = false;

        int bullets = bulletsPerBurst;
        for (int i = 0; i < bullets; i++)
        {
            ShootRandom();
            yield return new WaitForSeconds(burstDelay);
        }

        yield return new WaitForSeconds(shootInterval);
        canShoot = true;
    }

    void ShootRandom()
    {
        if (bulletPrefab == null || shootPoint == null) return;

        Vector2 target;

        if (Random.value > 0.5f)
        {
            target = (Vector2)playerTransform.position;
        }
        else
        {
            target = (Vector2)transform.position + new Vector2(Random.Range(-3f, 3f), Random.Range(-3f, 3f));
        }

        Vector2 dir = (target - (Vector2)shootPoint.position).normalized;

        GameObject bullet = Instantiate(bulletPrefab, shootPoint.position, Quaternion.identity);
        bullet.transform.up = dir;

        Rigidbody2D rbBullet = bullet.GetComponent<Rigidbody2D>();
        if (rbBullet != null)
        {
            rbBullet.velocity = dir * 8f; 
        }
    }

    IEnumerator DashTowardsPlayer()
    {
        canDash = false;

        Vector2 direction = (playerTransform.position - transform.position).normalized;
        float elapsed = 0f;

        while (elapsed < dashTime)
        {
            rb.velocity = direction * dashSpeed;
            elapsed += Time.deltaTime;
            yield return null;
        }

        rb.velocity = Vector2.zero;
        yield return new WaitForSeconds(dashCooldown);
        canDash = true;
    }

    public void TakeDamage(float dmg)
    {
        life -= dmg;


        if (life <= 0)
        {
            Destroy(gameObject);
        }
    }

    public void EnableAttack(bool enable)
    {
        isActive = enable;
    }
    void OnDrawGizmosSelected()
    {
        if (shootPoint != null)
        {
            Gizmos.color = Color.magenta;
            Gizmos.DrawWireSphere(shootPoint.position, 0.5f);
        }
    }
}
