using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class XarKalFinalW2 : MonoBehaviour, ITakeDamage
{
    [Header("Configuración General")]
    public float life = 250f;
    public float moveSpeed = 6f;
    public Transform player;
    public float followRange = 15f;
    public float floatHeight = 9f;
    public float floatAmplitude = 0.5f;
    public float floatSpeed = 2f;

    [Header("Ataques")]
    public GameObject missilePrefab;   
    public GameObject bulletPrefab;    
    public GameObject explosionPrefab;
    public GameObject damageParticlesPrefab;
    public Transform firePoints;
    public Transform[] bulletFirePoints;  
    public Transform[] missileFirePoints;
    public float timeBetweenAttacks = 2f;

    [Header("Láser")]
    public LineRenderer laserRenderer;
    public float laserDuration = 1.5f;
    public float laserDamagePerSecond = 8f;
    public float laserRange = 20f;

    private bool canAttack = true;
    private bool isDead = false;
    private float floatTimer = 0f;
    public GameObject DialogFinal;

    [Header("Estado")]
    public bool isActive = false;

    private enum AttackPattern { LaserBurst, MissileRain, RapidFire }
    private AttackPattern currentPattern;

    void Start()
    {
        if (player == null)
            player = GameObject.FindGameObjectWithTag("Player").transform;

        if (laserRenderer != null)
            laserRenderer.enabled = false;

        StartCoroutine(ChangePattern());
    }

    void Update()
    {
        if (!isActive || isDead) return;

        FollowPlayer();
        FloatMotion();

        if (canAttack)
            StartCoroutine(PerformAttack());
    }

    void FollowPlayer()
    {
        if (player == null) return;

        float distance = Vector2.Distance(transform.position, player.position);
        if (distance < followRange)
        {
            Vector3 targetPos = new Vector3(player.position.x, transform.position.y, transform.position.z);
            transform.position = Vector3.MoveTowards(transform.position, targetPos, moveSpeed * Time.deltaTime);
        }
    }

    void FloatMotion()
    {
        floatTimer += Time.deltaTime * floatSpeed;
        float yOffset = Mathf.Sin(floatTimer) * floatAmplitude;
        transform.position = new Vector3(transform.position.x, floatHeight + yOffset, transform.position.z);
    }

    IEnumerator PerformAttack()
    {
        canAttack = false;

        switch (currentPattern)
        {
            case AttackPattern.LaserBurst:
                yield return StartCoroutine(LaserBurst());
                break;
            case AttackPattern.MissileRain:
                yield return StartCoroutine(MissileRain());
                break;
            case AttackPattern.RapidFire:
                yield return StartCoroutine(RapidFire());
                break;
        }

        yield return new WaitForSeconds(timeBetweenAttacks);
        canAttack = true;
    }

    IEnumerator LaserBurst()
    {
        if (laserRenderer == null) yield break;

        Transform firePoint = firePoints;
        Vector3 originalX = transform.position;

        Vector2 start = firePoint.position;
        Vector2 dir = (player.position - (Vector3)start).normalized;

        float warningTime = 1.0f;

        laserRenderer.enabled = true;
        laserRenderer.startColor = new Color(1, 0, 0, 0.5f);
        laserRenderer.endColor = new Color(1, 0, 0, 0.5f);

        float timer = 0f;
        while (timer < warningTime)
        {
            dir = (player.position - (Vector3)firePoint.position).normalized;

            Vector2 end = start + dir * laserRange;
            laserRenderer.SetPosition(0, start);
            laserRenderer.SetPosition(1, end);

            transform.position = new Vector3(originalX.x, transform.position.y, transform.position.z);

            timer += Time.deltaTime;
            yield return null;
        }

        Vector2 fixedDir = dir;

        laserRenderer.startColor = Color.red;
        laserRenderer.endColor = Color.red;

        float elapsed = 0f;
        while (elapsed < laserDuration)
        {
            Vector2 end = start + fixedDir * laserRange;

            RaycastHit2D hit = Physics2D.Raycast(start, fixedDir, laserRange, LayerMask.GetMask("Player"));
            if (hit.collider != null)
            {
                end = hit.point;
                ITakeDamage dmg = hit.collider.GetComponent<ITakeDamage>();
                if (dmg != null)
                    dmg.TakeDamage(laserDamagePerSecond * Time.deltaTime);
            }

            laserRenderer.SetPosition(0, start);
            laserRenderer.SetPosition(1, end);

            transform.position = new Vector3(originalX.x, transform.position.y, transform.position.z);

            elapsed += Time.deltaTime;
            yield return null;
        }

        laserRenderer.enabled = false;
    }






    IEnumerator MissileRain()
    {
        int missileCount = 6;
        if (missileFirePoints.Length == 0 || missilePrefab == null) yield break;

        for (int i = 0; i < missileCount; i++)
        {
            Transform fp = missileFirePoints[Random.Range(0, missileFirePoints.Length)];
            Vector3 spawnPos = fp.position + Vector3.down * 0.5f;
            Instantiate(missilePrefab, spawnPos, Quaternion.identity);

            yield return new WaitForSeconds(0.5f);
        }
    }

    IEnumerator RapidFire()
    {
        int bulletCount = 12;
        for (int i = 0; i < bulletCount; i++)
        {
            if (bulletFirePoints.Length == 0 || bulletPrefab == null) yield break;

            Transform fp = bulletFirePoints[Random.Range(0, bulletFirePoints.Length)];

            GameObject bullet = Instantiate(bulletPrefab, fp.position + Vector3.down * 0.3f, Quaternion.identity);
            Vector2 dir = (player.position - fp.position).normalized;
            Rigidbody2D rb = bullet.GetComponent<Rigidbody2D>();
            if (rb != null)
                rb.velocity = dir * 12f;

            yield return new WaitForSeconds(0.1f);
        }
    }

    IEnumerator ChangePattern()
    {
        while (!isDead)
        {
            currentPattern = (AttackPattern)Random.Range(0, 3);
            yield return new WaitForSeconds(10f);
        }
    }

    public void TakeDamage(float dmg)
    {
        if (isDead) return;

        life -= dmg;

        if (damageParticlesPrefab != null)
            Instantiate(damageParticlesPrefab, transform.position, Quaternion.identity);

        if (life <= 0)
            StartCoroutine(Die());
    }

    IEnumerator Die()
    {
        isDead = true;
        canAttack = false;

        for (int i = 0; i < 7; i++)
        {
            Vector3 offset = new Vector3(Random.Range(-1f, 1f), Random.Range(-1f, 1f), 0);
            Instantiate(explosionPrefab, transform.position + offset, Quaternion.identity);
            yield return new WaitForSeconds(0.2f);
        }

        Destroy(gameObject);
        DialogFinal.SetActive(true);

    }

    public void EnableAttack(bool enable)
    {
        isActive = enable;
    }

}
