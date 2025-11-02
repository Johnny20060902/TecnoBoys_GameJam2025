using UnityEngine;
using System.Collections;

public class BossTankController : MonoBehaviour
{
    [Header("Vida")]
    public int maxHealth = 200;
    public int currentHealth;

    [Header("Referencias")]
    public Transform player;
    public Transform firePointMain;
    public Transform firePointLeft;
    public Transform firePointRight;
    public Transform missileLauncher;
    public GameObject bulletPrefab;
    public GameObject heavyBulletPrefab;
    public GameObject missilePrefab;

    [Header("Velocidades y tiempos")]
    public float moveSpeed = 1.5f;
    public float fireDelayNormal = 0.35f;
    public float fireDelayHeavy = 2.5f;
    public float missileDelay = 1.2f;
    public float patrolDistance = 8f;
    public float bulletSpeed = 9f;
    public float heavyForce = 12f;

    private Vector3 startPos;
    private bool facingRight = false;
    private bool isAttacking = false;
    private bool enraged = false;
    private bool introDone = false;

    private int patternIndex = 0;

    private void Start()
    {
        if (!player)
            Debug.LogWarning("⚠️ BossTankController: Asigna el jugador.");

        currentHealth = maxHealth;
        startPos = transform.position;
        StartCoroutine(IntroDelay());
    }

    private IEnumerator IntroDelay()
    {
        yield return new WaitForSeconds(2f);
        introDone = true;
        StartCoroutine(MainLoop());
    }

    private void Update()
    {
        if (!player || !introDone) return;

        FacePlayer();

        if (!isAttacking)
            PatrolMovement();
    }

    // =========================================================
    // 🚜 MOVIMIENTO
    // =========================================================
    private void PatrolMovement()
    {
        float dir = facingRight ? 1f : -1f;
        transform.Translate(Vector2.right * dir * moveSpeed * Time.deltaTime);

        if (Mathf.Abs(transform.position.x - startPos.x) >= patrolDistance)
        {
            facingRight = !facingRight;
            Flip();
        }
    }

    private void Flip()
    {
        Vector3 s = transform.localScale;
        s.x *= -1f;
        transform.localScale = s;
    }

    private void FacePlayer()
    {
        bool shouldFaceRight = player.position.x > transform.position.x;
        if (shouldFaceRight != facingRight)
        {
            facingRight = shouldFaceRight;
            Flip();
        }
    }

    // =========================================================
    // 🔄 CICLO DE ATAQUES
    // =========================================================
    private IEnumerator MainLoop()
    {
        while (currentHealth > 0)
        {
            if (!isAttacking)
            {
                isAttacking = true;

                int nextPattern = Random.Range(0, 4);

                switch (nextPattern)
                {
                    case 0: yield return StartCoroutine(Pattern_TurretBurst()); break;
                    case 1: yield return StartCoroutine(Pattern_HeavyCannonCombo()); break;
                    case 2: yield return StartCoroutine(Pattern_MissileVolley()); break;
                    case 3: yield return StartCoroutine(Pattern_MoveAndFire()); break;
                }

                yield return new WaitForSeconds(Random.Range(1.2f, 1.8f)); 
                isAttacking = false;
            }
            yield return null;
        }
    }

    // =========================================================
    // 🔫 PATRONES DE ATAQUE
    // =========================================================
    private IEnumerator Pattern_TurretBurst()
    {
        Debug.Log("🔫 Patrón: Ametralladora equilibrada");

        int totalBursts = enraged ? 4 : 3;
        int bulletsPerBurst = enraged ? 5 : 3;
        float spread = enraged ? 8f : 6f;

        for (int b = 0; b < totalBursts; b++)
        {
            for (int i = 0; i < bulletsPerBurst; i++)
            {
                if (i % 2 == 0)
                    FireBulletWithSpread(firePointLeft, spread);
                else
                    FireBulletWithSpread(firePointRight, spread);

                yield return new WaitForSeconds(fireDelayNormal * 1.1f);
            }
            yield return new WaitForSeconds(Random.Range(1.0f, 1.3f));
        }
    }

    private IEnumerator Pattern_HeavyCannonCombo()
    {
        Debug.Log("💥 Patrón: Cañón pesado + apoyo táctico");

        int volleys = enraged ? 3 : 2;

        for (int i = 0; i < volleys; i++)
        {
            FireHeavy(firePointMain);
            yield return new WaitForSeconds(0.4f);

            for (int j = 0; j < (enraged ? 2 : 1); j++)
            {
                FireBulletWithSpread(firePointLeft, 5f);
                yield return new WaitForSeconds(0.25f);
                FireBulletWithSpread(firePointRight, 5f);
                yield return new WaitForSeconds(0.3f);
            }

            yield return new WaitForSeconds(Random.Range(1.2f, 1.6f));
        }
    }

