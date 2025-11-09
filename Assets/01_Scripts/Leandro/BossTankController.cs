using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

/// <summary>
/// Boss tanque al estilo Metal Slug.
/// Movimiento con inercia, efectos visuales y fases cinematográficas.
/// </summary>
public class BossTankController : MonoBehaviour
{
    [Header("Vida")]
    public int maxHealth = 250;
    public int currentHealth;

    [Header("Referencias")]
    public Transform player;
    public Transform fireMain;
    public Transform fireLeft;
    public Transform fireRight;
    public Transform missileLauncher;
    public GameObject bulletPrefab;
    public GameObject heavyBulletPrefab;
    public GameObject missilePrefab;
    public GameObject minePrefab;
    public GameObject muzzleFlashPrefab;
    public GameObject hitEffectPrefab;
    public GameObject smokePrefab;

    [Header("UI")]
    public Canvas bossCanvas;
    public Image healthBarFill;

    [Header("Movimiento")]
    public float moveSpeed = 1.5f;
    public float acceleration = 3f;
    public float patrolRange = 8f;
    public float patrolPause = 0.3f;
    private Vector3 startPos;
    private float targetSpeed;
    private float currentSpeed;
    private bool reversing;

    [Header("Proyectiles")]
    public float bulletSpeed = 10f;
    public float heavyForce = 12f;
    public float missileDelay = 1.2f;

    [Header("Tiempos")]
    public float fireDelay = 0.35f;
    public float patternCooldown = 1.2f;

    [Header("Telegraph")]
    public float flashTime = 0.25f;
    public Color flashColor = Color.red;

    [Header("Efectos visuales")]
    public float shakeMagnitude = 0.08f;
    public float smokeSpawnRate = 0.5f;
    private float smokeTimer;

    private SpriteRenderer sr;
    private Color baseColor;
    private bool facingRight = true;
    private bool isAttacking;
    private bool enragedPhase2;
    private bool finalPhase;
    private bool introDone;

    private int lastPattern = -1;
    private float leftBound, rightBound;

    private enum State { Intro, Patrol, Attack, Dead }
    private State state = State.Intro;

    private AudioSource audioSource;

    private void Start()
    {
        sr = GetComponentInChildren<SpriteRenderer>();
        if (sr) baseColor = sr.color;

        currentHealth = maxHealth;
        startPos = transform.position;
        leftBound = startPos.x - patrolRange;
        rightBound = startPos.x + patrolRange;
        audioSource = GetComponent<AudioSource>();

        StartCoroutine(IntroSequence());
        InitUI();
    }

    private void InitUI()
    {
        if (bossCanvas)
        {
            Canvas canvas = Instantiate(bossCanvas);
            healthBarFill = canvas.GetComponentInChildren<Image>();
        }
    }

    private void Update()
    {
        if (!introDone || state == State.Dead || !player) return;

        FacePlayer();

        if (state == State.Patrol && !isAttacking)
            HandleMovement();

        UpdateUI();
        HandleSmokeEffects();
    }

    // ============================================================
    // MOVIMIENTO CON INERCIA
    // ============================================================
    private void HandleMovement()
    {
        float dir = facingRight ? 1f : -1f;
        targetSpeed = dir * moveSpeed;
        currentSpeed = Mathf.Lerp(currentSpeed, targetSpeed, Time.deltaTime * acceleration);

        transform.Translate(Vector2.right * currentSpeed * Time.deltaTime);

        // Vibración de motor
        transform.localPosition += (Vector3)(Random.insideUnitCircle * 0.003f);

        if (transform.position.x > rightBound && !reversing)
        {
            reversing = true;
            facingRight = false;
            Flip();
            StartCoroutine(PatrolPause());
        }
        else if (transform.position.x < leftBound && !reversing)
        {
            reversing = true;
            facingRight = true;
            Flip();
            StartCoroutine(PatrolPause());
        }
    }

