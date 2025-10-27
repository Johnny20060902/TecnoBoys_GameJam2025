using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AlienUmbrax : MonoBehaviour, ITakeDamage
{
    [Header("Stats")]
    public float maxLife = 120f;
    private float life;
    public float moveSpeed = 4f;
    public float dashSpeed = 10f;
    public float dashTime = 0.4f;
    public float dashCooldown = 3f;

    [Header("Attack")]
    public GameObject bulletPrefab;
    public Transform shootPoint;
    public float shootInterval = 2f;
    public int bulletsPerBurst = 4;
    public float burstDelay = 0.15f;
    public float shockwaveRadius = 3f;
    public float shockForce = 6f;

    [Header("References")]
    public SpriteRenderer spriteRenderer;
    private Rigidbody2D rb;
    private Transform playerTransform;

    private bool canShoot = true;
    private bool canDash = true;
    private bool enraged = false;
    public bool isActive = false;

    private Vector2 randomOffset;
    private float offsetTimer;
    private float changeOffsetTime = 0.6f;

    private Player playerScript;
    void Start()
    {
        life = maxLife;
        rb = GetComponent<Rigidbody2D>();

        playerTransform = GameObject.FindGameObjectWithTag("Player")?.transform;
        if (spriteRenderer == null)
            spriteRenderer = GetComponent<SpriteRenderer>();

        if (playerTransform != null)
            playerScript = playerTransform.GetComponent<Player>();
    }

    void Update()
    {
        Debug.Log(life);
        if (!isActive || playerTransform == null) return;

        if (!enraged && life <= maxLife / 2f)
            EnterEnragedMode();

        MoveErratic();

        if (canShoot)
            StartCoroutine(ShootPattern());

        if (canDash && Random.value < (enraged ? 0.04f : 0.015f))
            StartCoroutine(DashTowardsPlayer());


    }

    void MoveErratic()
    {
        offsetTimer += Time.deltaTime;
        if (offsetTimer >= changeOffsetTime)
        {
            offsetTimer = 0f;
            randomOffset = new Vector2(Random.Range(-2f, 2f), Random.Range(-2f, 2f));
        }

        Vector2 direction = (playerTransform.position - transform.position).normalized;
        Vector2 finalDir = (direction + randomOffset).normalized;
        rb.velocity = finalDir * moveSpeed * (enraged ? 1.3f : 1f);
    }

    IEnumerator ShootPattern()
    {
        canShoot = false;

        // 50% chance de disparar una ráfaga dirigida o circular
        if (Random.value > 0.5f)
            yield return StartCoroutine(ShootRandomBurst());
        else
            yield return StartCoroutine(ShootCircularBurst());

        yield return new WaitForSeconds(shootInterval / (enraged ? 1.5f : 1f));
        canShoot = true;
    }

    IEnumerator ShootRandomBurst()
    {
        for (int i = 0; i < bulletsPerBurst; i++)
        {
            ShootAtPlayer();
            yield return new WaitForSeconds(burstDelay / (enraged ? 1.3f : 1f));
        }
    }

    IEnumerator ShootCircularBurst()
    {
        int bulletCount = 12;
        for (int i = 0; i < bulletCount; i++)
        {
            float angle = i * (360f / bulletCount);
            Vector2 dir = new Vector2(Mathf.Cos(angle * Mathf.Deg2Rad), Mathf.Sin(angle * Mathf.Deg2Rad));

            GameObject bullet = Instantiate(bulletPrefab, shootPoint.position, Quaternion.identity);
            bullet.transform.up = dir;
            Rigidbody2D rbBullet = bullet.GetComponent<Rigidbody2D>();
            if (rbBullet != null)
                rbBullet.velocity = dir * 6f * (enraged ? 1.4f : 1f);
        }
        yield return null;
    }

    void ShootAtPlayer()
    {
        if (bulletPrefab == null || shootPoint == null) return;

        Vector2 dir = (playerTransform.position - shootPoint.position).normalized;
        GameObject bullet = Instantiate(bulletPrefab, shootPoint.position, Quaternion.identity);
        bullet.transform.up = dir;
        Rigidbody2D rbBullet = bullet.GetComponent<Rigidbody2D>();
        if (rbBullet != null)
            rbBullet.velocity = dir * 9f * (enraged ? 1.3f : 1f);
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
        ShockwaveAttack();

        yield return new WaitForSeconds(dashCooldown / (enraged ? 1.5f : 1f));
        canDash = true;
    }

    void ShockwaveAttack()
    {
        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, shockwaveRadius);
        foreach (var hit in hits)
        {
            if (hit.CompareTag("Player"))
            {
                Rigidbody2D playerRb = hit.GetComponent<Rigidbody2D>();
                if (playerRb != null)
                {
                    Vector2 pushDir = (hit.transform.position - transform.position).normalized;
                    playerRb.AddForce(pushDir * shockForce, ForceMode2D.Impulse);
                }

                ITakeDamage dmg = hit.GetComponent<ITakeDamage>();
                if (dmg != null) dmg.TakeDamage(2f);
            }
        }
    }

    void EnterEnragedMode()
    {
        enraged = true;
        moveSpeed *= 1.3f;
        shootInterval *= 0.7f;
        dashCooldown *= 0.7f;
        if (spriteRenderer != null)
            spriteRenderer.color = Color.red; 
    }

    public void TakeDamage(float dmg)
    {
        life -= dmg;
        if (life <= 0)
            Destroy(gameObject);

        if (life <= 90)
        {
            playerScript.hasBeamWeapon = true;

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
            Gizmos.DrawWireSphere(shootPoint.position, 0.4f);
        }

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, shockwaveRadius);
    }
}
