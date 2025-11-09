using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(HealthSystem))]
public class FinalBoss1 : MonoBehaviour
{
    [Header("Referencias")]
    public Transform player;
    public Transform firePoint;
    public Transform swordOrigin;
    public Transform stompOrigin;
    public CameraShake cameraShake;

    [Header("Movimiento")]
    public float moveSpeed = 2f;
    public float chaseRange = 12f;
    public float stopDistance = 2.5f;

    [Header("Ataques")]
    public GameObject bulletPrefab;
    public float bulletSpeed = 8f;

    [Header("Daños y rangos")]
    public float swordRange = 1.6f;
    public int swordDamage = 15;
    public float stompRange = 3f;
    public int stompDamage = 25;
    public float stompForce = 7f; // Empuje al jugador tras pisotón

    [Header("Tiempos")]
    public float attackDelay = 2f;
    public float stompShakeTime = 0.45f;
    public float stompShakeMagnitude = 0.35f;
    public float stompCooldown = 10f;

    [Header("Descanso del jefe")]
    public int phaseDamageThreshold = 200;
    private bool isResting = false;
    public float restDuration = 6f;

    [Header("Layers")]
    public LayerMask playerMask;

    private Rigidbody2D rb;
    private HealthSystem health;
    private Vector3 baseScale;
    private bool isAttacking = false;
    private int attackPhase = 0;
    private float lastStompTime = -999f;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        health = GetComponent<HealthSystem>();
        baseScale = transform.localScale;

        // 🔗 Suscribirse al evento de muerte
        health.OnDeath += OnBossDeath;

        if (player == null)
            player = GameObject.FindGameObjectWithTag("Player")?.transform;

        if (cameraShake == null && Camera.main != null)
            cameraShake = Camera.main.GetComponent<CameraShake>();
    }

    void Update()
    {
        if (player == null || isResting) return;

        FacePlayer();
        ChasePlayer();

        if (!isAttacking)
            StartCoroutine(AttackSequence());
    }

    void FacePlayer()
    {
        int dirSign = (player.position.x >= transform.position.x) ? 1 : -1;
        transform.localScale = new Vector3(baseScale.x * dirSign, baseScale.y, baseScale.z);
    }

    void ChasePlayer()
    {
        float dx = Mathf.Abs(player.position.x - transform.position.x);

        if (dx <= chaseRange)
        {
            if (dx > stopDistance)
                rb.velocity = new Vector2(Mathf.Sign(player.position.x - transform.position.x) * moveSpeed, rb.velocity.y);
            else
                rb.velocity = new Vector2(0, rb.velocity.y);
        }
        else
        {
            rb.velocity = new Vector2(0, rb.velocity.y);
        }
    }

    IEnumerator AttackSequence()
    {
        isAttacking = true;

        switch (attackPhase)
        {
            case 0:
                Shoot();
                break;
            case 1:
                SwordAttack();
                break;
            case 2:
                yield return StartCoroutine(TryStompAttack());
                break;
        }

        attackPhase = (attackPhase + 1) % 3;
        yield return new WaitForSeconds(attackDelay);
        isAttacking = false;
    }

    // 🔫 Disparo autodirigido
    void Shoot()
    {
        if (!bulletPrefab || !firePoint || !player) return;

        Vector2 dir = (player.position - firePoint.position).normalized;
        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        Quaternion rot = Quaternion.Euler(0, 0, angle);

        GameObject b = Instantiate(bulletPrefab, firePoint.position, rot);
        Rigidbody2D rbB = b.GetComponent<Rigidbody2D>();
        if (rbB != null)
        {
            rbB.gravityScale = 0;
            rbB.velocity = dir * bulletSpeed;
        }

        BulletUmbrax bullet = b.GetComponent<BulletUmbrax>();
        if (bullet != null)
        {
            bullet.damage = 10;
            bullet.hitMask = playerMask;
            bullet.SetHomingTarget(player);
        }
    }

    // ⚔️ Ataque cuerpo a cuerpo
    void SwordAttack()
    {
        if (!swordOrigin) return;

        Collider2D hit = Physics2D.OverlapCircle(swordOrigin.position, swordRange, playerMask);
        if (hit)
        {
            HealthSystem hs = hit.GetComponent<HealthSystem>();
            if (hs != null)
            {
                hs.TakeDamage(swordDamage);
                Debug.Log($"⚔️ Espadazo impactó en {hit.name}, daño: {swordDamage}");
            }
        }
    }

    // 💥 Pisotón solo si el jugador está cerca y cooldown completado
    IEnumerator TryStompAttack()
    {
        float dx = Mathf.Abs(player.position.x - transform.position.x);

        if (dx <= stompRange && Time.time >= lastStompTime + stompCooldown)
        {
            lastStompTime = Time.time;
            yield return StartCoroutine(StompAttack());
        }
        else
        {
            yield break;
        }
    }

    IEnumerator StompAttack()
    {
        Debug.Log("💥 FinalBoss realiza pisotón");

        yield return new WaitForSeconds(0.3f);

        if (cameraShake)
            yield return StartCoroutine(cameraShake.Shake(stompShakeTime, stompShakeMagnitude));

        Collider2D hit = Physics2D.OverlapCircle(
            stompOrigin ? stompOrigin.position : transform.position,
            stompRange,
            playerMask
        );

        if (hit)
        {
            HealthSystem hs = hit.GetComponent<HealthSystem>();
            Rigidbody2D playerRb = hit.GetComponent<Rigidbody2D>();

            if (hs != null)
            {
                hs.TakeDamage(stompDamage);
                Debug.Log($"💢 Pisotón golpeó a {hit.name}, daño: {stompDamage}");
            }

            if (playerRb != null)
            {
                Vector2 pushDir = (playerRb.transform.position - transform.position).normalized;
                playerRb.AddForce(pushDir * stompForce, ForceMode2D.Impulse);
            }
        }
    }

    // 💀 Al morir: se detiene, desactiva IA y destruye el objeto tras efecto
    void OnBossDeath()
    {
        Debug.Log("💀 FinalBoss derrotado. Deteniendo IA y esperando destrucción...");

        StopAllCoroutines();
        isAttacking = false;
        rb.velocity = Vector2.zero;
        this.enabled = false;

        StartCoroutine(DestroyAfterEffect());
    }

    IEnumerator DestroyAfterEffect()
    {
        // 🌫️ Espera 2 segundos antes de destruir (puede agregarse animación aquí)
        yield return new WaitForSeconds(2f);

        SceneManager.LoadScene("Raul_FinalGame");
        Destroy(gameObject);
    }

    // 🔵 Dibuja áreas en el editor
    void OnDrawGizmosSelected()
    {
        if (swordOrigin)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(swordOrigin.position, swordRange);
        }

        Gizmos.color = Color.yellow;
        Vector3 p = stompOrigin ? stompOrigin.position : transform.position;
        Gizmos.DrawWireSphere(p, stompRange);
    }
}