    private IEnumerator PatrolPause()
    {
        float cacheSpeed = moveSpeed;
        moveSpeed = 0;
        yield return new WaitForSeconds(patrolPause);
        moveSpeed = cacheSpeed;
        reversing = false;
    }

    private void Flip()
    {
        Vector3 s = transform.localScale;
        s.x *= -1;
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

    // ============================================================
    // BUCLE PRINCIPAL
    // ============================================================
    private IEnumerator IntroSequence()
    {
        yield return new WaitForSeconds(1.5f);
        introDone = true;
        state = State.Patrol;
        StartCoroutine(MainLoop());
    }

    private IEnumerator MainLoop()
    {
        while (currentHealth > 0)
        {
            if (!isAttacking)
            {
                isAttacking = true;
                state = State.Attack;

                int pattern = ChoosePattern();
                yield return ExecutePattern(pattern);

                yield return new WaitForSeconds(Random.Range(patternCooldown * 0.9f, patternCooldown * 1.3f));
                state = State.Patrol;
                isAttacking = false;
            }
            yield return null;
        }
    }

    private int ChoosePattern()
    {
        List<int> patterns = new();

        if (!enragedPhase2 && !finalPhase)
            patterns.AddRange(new[] { 0, 1, 2, 3 });
        else if (enragedPhase2 && !finalPhase)
            patterns.AddRange(new[] { 0, 1, 2, 3, 4, 5 });
        else if (finalPhase)
            patterns.AddRange(new[] { 0, 1, 2, 3, 4, 5, 6 });

        int chosen;
        int safety = 10;
        do
        {
            chosen = patterns[Random.Range(0, patterns.Count)];
            safety--;
        } while (chosen == lastPattern && safety > 0);

        lastPattern = chosen;
        return chosen;
    }

    private IEnumerator ExecutePattern(int id)
    {
        switch (id)
        {
            case 0: yield return Pattern_MachineGunSweep(); break;
            case 1: yield return Pattern_HeavyBombard(); break;
            case 2: yield return Pattern_MissileSalvo(); break;
            case 3: yield return Pattern_WalkingFire(); break;
            case 4: yield return Pattern_MineAndSpray(); break;
            case 5: yield return Pattern_FlankShot(); break;
            case 6: yield return Pattern_FuryChain(); break;
        }
    }

    // ============================================================
    // PATRONES
    // ============================================================
    private IEnumerator Pattern_MachineGunSweep()
    {
        yield return FlashTelegraph();
        for (int i = 0; i < (enragedPhase2 ? 4 : 3); i++)
        {
            FireBulletWithSpread(fireLeft, 7);
            FireBulletWithSpread(fireRight, 7);
            yield return new WaitForSeconds(fireDelay);
        }
    }

    private IEnumerator Pattern_HeavyBombard()
    {
        yield return FlashTelegraph();
        int shots = finalPhase ? 6 : enragedPhase2 ? 4 : 3;
        for (int i = 0; i < shots; i++)
        {
            FireHeavyArc(fireMain, 0.3f);
            RecoilEffect(0.2f);
            yield return new WaitForSeconds(0.6f);
        }
    }

    private IEnumerator Pattern_MissileSalvo()
    {
        yield return FlashTelegraph();
        int missiles = finalPhase ? 7 : enragedPhase2 ? 5 : 3;
        for (int i = 0; i < missiles; i++)
        {
            GameObject missile = Instantiate(missilePrefab, missileLauncher.position, Quaternion.identity);
            var hm = missile.GetComponent<HomingMissile>();
            if (hm && player) hm.SetTarget(player);
            Destroy(missile, 6f);
            yield return new WaitForSeconds(missileDelay * Random.Range(0.7f, 1.1f));
        }
    }

    private IEnumerator Pattern_WalkingFire()
    {
        yield return FlashTelegraph();
        Vector3 target = transform.position + new Vector3(facingRight ? 3 : -3, 0, 0);
        yield return MoveTo(target, 1f);
        for (int i = 0; i < (enragedPhase2 ? 4 : 2); i++)
        {
            FireBulletWithSpread(fireLeft, 6);
            FireBulletWithSpread(fireRight, 6);
            yield return new WaitForSeconds(fireDelay);
        }
        yield return new WaitForSeconds(0.3f);
        FireHeavyArc(fireMain, 0.25f);
        RecoilEffect(0.3f);
        yield return MoveTo(startPos, 1f);
    }

    private IEnumerator Pattern_MineAndSpray()
    {
        yield return FlashTelegraph();
        if (minePrefab && fireMain)
        {
            GameObject mine = Instantiate(minePrefab, fireMain.position, Quaternion.identity);
            Rigidbody2D rb = mine.GetComponent<Rigidbody2D>();
            if (rb)
                rb.AddForce((facingRight ? Vector2.right : Vector2.left) * 4f + Vector2.up * 2f, ForceMode2D.Impulse);
            Destroy(mine, 8f);
        }
        FireBulletWithSpread(fireLeft, 5);
        FireBulletWithSpread(fireRight, 5);
        yield return new WaitForSeconds(0.8f);
    }

    private IEnumerator Pattern_FlankShot()
    {
        yield return FlashTelegraph();
        for (int i = 0; i < (finalPhase ? 4 : 3); i++)
        {
            FireBulletFan(fireMain, 5, 25);
            yield return new WaitForSeconds(fireDelay * 1.2f);
        }
    }

    private IEnumerator Pattern_FuryChain()
    {
        yield return FlashTelegraph();
        StartCoroutine(Pattern_MachineGunSweep());
        yield return new WaitForSeconds(0.6f);
        StartCoroutine(Pattern_MissileSalvo());
        yield return new WaitForSeconds(0.5f);
        FireHeavyArc(fireMain, 0.35f);
        RecoilEffect(0.3f);
        yield return new WaitForSeconds(1f);
    }

    // ============================================================
    // DISPAROS
    // ============================================================
    private void FireBulletWithSpread(Transform fp, float spread)
    {
        if (!fp || !player) return;
        Vector2 dir = (player.position - fp.position).normalized;
        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg + Random.Range(-spread, spread);
        SpawnBullet(fp.position, angle, bulletSpeed);
        SpawnMuzzleFlash(fp);
    }

    private void FireBulletFan(Transform fp, int count, float spread)
    {
        if (!fp || !player) return;
        Vector2 dir = (player.position - fp.position).normalized;
        float baseAngle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        for (int i = 0; i < count; i++)
        {
            float offset = Mathf.Lerp(-spread / 2, spread / 2, (float)i / (count - 1));
            SpawnBullet(fp.position, baseAngle + offset, bulletSpeed);
        }
        SpawnMuzzleFlash(fp);
    }

    private void FireHeavyArc(Transform fp, float upBias)
    {
        if (!fp || !player) return;
        Vector2 dir = (player.position - fp.position).normalized;
        GameObject shell = Instantiate(heavyBulletPrefab, fp.position, Quaternion.identity);
        Rigidbody2D rb = shell.GetComponent<Rigidbody2D>();
        rb.gravityScale = 0.6f;
        rb.AddForce((dir + Vector2.up * upBias) * heavyForce, ForceMode2D.Impulse);
        SpawnMuzzleFlash(fp);
        Destroy(shell, 6f);
    }

    private void SpawnBullet(Vector3 pos, float angle, float speed)
    {
        GameObject bullet = Instantiate(bulletPrefab, pos, Quaternion.Euler(0, 0, angle));
        Rigidbody2D rb = bullet.GetComponent<Rigidbody2D>();
        if (rb)
            rb.velocity = new Vector2(Mathf.Cos(angle * Mathf.Deg2Rad), Mathf.Sin(angle * Mathf.Deg2Rad)) * speed;
        Destroy(bullet, 3.5f);
    }

    private void SpawnMuzzleFlash(Transform fp)
    {
        if (!muzzleFlashPrefab) return;
        GameObject flash = Instantiate(muzzleFlashPrefab, fp.position, fp.rotation);
        Destroy(flash, 0.2f);
    }

    private void RecoilEffect(float intensity)
    {
        Vector3 original = transform.position;
        Vector3 offset = new Vector3(facingRight ? -0.2f : 0.2f, 0, 0);
        transform.position = original + offset * intensity;
        StartCoroutine(ResetRecoil(original));
    }

    private IEnumerator ResetRecoil(Vector3 origin)
    {
        yield return new WaitForSeconds(0.1f);
        transform.position = origin;
    }

    private IEnumerator MoveTo(Vector3 target, float time)
    {
        Vector3 start = transform.position;
        float t = 0;
        while (t < time)
        {
            transform.position = Vector3.Lerp(start, target, t / time);
            t += Time.deltaTime;
            yield return null;
        }
    }

    private IEnumerator FlashTelegraph()
    {
        if (!sr) yield break;
        sr.color = flashColor;
        yield return new WaitForSeconds(flashTime);
        sr.color = baseColor;
    }

    // ============================================================
    // VIDA / EFECTOS
    // ============================================================
    public void TakeDamage(int dmg)
    {
        if (state == State.Dead) return;
        currentHealth -= dmg;
        UpdateUI();

        if (hitEffectPrefab)
            Instantiate(hitEffectPrefab, transform.position, Quaternion.identity);

        StartCoroutine(DamageFlash());

        if (!enragedPhase2 && currentHealth <= maxHealth * 0.5f)
        {
            enragedPhase2 = true;
            moveSpeed *= 1.2f;
            fireDelay *= 0.85f;
            missileDelay *= 0.85f;
            Debug.Log("⚠️ ENTRA EN FASE 2 (agresiva)");
        }

        if (!finalPhase && currentHealth <= maxHealth * 0.25f)
        {
            finalPhase = true;
            moveSpeed *= 1.3f;
            patternCooldown *= 0.75f;
            Debug.Log("💢 FASE FINAL — FURIA TOTAL");
        }

        if (currentHealth <= 0)
        {
            StartCoroutine(DeathSequence());
        }
    }

    private IEnumerator DamageFlash()
    {
        for (int i = 0; i < 2; i++)
        {
            sr.color = flashColor;
            yield return new WaitForSeconds(0.08f);
            sr.color = baseColor;
        }
    }

    private IEnumerator DeathSequence()
    {
        state = State.Dead;
        Debug.Log("💥 Tanque destruido");
        for (int i = 0; i < 10; i++)
        {
            transform.position += (Vector3)Random.insideUnitCircle * 0.15f;
            sr.color = (i % 2 == 0) ? flashColor : baseColor;
            yield return new WaitForSeconds(0.1f);
        }
        Destroy(gameObject);
        SceneManager.LoadScene("Johnny_FinalBattle");
    }

    // ============================================================
    // EFECTOS / UI
    // ============================================================
    private void HandleSmokeEffects()
    {
        if (!finalPhase || !smokePrefab) return;
        smokeTimer += Time.deltaTime;
        if (smokeTimer >= smokeSpawnRate)
        {
            smokeTimer = 0;
            Vector3 pos = transform.position + new Vector3(Random.Range(-1f, 1f), Random.Range(0.5f, 1.5f), 0);
            Instantiate(smokePrefab, pos, Quaternion.identity);
        }
    }

    private void UpdateUI()
    {
        if (healthBarFill)
        {
            float fill = Mathf.Clamp01((float)currentHealth / maxHealth);
            healthBarFill.fillAmount = fill;
        }
    }

    // ============================================================
    // COLISIONES
    // ============================================================
    private void OnTriggerEnter2D(Collider2D col)
    {
        if (col.CompareTag("PlayerBullet"))
        {
            TakeDamage(10);
            Destroy(col.gameObject);
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawLine(startPos + Vector3.left * patrolRange, startPos + Vector3.right * patrolRange);
    }
}
