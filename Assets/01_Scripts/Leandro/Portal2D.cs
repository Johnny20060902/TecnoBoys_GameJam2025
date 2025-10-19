using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class Portal2D : MonoBehaviour
{
    [Header("Configuración General")]
    [Tooltip("Marcar en el prefab azul si este portal es el azul")]
    public bool isBlue;
    [HideInInspector] public Portal2D linkedPortal;

    [Header("Teletransporte")]
    [Tooltip("Distancia mínima desde el portal donde aparece el objeto")]
    public float exitOffset = 0.6f;

    [Tooltip("Tiempo para evitar teletransportes infinitos")]
    public float teleportCooldown = 0.25f;

    [Tooltip("Multiplicador del impulso de salida (1 = realista)")]
    public float momentumMultiplier = 1f;

    [Tooltip("Velocidad mínima de salida")]
    public float minExitBoost = 6f;

    [Tooltip("Velocidad máxima permitida al salir")]
    public float maxExitSpeed = 120f;

    [Tooltip("Capas sólidas (Ground, Wall, etc.)")]
    public LayerMask environmentMask;

    // Control de cooldown individual (por Rigidbody)
    private static readonly Dictionary<Rigidbody2D, float> cooldownUntil = new Dictionary<Rigidbody2D, float>();
    private Collider2D triggerCol;

    void Awake()
    {
        triggerCol = GetComponent<Collider2D>();
        triggerCol.isTrigger = true;
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (linkedPortal == null) return;
        if (!other || other.isTrigger) return;
        if (!other.CompareTag("Player") && !other.CompareTag("Cube")) return;

        Rigidbody2D rb = other.attachedRigidbody;
        if (rb == null) return;

        // evitar rebotes infinitos
        if (cooldownUntil.TryGetValue(rb, out float until) && Time.time < until)
            return;

        if (!linkedPortal.gameObject.activeInHierarchy) return;

        PlayerGrab grab = other.GetComponent<PlayerGrab>();
        Rigidbody2D heldCube = (grab != null && grab.IsHoldingCube()) ? grab.GetHeldCube() : null;

        StartCoroutine(TeleportRoutine(other, linkedPortal, heldCube));
    }

    // ============================================================
    // 🌀 TELETRANSPORTE — siempre sale en la dirección del frente
    // ============================================================
    // ============================================================
// 🌀 TELETRANSPORTE — salida realista sin exceso de impulso
// ============================================================
private IEnumerator TeleportRoutine(Collider2D other, Portal2D destination, Rigidbody2D heldCube)
{
    Rigidbody2D rb = other.attachedRigidbody;
    if (rb == null) yield break;

    cooldownUntil[rb] = Time.time + teleportCooldown;

    // 🔹 Magnitud de velocidad de entrada (puede ser 0 si está quieto)
    float entrySpeed = rb.velocity.magnitude;

    // 🔹 Dirección de salida
    Vector2 exitDir = destination.transform.right.normalized;

    // 🔹 Salida realista: si entra rápido, conserva parte del momentum; si entra lento, apenas sale
    float baseMomentum = Mathf.Lerp(0.5f, 1f, Mathf.Clamp01(entrySpeed / 12f)); // menos fuerza si entra despacio
    float exitSpeed = entrySpeed * momentumMultiplier * baseMomentum;

    // 🔹 Limitar salida dentro de rango controlado
    exitSpeed = Mathf.Clamp(exitSpeed, 0f, maxExitSpeed);

    Vector2 vOut = exitDir * exitSpeed;

    // 🔹 Posición de salida segura
    Vector3 exitPos = FindSafeExitPosition(destination.transform.position, exitDir, other);

    yield return new WaitForFixedUpdate();

    rb.position = exitPos;
    rb.velocity = vOut;

    // 🔹 Transición limpia (sin sobresalto)
    yield return StartCoroutine(MaintainExitDirection(rb, exitDir, vOut.magnitude, 0.1f));

    // Avisar al jugador
    PlayerLeandro player = other.GetComponent<PlayerLeandro>();
    if (player != null)
        player.OnPortalExit(vOut, teleportCooldown);

    // 🔹 Teletransportar cubo si lo tiene agarrado
    if (heldCube != null)
    {
        cooldownUntil[heldCube] = Time.time + teleportCooldown;

        float cubeEntry = heldCube.velocity.magnitude;
        float cubeMomentum = Mathf.Lerp(0.5f, 1f, Mathf.Clamp01(cubeEntry / 12f));
        float cubeSpeed = cubeEntry * momentumMultiplier * cubeMomentum;
        cubeSpeed = Mathf.Clamp(cubeSpeed, 0f, maxExitSpeed);

        Vector2 cubeVOut = exitDir * cubeSpeed;
        Vector3 cubeExitPos = destination.transform.position + (Vector3)(exitDir * (exitOffset + 0.4f));

        yield return new WaitForFixedUpdate();
        heldCube.position = cubeExitPos;
        heldCube.velocity = cubeVOut;
    }
}


    // ============================================================
    // 🔸 Mantener dirección de salida por breve tiempo
    // ============================================================
    private IEnumerator MaintainExitDirection(Rigidbody2D rb, Vector2 dir, float speed, float duration)
    {
        float timer = 0f;
        float originalGravity = rb.gravityScale;
        rb.gravityScale = 0f;

        while (timer < duration)
        {
            rb.velocity = dir.normalized * speed;
            timer += Time.fixedDeltaTime;
            yield return new WaitForFixedUpdate();
        }

        rb.gravityScale = originalGravity;
    }

    // ============================================================
    // 🔸 Busca posición segura fuera del portal
    // ============================================================
    private Vector3 FindSafeExitPosition(Vector3 portalPos, Vector2 dir, Collider2D obj)
    {
        float safeDist = exitOffset;
        RaycastHit2D hit = Physics2D.Raycast(portalPos, dir, exitOffset, environmentMask);
        if (hit.collider != null)
            safeDist = Mathf.Max(0.1f, hit.distance - 0.01f);

        Vector3 exit = portalPos + (Vector3)(dir * safeDist);

        // ajuste vertical si es una pared
        if (Mathf.Abs(dir.y) < 0.5f && obj != null)
            exit.y += obj.bounds.extents.y * 0.5f;

        return exit;
    }

    // ============================================================
    // 🔸 Visualización del portal en escena
    // ============================================================
    void OnDrawGizmosSelected()
    {
        Gizmos.color = isBlue ? Color.cyan : new Color(1f, 0.5f, 0f);
        Vector3 dir = transform.right * exitOffset;
        Gizmos.DrawLine(transform.position, transform.position + dir);
        Gizmos.DrawWireSphere(transform.position + dir, 0.1f);
    }
    
}
