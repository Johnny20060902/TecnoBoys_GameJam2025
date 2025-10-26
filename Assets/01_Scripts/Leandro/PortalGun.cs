using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PortalGun : MonoBehaviour
{
    [Header("Referencias")]
    public Transform firePoint;                 // Punto de origen del disparo (mano del jugador)
    public GameObject portalBluePrefab;         // Prefab del portal azul
    public GameObject portalOrangePrefab;       // Prefab del portal naranja
    public LayerMask shootMask;                 // Capas válidas para colocar portales (Ground, Wall, etc.)

    [Header("Configuración")]
    public float maxDistance = 25f;             // Distancia máxima del disparo
    public float surfaceOffset = 0.05f;         // Separación del portal respecto a la superficie
    public float minPortalSpacing = 0.2f;       // Margen mínimo para no colocar portales superpuestos

    private GameObject portalBlue;
    private GameObject portalOrange;

    void Update()
    {
        // 🔹 Disparo del portal azul (clic izquierdo)
        if (Input.GetMouseButtonDown(0))
            ShootPortal(ref portalBlue, portalBluePrefab);

        // 🔹 Disparo del portal naranja (clic derecho)
        if (Input.GetMouseButtonDown(1))
            ShootPortal(ref portalOrange, portalOrangePrefab);
    }

    // ===========================================================
    //  🔸 Disparo del portal (colocación precisa y orientación real)
    // ===========================================================
    void ShootPortal(ref GameObject portalInstance, GameObject prefab)
    {
        if (firePoint == null) return;

        // 🧭 Dirección hacia el cursor del mouse (en 2D)
        Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector2 dir = (mousePos - (Vector2)firePoint.position).normalized;

        // 🔦 Lanza un raycast para detectar superficies válidas
        RaycastHit2D hit = Physics2D.Raycast(firePoint.position, dir, maxDistance, shootMask);

        if (hit.collider != null)
        {
            // 🔹 Punto exacto del impacto
            Vector2 hitPoint = hit.point;

            // 🔹 Normal de la superficie (dirección perpendicular)
            Vector2 surfaceNormal = hit.normal.normalized;

            // 🔹 Evitar que se coloque sobre esquinas imposibles
            if (hit.collider.isTrigger) return;

            // 🔹 Ajustar la posición del portal un poco fuera de la superficie
            Vector2 spawnPos = hitPoint + surfaceNormal * surfaceOffset;

            // =====================================================
            // 🔹 Cálculo exacto de rotación
            // =====================================================
            // Queremos que el portal "mire" hacia afuera de la superficie
            float angle = Mathf.Atan2(surfaceNormal.y, surfaceNormal.x) * Mathf.Rad2Deg;

            // El sprite del portal debe mirar "hacia la normal"
            // Si tu sprite está invertido (mira al revés), descomenta:
            // angle += 180f;

            Quaternion rot = Quaternion.Euler(0f, 0f, angle);

            // =====================================================
            // 🔹 Crear o mover el portal
            // =====================================================
            if (portalInstance == null)
            {
                portalInstance = Instantiate(prefab, spawnPos, rot);
            }
            else
            {
                // Si ya existe, simplemente lo movemos
                portalInstance.transform.SetPositionAndRotation(spawnPos, rot);
            }

            // =====================================================
            // 🔗 Enlazar automáticamente ambos portales
            // =====================================================
            if (portalBlue != null && portalOrange != null)
            {
                var blue = portalBlue.GetComponent<Portal2D>();
                var orange = portalOrange.GetComponent<Portal2D>();

                if (blue != null && orange != null)
                {
                    blue.linkedPortal = orange;
                    orange.linkedPortal = blue;
                }
            }

            // =====================================================
            // 🔍 Debug visual
            // =====================================================
            Debug.DrawRay(hitPoint, surfaceNormal * 0.5f, Color.green, 1f);
        }
        else
        {
            // 🔴 Si no golpea nada, dibuja la línea del disparo en rojo
            Debug.DrawRay(firePoint.position, dir * maxDistance, Color.red, 0.3f);
        }
    }

    // ===========================================================
    //  🔹 GIZMOS (ayuda visual en el editor)
    // ===========================================================
    void OnDrawGizmos()
    {
        if (firePoint == null || Camera.main == null) return;

        Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector2 dir = (mousePos - (Vector2)firePoint.position).normalized;

        Gizmos.color = Color.cyan;
        Gizmos.DrawLine(firePoint.position, firePoint.position + (Vector3)dir * maxDistance);
    }
}
