using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class PlayerCombatJohnny : MonoBehaviour
{
    public PlayerJohnny ctrl;
    [HideInInspector] public bool autoAlignSword = true;


    [Header("Rango")]
    public Transform firePoint;
    public BulletElarion bulletPrefab;
    public float fireCooldown = 0.5f;

    [Header("Espada")]
    [HideInInspector] public GameObject swordInHand;
    public Transform meleeOrigin;
    public float meleeRange = 1.1f;
    public float meleeHeight = 1f;
    public float swordDamage = 10f;
    public LayerMask enemyMask;

    [Header("Recolección de espada")]
    public float pickUpRange = 1.2f;
    public LayerMask swordLayer;

    [Header("Imbuir (Q)")]
    public float imbueMultiplier = 1.5f;
    public float imbueDuration = 6f;
    public Color imbuedColor = new Color(0.7f, 0.9f, 1f);

    [Header("Posición de la mano (se espeja en X)")]
    public float handOffsetX = 0.6f;
    public float handOffsetY = 0.0f;

    [HideInInspector] public bool hasSword;
    [HideInInspector] public int mode = 1; // 1 = disparo, 2 = espada

    float cd;
    bool imbued;

    float baseSwordDamage;
    Color baseSwordColor;

    Vector3 swordBaseLocalScale;
    float swordBaseZ;
    Vector3 swordBaseLocalPos;

    void Start()
    {
        baseSwordDamage = swordDamage;

        if (swordInHand != null)
        {
            var sr = swordInHand.GetComponent<SpriteRenderer>();
            baseSwordColor = sr != null ? sr.color : Color.white;
            swordBaseLocalScale = swordInHand.transform.localScale;
            swordBaseZ = swordInHand.transform.localEulerAngles.z;
            swordBaseLocalPos = swordInHand.transform.localPosition;
        }

        // 🔹 Detectar si estamos en el Nivel 1
        string sceneName = SceneManager.GetActiveScene().name;
        bool isLevel1 = sceneName.Contains("Level1");

        if (PlayerProgress.Instance != null)
        {
            if (isLevel1 && !PlayerProgress.Instance.hasSword)
            {
                // 🧠 Nivel 1 y todavía no la obtuvo → empieza sin espada
                hasSword = false;
                mode = 1;
                PlayerProgress.Instance.SetSwordObtained(false);

                if (swordInHand != null)
                {
                    swordInHand.SetActive(false);
                    var sr = swordInHand.GetComponent<SpriteRenderer>();
                    if (sr != null) sr.enabled = false;
                }

                Debug.Log("🗡️ Nivel 1: sin espada (debe recogerla con E)");
            }
            else
            {
                // ✅ Nivel 2 o ya tenía la espada guardada
                hasSword = true;
                mode = 2;
                UnlockSword();

                if (swordInHand != null)
                {
                    swordInHand.SetActive(true);
                    var sr = swordInHand.GetComponent<SpriteRenderer>();
                    if (sr != null) sr.enabled = true;
                }

                Debug.Log("✅ Espada equipada automáticamente (nivel 2 o progreso guardado)");
            }
        }
        else
        {
            Debug.LogWarning("⚠️ No hay instancia de PlayerProgress, creando sin espada...");
            hasSword = isLevel1 ? false : true;
        }

        UpdateVisuals();
        ApplyHandPose();
    }

    void Update()
    {
        cd -= Time.deltaTime;

        // Cambio entre modos
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            mode = 1;
            UpdateVisuals();
            Debug.Log("🔫 Modo 1: Disparo");
        }

        if (Input.GetKeyDown(KeyCode.Alpha2) && hasSword)
        {
            mode = 2;
            UpdateVisuals();
            Debug.Log("🗡️ Modo 2: Espada");
        }

        // Ataques
        if (Input.GetMouseButtonDown(0))
        {
            if (mode == 1) TryShoot();
            else if (mode == 2 && hasSword) Melee();
        }

        // Imbuir espada
        if (Input.GetKeyDown(KeyCode.Q) && hasSword)
            StartCoroutine(ImbueSword());

        // Recolectar espada (solo si no la tiene)
        if (!hasSword && Input.GetKeyDown(KeyCode.E))
            TryPickUpSword();

        ApplyHandPose();
    }

    void TryShoot()
    {
        if (cd > 0 || bulletPrefab == null || firePoint == null) return;
        var b = Instantiate(bulletPrefab, firePoint.position, Quaternion.identity);
        int sign = GetFacingSign();
        b.transform.right = (sign < 0) ? Vector3.left : Vector3.right;
        cd = fireCooldown;
    }


    void Melee()
    {
        if (!hasSword) return;

        Vector2 dir = (ctrl != null && ctrl.facing < 0) ? Vector2.left : Vector2.right;
        Vector2 origin = meleeOrigin != null ? (Vector2)meleeOrigin.position : (Vector2)transform.position;
        Vector2 center = origin + dir * (meleeRange * 0.5f);

        Collider2D[] hits = Physics2D.OverlapBoxAll(center, new Vector2(meleeRange, meleeHeight), 0f, enemyMask);

        foreach (var h in hits)
        {
            // ✅ 1️⃣ Daño a enemigos normales con HealthSystem
            var hs = h.GetComponent<HealthSystem>() ?? h.GetComponentInParent<HealthSystem>();
            if (hs != null)
            {
                hs.TakeDamage(swordDamage);
                Debug.Log($"⚔️ Golpeó a enemigo {h.name} ({swordDamage} daño)");
                continue;
            }

            // ✅ 2️⃣ Daño a Spawners destructibles
            var ds = h.GetComponent<DestructibleSpawner>() ?? h.GetComponentInParent<DestructibleSpawner>();
            if (ds != null)
            {
                ds.hp -= (int)swordDamage;
                Debug.Log($"💥 Golpeó al spawner {h.name}, HP restante: {ds.hp}");

                if (ds.hp <= 0)
                {
                    if (ds.destroyVfx != null)
                        Instantiate(ds.destroyVfx, ds.transform.position, Quaternion.identity);

                    Destroy(ds.gameObject);
                    Debug.Log($"💀 Spawner {h.name} destruido");
                }
            }
        }
    }


    IEnumerator ImbueSword()
    {
        if (imbued) yield break;
        imbued = true;
        swordDamage = baseSwordDamage * imbueMultiplier;

        if (swordInHand != null)
        {
            var sr = swordInHand.GetComponent<SpriteRenderer>();
            if (sr != null) sr.color = imbuedColor;
        }

        yield return new WaitForSeconds(imbueDuration);

        swordDamage = baseSwordDamage;
        if (swordInHand != null)
        {
            var sr = swordInHand.GetComponent<SpriteRenderer>();
            if (sr != null) sr.color = baseSwordColor;
        }
        imbued = false;
    }

    void TryPickUpSword()
    {
        var transformador = GetComponent<PlayerTransformation>();
        var sr = GetComponent<SpriteRenderer>();

        // 🚫 Bloqueo absoluto: si es Elarion o si su sprite es el de Elarion
        if ((transformador != null && transformador.EsElarion()) ||
            (sr != null && transformador != null && sr.sprite == transformador.elarionSprite))
        {
            Debug.Log("❌ Elarion no puede recoger ni usar espada.");
            return;
        }

        // ✅ Detección normal de espada cercana
        Collider2D swordNearby = Physics2D.OverlapCircle(transform.position, pickUpRange, swordLayer);
        if (swordNearby != null && !hasSword)
        {
            GrantSword();
            Destroy(swordNearby.gameObject);
            Debug.Log("🗡️ Espada recogida correctamente con tecla E");
        }
    }


    // === MÉTODOS PÚBLICOS USADOS POR WEAPONPICKUP ===
    public void UnlockSword()
    {
        hasSword = true;
        mode = 2;
        UpdateVisuals();
        ApplyHandPose();
    }

    public void GrantSword()
    {
        hasSword = true;
        mode = 2;
        UpdateVisuals();
        ApplyHandPose();

        if (PlayerProgress.Instance != null)
            PlayerProgress.Instance.SetSwordObtained(true);

        if (swordInHand != null)
        {
            swordInHand.SetActive(true);
            var sr = swordInHand.GetComponent<SpriteRenderer>();
            if (sr != null) sr.enabled = true;
        }

        Debug.Log("🗡️ Espada otorgada al jugador (pickup o script externo)");
    }

    public void UpdateVisuals()
    {
        if (swordInHand != null)
            swordInHand.SetActive(hasSword && mode == 2);
    }

    // Añade este helper dentro de la clase
    int GetFacingSign()
    {
        // Si usas flipX en el SpriteRenderer del player, podés leerlo aquí:
        // var sr = GetComponent<SpriteRenderer>();
        // if (sr != null) return sr.flipX ? -1 : 1;

        // Por defecto: usa el signo de la escala actual del player
        return transform.lossyScale.x < 0f ? -1 : 1;
    }

    // Reemplaza TU método ApplyHandPose() por este:
    public void ApplyHandPose()
    {
        if (!autoAlignSword) return; // 🔒 No reposicionar si está en modo humano
        if (swordInHand == null) return;

        int sign = GetFacingSign();

        // 1) Posición de la mano (espejo solo en X)
        Vector3 targetLocalPos;
        if (firePoint != null)
        {
            targetLocalPos = firePoint.localPosition;
            targetLocalPos.x = Mathf.Abs(targetLocalPos.x) * sign;
        }
        else
        {
            targetLocalPos = new Vector3(Mathf.Abs(handOffsetX) * sign, handOffsetY, swordBaseLocalPos.z);
        }
        swordInHand.transform.localPosition = targetLocalPos;

        // 2) NADA de rotación en Y. Mantén tu ángulo Z original (diagonal bonita).
        //    El espejo lo hace la escala X.
        swordInHand.transform.localEulerAngles = new Vector3(0f, 0f, swordBaseZ);

        // 3) Espejo con escala X (no cambies Y ni Z)
        var baseS = swordBaseLocalScale;
        swordInHand.transform.localScale = new Vector3(Mathf.Abs(baseS.x) * sign, baseS.y, baseS.z);

        // 4) Alineá el meleeOrigin al lado actual
        if (meleeOrigin != null)
        {
            var m = meleeOrigin.localPosition;
            m.x = Mathf.Abs(m.x) * sign;
            meleeOrigin.localPosition = m;
        }
    }


}
