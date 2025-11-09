using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(SpriteRenderer))]
public class PlayerTransformation : MonoBehaviour
{
    [Header("Sprites")]
    public Sprite elarionSprite;
    public Sprite humanoSprite;

    [Header("Configuración")]
    public float changeCooldown = 3f;
    public Vector3 humanoOffset = new Vector3(0.15f, 0.55f, 0f); // Ajustable en el inspector

    private bool isElarion = true;
    private float lastChangeTime = -999f;

    private SpriteRenderer sr;
    private PlayerCombatJohnny combat;
    private Transform swordInHand;
    private Transform meleeOrigin;

    private Vector3 swordBasePos;
    private Vector3 meleeBasePos;

    void Start()
    {
        sr = GetComponent<SpriteRenderer>();
        combat = GetComponent<PlayerCombatJohnny>();

        swordInHand = transform.Find("SwordInHand");
        meleeOrigin = transform.Find("MeleeOrigin");

        if (swordInHand != null)
            swordBasePos = swordInHand.localPosition;
        if (meleeOrigin != null)
            meleeBasePos = meleeOrigin.localPosition;

        // 🔹 Detectar nivel actual
        string sceneName = SceneManager.GetActiveScene().name;
        bool isLevel1 = sceneName.Contains("Level1");

        // 🔹 Nivel 1 → empieza como Elarion
        // 🔹 Nivel 2+ → empieza como Humano
        isElarion = isLevel1;
        sr.sprite = isElarion ? elarionSprite : humanoSprite;

        transform.localScale = new Vector3(0.3f, 0.3f, 0.3f);
        AplicarRestricciones();

        // 🔹 Actualizar visuales iniciales
        if (combat != null)
            combat.UpdateVisuals();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.C))
            TryTransform();

        // 🔒 En forma Elarion, forzar modo disparo y desactivar espada siempre
        if (isElarion && combat != null)
        {
            combat.hasSword = false;
            combat.mode = 1;
            combat.UpdateVisuals();

            if (combat.swordInHand != null)
                combat.swordInHand.SetActive(false);
        }
    }

    void TryTransform()
    {
        if (Time.time - lastChangeTime < changeCooldown)
            return;

        lastChangeTime = Time.time;
        StartCoroutine(TransformRoutine());
    }

    IEnumerator TransformRoutine()
    {
        yield return new WaitForSeconds(0.05f);
        isElarion = !isElarion;

        float flip = Mathf.Sign(transform.localScale.x);
        transform.localScale = new Vector3(0.3f * flip, 0.3f, 0.3f);

        sr.sprite = isElarion ? elarionSprite : humanoSprite;
        AplicarRestricciones();
    }

    private void AplicarRestricciones()
    {
        if (combat == null) return;

        // 🔹 Desactivar autoalineación temporal (para evitar que ApplyHandPose reemplace la posición)
        bool wasActive = combat.enabled;
        combat.enabled = false;

        if (isElarion)
        {
            // 🌌 Elarion: solo disparos, sin espada
            combat.hasSword = false;
            combat.mode = 1;
            combat.autoAlignSword = true;
            combat.UpdateVisuals();

            if (combat.swordInHand != null)
                combat.swordInHand.SetActive(false);

            if (swordInHand != null)
                swordInHand.localPosition = swordBasePos;
            if (meleeOrigin != null)
                meleeOrigin.localPosition = meleeBasePos;

            Debug.Log("🌌 Elarion activo: solo disparos, sin espada ni cambio de modo.");
        }
        else
        {
            // 🧍 Humano: puede disparar y usar espada
            combat.hasSword = true;
            combat.mode = 2;
            combat.autoAlignSword = false;
            combat.UpdateVisuals();

            if (combat.swordInHand != null)
                combat.swordInHand.SetActive(true);

            // ⚙️ Aplicar offset visual
            if (swordInHand != null)
                swordInHand.localPosition = swordBasePos + humanoOffset;
            if (meleeOrigin != null)
                meleeOrigin.localPosition = meleeBasePos + humanoOffset;

            Debug.Log("🧍 Humano activo: espada y disparos habilitados (teclas 1 y 2).");
        }

        // ✅ Rehabilitar control de combate luego de reposicionar
        combat.enabled = wasActive;
    }

    public bool EsElarion()
    {
        return isElarion;
    }
}
