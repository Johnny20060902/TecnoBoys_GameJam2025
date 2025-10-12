using UnityEngine;

public class PlayerCombatJohnny : MonoBehaviour
{
    public PlayerJohnny ctrl;
    [Header("Rango")]
    public Transform firePoint;           // un empty delante de la mano
    public BulletElarion bulletPrefab;
    public float fireCooldown = 0.25f;

    [Header("Espada")]
    public GameObject swordInHand;        // sprite/cuadrado como hijo del player
    public Transform meleeOrigin;         // punto al frente
    public float meleeRange = 1.1f;
    public float swordDamage = 2f;
    public LayerMask enemyMask;

    [Header("Imbuir (Q)")]
    public float imbueMultiplier = 2f;
    public float imbueDuration = 6f;
    public Color imbuedColor = new Color(0.7f, 0.9f, 1f);

    bool hasSword;
    int mode = 1; // 1 rango, 2 espada
    float cd;

    float baseSwordDamage;
    Color baseSwordColor;
    bool imbued;

    void Start()
    {
        baseSwordDamage = swordDamage;
        if (swordInHand != null) baseSwordColor = swordInHand.GetComponent<SpriteRenderer>()?.color ?? Color.white;
        UpdateVisuals();
    }

    void Update()
    {
        cd -= Time.deltaTime;

        // cambiar modo con 1/2
        if (Input.GetKeyDown(KeyCode.Alpha1)) { mode = 1; UpdateVisuals(); }
        if (Input.GetKeyDown(KeyCode.Alpha2) && hasSword) { mode = 2; UpdateVisuals(); }

        // imbuir con Q cuando hay espada
        if (Input.GetKeyDown(KeyCode.Q) && hasSword) StartCoroutine(ImbueSword());

        // atacar con click izquierdo
        if (Input.GetMouseButtonDown(0))
        {
            if (mode == 1) TryShoot();
            else if (mode == 2) Melee();
        }
    }

    void TryShoot()
    {
        if (cd > 0) return;
        var b = Instantiate(bulletPrefab, firePoint.position, Quaternion.identity);
        // orientar el proyectil hacia la derecha o izquierda según facing
        if (ctrl != null && ctrl.facing < 0) b.transform.right = Vector3.left;
        else b.transform.right = Vector3.right;

        cd = fireCooldown;
    }

    void Melee()
    {
        Vector2 dir = (ctrl != null && ctrl.facing < 0) ? Vector2.left : Vector2.right;
        Vector2 center = (Vector2)meleeOrigin.position + dir * (meleeRange * 0.5f);
        var hits = Physics2D.OverlapBoxAll(center, new Vector2(meleeRange, 1f), 0, enemyMask);
       foreach (var h in hits)
       {
           var e = h.GetComponent<Enemy>();
           if (e != null) e.TakeDamage(swordDamage);
       
           var sp = h.GetComponent<DestructibleSpawner>();
           if (sp != null) sp.TakeDamage(swordDamage);
       }

    }

    System.Collections.IEnumerator ImbueSword()
    {
        if (imbued) yield break;
        imbued = true;
        swordDamage = baseSwordDamage * imbueMultiplier;
        if (swordInHand != null)
        {
            var sr = swordInHand.GetComponent<SpriteRenderer>();
            if (sr != null) sr.color = imbuedColor;
        }
        // avisar al tutorial
        FindObjectOfType<TutorialManager>()?.NotifyImbue();
        yield return new WaitForSeconds(imbueDuration);
        swordDamage = baseSwordDamage;
        if (swordInHand != null)
        {
            var sr = swordInHand.GetComponent<SpriteRenderer>();
            if (sr != null) sr.color = baseSwordColor;
        }
        imbued = false;
    }

    public void GrantSword()
    {
        hasSword = true;
        mode = 2;
        UpdateVisuals();
        FindObjectOfType<TutorialManager>()?.NotifyPickedSword();
    }

    void UpdateVisuals()
    {
        if (swordInHand != null) swordInHand.SetActive(hasSword && mode == 2);
    }
}
