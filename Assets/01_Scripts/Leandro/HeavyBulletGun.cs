using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeavyBulletGun : MonoBehaviour
{
    [Header("Referencias")]
    public Transform firePoint;            
    public GameObject bulletPrefab;        
    public LayerMask aimMask = ~0;         

    [Header("Disparo")]
    public bool autoFire = false;           // Metal Slug no usa fuego automático continuo
    public float fireRate = 2.5f;           // 2.5 balas por segundo (más pausado y contundente)
    public float bulletSpeed = 18f;         // más rápida y con fuerza realista
    public int damage = 15;                 
    public float spreadDegrees = 4f;        // pequeña dispersión para hacerlo natural
    public float recoilForce = 1.2f;        // retroceso leve del jugador

    float fireCooldown;
    Camera cam;
    Rigidbody2D playerRb;
    AudioSource audioSource;

    void Awake()
    {
        cam = Camera.main;
        playerRb = GetComponentInParent<Rigidbody2D>();
        audioSource = GetComponent<AudioSource>();

        if (!firePoint) Debug.LogWarning("⚠️ HeavyBulletGun: Asigna FirePoint.");
        if (!bulletPrefab) Debug.LogWarning("⚠️ HeavyBulletGun: Asigna BulletPrefab.");
    }

    void Update()
    {
        AimToMouse();
        HandleShooting();
    }

    void AimToMouse()
    {
        if (!firePoint || cam == null) return;

        Vector3 mouse = cam.ScreenToWorldPoint(Input.mousePosition);
        mouse.z = firePoint.position.z;

        Vector2 dir = (mouse - firePoint.position).normalized;
        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;

        firePoint.rotation = Quaternion.Euler(0f, 0f, angle);
    }

    void HandleShooting()
    {
        fireCooldown -= Time.deltaTime;

        bool trigger = autoFire ? Input.GetMouseButton(0) : Input.GetMouseButtonDown(0);
        if (!trigger || fireCooldown > 0f || !bulletPrefab || !firePoint) return;

        fireCooldown = 1f / Mathf.Max(0.01f, fireRate);

        // Dispersión leve como en Metal Slug
        float z = firePoint.eulerAngles.z + Random.Range(-spreadDegrees * 0.5f, spreadDegrees * 0.5f);
        Vector2 dir = new Vector2(Mathf.Cos(z * Mathf.Deg2Rad), Mathf.Sin(z * Mathf.Deg2Rad));

        // Instancia de bala
        GameObject b = Instantiate(bulletPrefab, firePoint.position, Quaternion.Euler(0, 0, z));

        Rigidbody2D rb = b.GetComponent<Rigidbody2D>();
        if (rb)
        {
            rb.gravityScale = 0f;
            rb.velocity = dir * bulletSpeed;
        }


        // 💥 Retroceso suave al jugador (tipo Metal Slug)
        if (playerRb != null)
            playerRb.AddForce(-dir * recoilForce, ForceMode2D.Impulse);

        // 💣 Sonido si tiene audio
        if (audioSource != null) audioSource.Play();

        Destroy(b, 3f);
    }

    // Gizmo de depuración
    void OnDrawGizmosSelected()
    {
        if (!firePoint) return;
        Gizmos.color = Color.yellow;
        Gizmos.DrawLine(firePoint.position, firePoint.position + firePoint.right * 1.5f);
    }
}
