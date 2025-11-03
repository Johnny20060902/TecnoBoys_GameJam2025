using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PortalGun : MonoBehaviour
{
    [Header("Referencias")]
    public Transform firePoint;                 
    public GameObject portalBluePrefab;         
    public GameObject portalOrangePrefab;       
    public LayerMask shootMask;                 

    [Header("Configuración")]
    public float maxDistance = 25f;             
    public float surfaceOffset = 0.05f;         
    public float minPortalSpacing = 0.2f;       

    private GameObject portalBlue;
    private GameObject portalOrange;
    private bool tienePistola = false;

    // 🌍 Se mantiene entre escenas (memoria + guardado en disco)
    public static bool pistolaObtenida = false;

    // ==========================================================
    void Awake()
    {
        // Restaurar desde PlayerPrefs si venimos de otra escena
        if (PlayerPrefs.GetInt("TienePistola", 0) == 1)
        {
            pistolaObtenida = true;
            tienePistola = true;

            // Mostrar el modelo en la mano
            Transform pistol = firePoint.Find("PistolInHand");
            if (pistol != null)
                pistol.gameObject.SetActive(true);
        }
    }

    void Update()
    {
        if (!tienePistola)
            return; 

        if (Input.GetMouseButtonDown(0))
            ShootPortal(ref portalBlue, portalBluePrefab);

        if (Input.GetMouseButtonDown(1))
            ShootPortal(ref portalOrange, portalOrangePrefab);
    }

    public void ActivarPistola()
    {
        tienePistola = true;
        pistolaObtenida = true;

        // Guardar estado permanente
        PlayerPrefs.SetInt("TienePistola", 1);
        PlayerPrefs.Save();

        // Mostrar visualmente
        Transform pistol = firePoint.Find("PistolInHand");
        if (pistol != null)
            pistol.gameObject.SetActive(true);
    }

    // ==========================================================
    void ShootPortal(ref GameObject portalInstance, GameObject prefab)
    {
        if (firePoint == null) return;

        Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector2 dir = (mousePos - (Vector2)firePoint.position).normalized;
        RaycastHit2D hit = Physics2D.Raycast(firePoint.position, dir, maxDistance, shootMask);

        if (hit.collider != null)
        {
            Vector2 hitPoint = hit.point;
            Vector2 surfaceNormal = hit.normal.normalized;
            if (hit.collider.isTrigger) return;

            Vector2 spawnPos = hitPoint + surfaceNormal * surfaceOffset;
            float angle = Mathf.Atan2(surfaceNormal.y, surfaceNormal.x) * Mathf.Rad2Deg;
            Quaternion rot = Quaternion.Euler(0f, 0f, angle);

            if (portalInstance == null)
                portalInstance = Instantiate(prefab, spawnPos, rot);
            else
                portalInstance.transform.SetPositionAndRotation(spawnPos, rot);

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

            Debug.DrawRay(hitPoint, surfaceNormal * 0.5f, Color.green, 1f);
        }
        else
        {
            Debug.DrawRay(firePoint.position, dir * maxDistance, Color.red, 0.3f);
        }
    }

    // ==========================================================
    void LateUpdate()
    {
        if (!tienePistola || firePoint == null) return;

        Vector3 mouseWorldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mouseWorldPos.z = 0;

        Vector3 dir = (mouseWorldPos - firePoint.position).normalized;
        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;

        firePoint.rotation = Quaternion.Euler(0f, 0f, angle);

        if (Mathf.Abs(angle) > 90)
            firePoint.localScale = new Vector3(1, -1, 1);
        else
            firePoint.localScale = new Vector3(1, 1, 1);
    }

    // ==========================================================
    void OnDrawGizmos()
    {
        if (firePoint == null || Camera.main == null) return;

        Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector2 dir = (mousePos - (Vector2)firePoint.position).normalized;

        Gizmos.color = Color.cyan;
        Gizmos.DrawLine(firePoint.position, firePoint.position + (Vector3)dir * maxDistance);
    }
}
