using UnityEngine;
using System.Collections;

[RequireComponent(typeof(HealthSystem))]
[RequireComponent(typeof(Rigidbody2D))]
public class EnemyMelee : MonoBehaviour
{
    [Header("Ataque cuerpo a cuerpo")]
    public float damage = 3f;
    public float attackRange = 1.2f;
    public float detectRange = 5f;
    public float stopDistance = 1.4f;
    public float moveSpeed = 2f;
    public float attackCooldown = 1.5f;
    public Transform attackPoint;
    public LayerMask playerMask;

    [Header("Detección de suelo")]
    public Transform groundCheck;
    public float groundCheckRadius = 0.2f;
    public LayerMask groundMask;

    [Header("Referencias")]
    public Transform swordInHand;
    public Transform healthBar;

    [Header("Visual")]
    public float handOffsetX = 0.6f;
    public float handOffsetY = 0f;
    public Color embuidoColor = Color.black;

    private HealthSystem health;
    private Transform player;
    private Rigidbody2D rb;
    private SpriteRenderer swordRenderer;

    private float nextAttackTime;
    private bool isGrounded;
    private bool isEmbuido;
    private float baseDamage;
    private Color baseColor;

    // Datos base de la espada (posición y rotación)
    private Vector3 swordBaseLocalScale;
    private Vector3 swordBaseLocalPos;
    private float swordBaseZ;

    private int facing = 1; // 1 = derecha, -1 = izquierda

    void Start()
    {
        health = GetComponent<HealthSystem>();
        rb = GetComponent<Rigidbody2D>();
        player = GameObject.FindGameObjectWithTag("Player")?.transform;

        if (swordInHand != null)
        {
            swordRenderer = swordInHand.GetComponent<SpriteRenderer>();
            baseColor = swordRenderer != null ? swordRenderer.color : Color.gray;
            swordBaseLocalPos = swordInHand.localPosition;
            swordBaseLocalScale = swordInHand.localScale;
            swordBaseZ = swordInHand.localEulerAngles.z;
        }

        baseDamage = damage;
    }

    void Update()
    {
        if (!player || health.currentHealth <= 0) return;

        isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundMask);
        float distance = Vector2.Distance(transform.position, player.position);

        HandleOrientation();
        HandleMovement(distance);
        HandleAttack(distance);
        ApplyHandPose();
    }

    // 🔹 Detecta hacia dónde mirar y aplica flip
    void HandleOrientation()
    {
        float dir = player.position.x - transform.position.x;
        int newFacing = dir >= 0 ? 1 : -1;

        if (newFacing != facing)
        {
            facing = newFacing;
        }
    }

    // 🔹 Movimiento con detección de suelo
    void HandleMovement(float distance)
    {
        if (!isGrounded)
        {
            rb.velocity = new Vector2(0, rb.velocity.y);
            return;
        }

        if (distance < detectRange && distance > stopDistance)
        {
            rb.velocity = new Vector2(facing * moveSpeed, rb.velocity.y);
        }
        else
        {
            rb.velocity = new Vector2(0, rb.velocity.y);
        }
    }

    // 🔹 Ataque cuerpo a cuerpo
    void HandleAttack(float distance)
    {
        if (distance > attackRange || Time.time < nextAttackTime) return;

        StartCoroutine(EmbuirEspada());

        Collider2D hit = Physics2D.OverlapCircle(attackPoint.position, attackRange, playerMask);
        if (hit != null)
        {
            var hs = hit.GetComponent<HealthSystem>();
            if (hs != null)
            {
                hs.TakeDamage(damage);
                Debug.Log($"💥 {name} golpeó al jugador ({damage} daño)");
            }

            nextAttackTime = Time.time + attackCooldown;
        }
    }

    // 🔹 Posición e inclinación espejo de la espada (idéntico a Player)
    // 🔹 Posición e inclinación espejo de la espada (idéntico a Player)
    void ApplyHandPose()
    {
        if (swordInHand == null) return;

        int sign = facing; // mismo sistema que el player: 1 derecha, -1 izquierda

        // 🔸 Posición de la espada respecto al cuerpo
        swordInHand.localPosition = new Vector3(
            handOffsetX * sign,
            handOffsetY,
            swordBaseLocalPos.z
        );

        // 🔸 Rotación igual que el Player (inclinación espejo)
        Quaternion rotY = Quaternion.Euler(0f, (sign < 0 ? 180f : 0f), 0f);
        Quaternion rotZ = Quaternion.Euler(0f, 0f, swordBaseZ);
        swordInHand.localRotation = rotY * rotZ;

        // 🔸 Mantener escala base (no invertida)
        swordInHand.localScale = swordBaseLocalScale;

        // 🔸 Corrige barra de vida para no voltearla
        if (healthBar != null)
        {
            Vector3 barScale = healthBar.localScale;
            barScale.x = Mathf.Abs(barScale.x);
            healthBar.localScale = barScale;
        }

        // 🔸 Ajusta attackPoint para que siga la dirección
        if (attackPoint != null)
        {
            attackPoint.localPosition = new Vector3(
                Mathf.Abs(attackPoint.localPosition.x) * sign,
                attackPoint.localPosition.y,
                attackPoint.localPosition.z
            );
        }
    }


    // 🔹 Embrujo temporal de espada
    IEnumerator EmbuirEspada()
    {
        if (isEmbuido || swordRenderer == null) yield break;
        isEmbuido = true;

        swordRenderer.color = embuidoColor;
        yield return new WaitForSeconds(0.5f);
        swordRenderer.color = baseColor;

        isEmbuido = false;
    }

    void OnDrawGizmosSelected()
    {
        if (attackPoint != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(attackPoint.position, attackRange);
        }

        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, detectRange);

        if (groundCheck != null)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(groundCheck.position, groundCheckRadius);
        }
    }
}
