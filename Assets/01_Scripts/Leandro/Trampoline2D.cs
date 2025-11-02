using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class Trampoline2D : MonoBehaviour
{
    [Header("Configuración del Trampolín")]
    [Tooltip("Fuerza con la que impulsa al jugador hacia arriba.")]
    public float bounceForce = 14f;

    [Tooltip("Tiempo mínimo entre rebotes para evitar spam.")]
    public float bounceCooldown = 0.1f;

    [Tooltip("Si quieres que el trampolín solo funcione con el jugador.")]
    public bool onlyAffectsPlayer = true;

    [Header("Efectos opcionales")]
    public Animator animator;
    public string bounceTriggerName = "Bounce"; // Nombre del trigger de animación

    private float lastBounceTime;

    void Awake()
    {
        Collider2D col = GetComponent<Collider2D>();
        col.isTrigger = false; // debe ser sólido
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        // Evitar múltiples rebotes rápidos
        if (Time.time - lastBounceTime < bounceCooldown) return;
        lastBounceTime = Time.time;

        // Filtrar por jugador si corresponde
        if (onlyAffectsPlayer && !collision.collider.CompareTag("Player")) return;

        Rigidbody2D rb = collision.collider.attachedRigidbody;
        if (rb == null) return;

        // Aplica rebote fuerte hacia arriba
        Vector2 bounceDir = Vector2.up;
        rb.velocity = new Vector2(rb.velocity.x, 0f); // reinicia la velocidad vertical
        rb.AddForce(bounceDir * bounceForce, ForceMode2D.Impulse);

        // Activar animación
        if (animator != null && !string.IsNullOrEmpty(bounceTriggerName))
        {
            animator.SetTrigger(bounceTriggerName);
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireCube(transform.position + Vector3.up * 0.2f, new Vector3(1f, 0.3f, 0));
    }
}