    private IEnumerator Pattern_MissileVolley()
    {
        Debug.Log("🚀 Patrón: Misiles guiados balanceados");

        int count = enraged ? 5 : 3;

        for (int i = 0; i < count; i++)
        {
            if (missilePrefab && missileLauncher)
            {
                Quaternion rot = Quaternion.Euler(0, 0, Random.Range(75f, 105f));
                GameObject missile = Instantiate(missilePrefab, missileLauncher.position, rot);

                HomingMissile hm = missile.GetComponent<HomingMissile>();
                if (hm && player) hm.SetTarget(player);

                Destroy(missile, 5.5f);
            }

            yield return new WaitForSeconds(Random.Range(1.0f, 1.4f));
        }
    }

    private IEnumerator Pattern_MoveAndFire()
    {
        Debug.Log("🏃 Patrón: Avance con fuego limitado");

        Vector3 advance = transform.position + new Vector3(facingRight ? 2.5f : -2.5f, 0, 0);
        yield return MoveToPosition(advance, 1.2f);

        int shots = enraged ? 3 : 2;

        for (int i = 0; i < shots; i++)
        {
            FireBulletWithSpread(firePointLeft, 5f);
            FireBulletWithSpread(firePointRight, 5f);
            yield return new WaitForSeconds(0.4f);
        }

        yield return new WaitForSeconds(0.8f);
        FireHeavy(firePointMain);

        Vector3 retreat = startPos;
        yield return MoveToPosition(retreat, 1.1f);
    }

    // =========================================================
    // 🎯 DISPAROS
    // =========================================================
    private void FireBulletWithSpread(Transform firePoint, float spread)
    {
        if (!player || !firePoint || !bulletPrefab) return;

        Vector2 dir = (player.position - firePoint.position).normalized;

        float angleOffset = Random.Range(-spread, spread);
        float baseAngle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        float finalAngle = baseAngle + angleOffset;

        Vector2 spreadDir = new Vector2(Mathf.Cos(finalAngle * Mathf.Deg2Rad), Mathf.Sin(finalAngle * Mathf.Deg2Rad));

        GameObject bullet = Instantiate(bulletPrefab, firePoint.position, Quaternion.Euler(0, 0, finalAngle));
        Rigidbody2D rb = bullet.GetComponent<Rigidbody2D>();

        if (rb)
        {
            rb.gravityScale = 0f;
            rb.velocity = spreadDir * bulletSpeed;
        }

        Destroy(bullet, 3.5f);
    }

    private void FireHeavy(Transform firePoint)
    {
        if (!player || !firePoint || !heavyBulletPrefab) return;

        Vector2 dir = (player.position - firePoint.position).normalized;
        GameObject shell = Instantiate(heavyBulletPrefab, firePoint.position, Quaternion.identity);

        Rigidbody2D rb = shell.GetComponent<Rigidbody2D>();
        if (rb)
        {
            rb.gravityScale = 0.6f;
            rb.AddForce((dir + Vector2.up * 0.25f) * heavyForce, ForceMode2D.Impulse);
        }

        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        shell.transform.rotation = Quaternion.Euler(0, 0, angle);
        Destroy(shell, 6f);
    }

    private IEnumerator MoveToPosition(Vector3 target, float duration)
    {
        Vector3 start = transform.position;
        float t = 0f;
        while (t < duration)
        {
            transform.position = Vector3.Lerp(start, target, t / duration);
            t += Time.deltaTime;
            yield return null;
        }
        transform.position = target;
    }

    // =========================================================
    // 💀 VIDA Y ENRAGED
    // =========================================================
    public void TakeDamage(int dmg)
    {
        currentHealth -= dmg;

        if (!enraged && currentHealth <= maxHealth * 0.5f)
        {
            enraged = true;
            moveSpeed *= 1.3f;
            fireDelayNormal *= 0.8f;
            missileDelay *= 0.8f;
            Debug.Log("🔥 ENRAGED MODE ACTIVADO");
        }

        if (currentHealth <= 0)
        {
            StopAllCoroutines();
            StartCoroutine(DeathSequence());
        }
    }

    private IEnumerator DeathSequence()
    {
        Debug.Log("💥 Tanque destruido");
        yield return new WaitForSeconds(1f);
        Destroy(gameObject);
    }

    // =========================================================
    // 💥 RECIBIR DAÑO DEL JUGADOR
    // =========================================================
    private void OnTriggerEnter2D(Collider2D collision)
    {
        // 🔹 Daño por bala del jugador
        if (collision.CompareTag("PlayerBullet"))
        {
            TakeDamage(10);
            Destroy(collision.gameObject);
        }

       
    }
}
