using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class PortalOnlyCube : MonoBehaviour
{
    [Header("Identificador de grupo")]
    [Tooltip("Solo los botones con el mismo GroupID afectarán a este portal.")]
    public string groupID = "A";

    [Header("Configuración de Portal")]
    [Tooltip("Referencia al otro portal (su pareja)")]
    public PortalOnlyCube linkedPortal;

    [Tooltip("Distancia mínima desde el portal donde aparece el cubo")]
    public float exitOffset = 0.6f;

    [Tooltip("Tiempo para evitar teletransportes dobles")]
    public float teleportCooldown = 0.25f;

    [Tooltip("Multiplicador del impulso de salida (1 = realista)")]
    public float momentumMultiplier = 1f;

    [Tooltip("Velocidad máxima permitida al salir")]
    public float maxExitSpeed = 100f;

    [Tooltip("Capas sólidas (Ground, Wall, etc.)")]
    public LayerMask environmentMask;

    private static readonly Dictionary<Rigidbody2D, float> cooldowns = new();
    private Collider2D portalCollider;

    void Awake()
    {
        portalCollider = GetComponent<Collider2D>();
        portalCollider.isTrigger = true;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (linkedPortal == null) return;
        if (!enabled || !linkedPortal.enabled) return;

        Rigidbody2D rb = other.attachedRigidbody;
        if (rb == null) return;

        // Solo objetos tipo Cube
        if (!other.CompareTag("Cube")) return;

        // Cooldown para evitar rebotes infinitos
        if (cooldowns.TryGetValue(rb, out float until) && Time.time < until)
            return;

        cooldowns[rb] = Time.time + teleportCooldown;
        StartCoroutine(TeleportCube(rb, linkedPortal));
    }

    private IEnumerator TeleportCube(Rigidbody2D rb, PortalOnlyCube destination)
    {
        if (rb == null || destination == null) yield break;
        if (!destination.gameObject.activeInHierarchy) yield break;

        // 🔹 Magnitud de velocidad de entrada
        float entrySpeed = rb.velocity.magnitude;

        // 🔹 Dirección de salida (usa 'up' para orientación real del portal)
        Vector2 exitDir = destination.transform.up.normalized;

        // 🔹 Cálculo del momentum realista
        float baseMomentum = Mathf.Lerp(0.5f, 1f, Mathf.Clamp01(entrySpeed / 12f));
        float exitSpeed = entrySpeed * momentumMultiplier * baseMomentum;
        exitSpeed = Mathf.Clamp(exitSpeed, 0f, maxExitSpeed);

        Vector2 vOut = exitDir * exitSpeed;

        // 🔹 Posición de salida (delante del portal)
        Vector3 exitPos = destination.transform.position + (Vector3)(exitDir * destination.exitOffset);

        // 🔹 Evitar glitch visual
        yield return new WaitForFixedUpdate();

        // 🔹 Evitar error si el cubo fue destruido (por CubeVanishOnPortal u otro)
        if (rb == null || rb.gameObject == null)
            yield break;

        // Teletransportar físicamente
        rb.position = exitPos;
        rb.velocity = vOut;

        // 🔹 Mantener dirección estable un corto tiempo
        yield return StartCoroutine(MaintainExitDirection(rb, exitDir, vOut.magnitude, 0.08f));

        cooldowns[rb] = Time.time + teleportCooldown;
    }

    // 🔹 Mantiene la dirección de salida un breve tiempo
    private IEnumerator MaintainExitDirection(Rigidbody2D rb, Vector2 dir, float speed, float duration)
    {
        if (rb == null) yield break;

        float timer = 0f;
        float originalGravity = rb.gravityScale;
        rb.gravityScale = 0f;

        while (timer < duration)
        {
            if (rb == null || rb.gameObject == null)
                yield break;

            rb.velocity = dir.normalized * speed;
            timer += Time.fixedDeltaTime;
            yield return new WaitForFixedUpdate();
        }

        if (rb != null)
            rb.gravityScale = originalGravity;
    }

    // 🔹 Gizmos para visualizar la salida
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.magenta;
        Vector3 dir = transform.up * exitOffset;
        Gizmos.DrawLine(transform.position, transform.position + dir);
        Gizmos.DrawWireSphere(transform.position + dir, 0.1f);

        if (linkedPortal != null)
            Gizmos.DrawLine(transform.position, linkedPortal.transform.position);
    }
}
