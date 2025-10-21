using UnityEngine;
using System.Collections;

[RequireComponent(typeof(HealthSystem))]
[RequireComponent(typeof(Rigidbody2D))]
public class EnemyMelee : MonoBehaviour
{
    [Header("Ataque cuerpo a cuerpo")]
    public float damage = 3f;
    public float attackRange = 1.2f;
    public float detectRange = 4f;
    public float stopDistance = 1.5f;
    public float moveSpeed = 2f;
    public float attackCooldown = 1.5f;
    public Transform attackPoint;
    public LayerMask playerMask;

    [Header("Referencias")]
    public Transform swordInHand;  // 👈 Asigná aquí el objeto de la espada
    public Transform healthBar;    // 👈 Asigná aquí el canvas/barra de vida

    private float nextAttackTime;
    private HealthSystem health;
    private Transform player;
    private Rigidbody2D rb;

    void Start()
    {
        health = GetComponent<HealthSystem>();
        rb = GetComponent<Rigidbody2D>();

        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null)
            player = playerObj.transform;
    }

    void Update()
    {
        if (!player || health.currentHealth <= 0) return;

        float distance = Vector2.Distance(transform.position, player.position);

        HandleOrientation();
        HandleMovement(distance);
        HandleAttack(distance);
    }

    // 🔹 Gira visualmente el enemigo hacia el jugador y ajusta hijos
    void HandleOrientation()
    {
        if (!player) return;

        // Detecta si el jugador está a la derecha o izquierda
        bool playerIsRight = player.position.x > transform.position.x;

        // Reorienta visualmente el enemigo (sin depender de scale.x)
        Vector3 localScale = transform.localScale;
        localScale.x = Mathf.Abs(localScale.x) * (playerIsRight ? 1 : -1);
        transform.localScale = localScale;

        // 🔹 Asegura que la barra de vida no se invierta
        if (healthBar != null)
        {
            Vector3 barScale = healthBar.localScale;
            barScale.x = Mathf.Abs(barScale.x);
            healthBar.localScale = barScale;
        }

        // 🔹 Ajusta AttackPoint y Espada en el lado correcto
        if (attackPoint != null)
        {
            Vector3 pos = attackPoint.localPosition;
            pos.x = Mathf.Abs(pos.x) * (playerIsRight ? 1 : -1);
            attackPoint.localPosition = pos;
        }

        if (swordInHand != null)
        {
            Vector3 swordPos = swordInHand.localPosition;
            swordPos.x = Mathf.Abs(swordPos.x) * (playerIsRight ? 1 : -1);
            swordInHand.localPosition = swordPos;
        }
    }

    // 🔹 Movimiento simple hacia el jugador
    void HandleMovement(float distance)
    {
        if (distance < detectRange && distance > stopDistance)
        {
            Vector2 dir = (player.position - transform.position).normalized;
            rb.velocity = new Vector2(dir.x * moveSpeed, rb.velocity.y);
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

    void OnDrawGizmosSelected()
    {
        if (attackPoint != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(attackPoint.position, attackRange);
        }

        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, detectRange);
    }
}
