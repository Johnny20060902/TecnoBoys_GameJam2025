using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(HealthSystem))]
public class BossUmbrax : MonoBehaviour
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
    public float stompForce = 7f; // Fuerza del empuje al jugador

    [Header("Tiempos")]
    public float attackDelay = 2f;
    public float stompShakeTime = 0.45f;
    public float stompShakeMagnitude = 0.35f;
    public float stompCooldown = 10f; // ⏳ Cooldown del pisotón

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
    private float lastStompTime = -999f; // Tiempo del último pisotón

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        health = GetComponent<HealthSystem>();
        baseScale = transform.localScale;

        // Evento de muerte
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

    // 🔥 Disparo autodirigido
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

    // 💢 Pisotón con rango y cooldown
    IEnumerator TryStompAttack()
    {
        float dx = Mathf.Abs(player.position.x - transform.position.x);

        // Solo hace el pisotón si el jugador está muy cerca y el cooldown terminó
        if (dx <= stompRange && Time.time >= lastStompTime + stompCooldown)
        {
            lastStompTime = Time.time;
            yield return StartCoroutine(StompAttack());
        }
        else
        {
            yield break; // No hace nada
        }
    }

    IEnumerator StompAttack()
    {
        Debug.Log("💥 Boss realiza pisotón");

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

            // 🌀 Empuje del pisotón
            if (playerRb != null)
            {
                Vector2 pushDir = (playerRb.transform.position - transform.position).normalized;
                playerRb.AddForce(pushDir * stompForce, ForceMode2D.Impulse);
            }
        }
    }

    void OnBossDeath()
    {
        Debug.Log("💀 Boss derrotado. Cargando siguiente nivel...");

        // ⚠️ Detiene todos los comportamientos del jefe
        StopAllCoroutines();
        isAttacking = false;
        rb.velocity = Vector2.zero;

        // ❌ Desactiva su IA y ataques para que deje de moverse
        this.enabled = false;

        // 🕒 Inicia carga con seguridad
        StartCoroutine(LoadNextLevel());
    }

    IEnumerator LoadNextLevel()
    {
        Debug.Log("⏳ Esperando 2 segundos antes de cargar siguiente nivel...");
        yield return new WaitForSeconds(2f);

        string nextScene = "Raul_SecondWorldLevel1";

        // ✅ Validar que la escena existe en Build Settings
        if (!Application.CanStreamedLevelBeLoaded(nextScene))
        {
            Debug.LogError($"❌ No se encontró la escena '{nextScene}' en Build Settings. Añadila para poder cargarla.");
            yield break;
        }

        Debug.Log($"🚪 Cargando escena: {nextScene}");
        SceneManager.LoadScene(nextScene);
    }

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
