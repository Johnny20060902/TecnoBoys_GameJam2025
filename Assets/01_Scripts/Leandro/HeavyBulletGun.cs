using System.Collections;
using UnityEngine;

public class HeavyBulletGun : MonoBehaviour
{
    [Header("Referencias")]
    public Transform firePoint;
    public GameObject bulletPrefab;
    public ParticleSystem muzzleFlash;
    public AudioClip shootSound;
    public AudioClip reloadSound;
    private AudioSource audioSource;

    [Header("Balística")]
    public float bulletSpeed = 24f;          // velocidad inicial
    public float bulletGravity = 2.5f;       // gravedad realista
    public float spreadDegrees = 3.5f;       // dispersión natural
    public float damage = 12f;               // daño base

    [Header("Disparo y recarga")]
    public int magazineSize = 6;             // capacidad del cargador
    public float reloadTime = 1.4f;          // tiempo de recarga
    public float fireRate = 3f;              // balas por segundo
    public bool semiAutomatic = true;        // requiere clics individuales

    [Header("Recoil")]
    public float recoilForce = 1f;           // retroceso físico
    public float recoilKickAngle = 3f;       // retroceso visual (rotación leve)
    private Quaternion baseRotation;

    private Camera cam;
    private Rigidbody2D playerRb;
    private float nextFireTime;
    private int currentAmmo;
    private bool reloading;

    void Awake()
    {
        cam = Camera.main;
        playerRb = GetComponentInParent<Rigidbody2D>();
        audioSource = GetComponent<AudioSource>();
        baseRotation = transform.localRotation;
        currentAmmo = magazineSize;
    }

    void Update()
    {
        if (reloading) return;
        AimToMouse();

        if (semiAutomatic ? Input.GetMouseButtonDown(0) : Input.GetMouseButton(0))
            TryShoot();

        if (Input.GetKeyDown(KeyCode.R) && currentAmmo < magazineSize)
            StartCoroutine(Reload());
    }

    // ==============================================
    // 🔫 DISPARO
    // ==============================================
    void TryShoot()
    {
        if (Time.time < nextFireTime) return;
        if (currentAmmo <= 0)
        {
            StartCoroutine(Reload());
            return;
        }

        currentAmmo--;
        nextFireTime = Time.time + 1f / fireRate;

        float spread = Random.Range(-spreadDegrees, spreadDegrees);
        float angle = firePoint.eulerAngles.z + spread;
        Vector2 dir = new Vector2(Mathf.Cos(angle * Mathf.Deg2Rad), Mathf.Sin(angle * Mathf.Deg2Rad));

        // Instanciar bala física
        GameObject b = Instantiate(bulletPrefab, firePoint.position, Quaternion.Euler(0, 0, angle));
        Rigidbody2D rb = b.GetComponent<Rigidbody2D>();
        if (rb)
        {
            rb.gravityScale = bulletGravity / 9.81f; // gravedad realista en proporción
            rb.velocity = dir * bulletSpeed;
        }

        BulletPhysics bp = b.GetComponent<BulletPhysics>();
        if (bp)
        {
            bp.damage = damage;
            bp.lifeTime = 3f;
        }

        // 🔥 efectos visuales y sonoros
        if (muzzleFlash) muzzleFlash.Play();
        if (audioSource && shootSound) audioSource.PlayOneShot(shootSound, 0.9f);

        // 💥 recoil físico y visual
        if (playerRb) playerRb.AddForce(-dir * recoilForce, ForceMode2D.Impulse);
        StartCoroutine(VisualRecoilKick());

        Destroy(b, 4f);
    }

    IEnumerator VisualRecoilKick()
    {
        Quaternion start = firePoint.localRotation;
        firePoint.localRotation = Quaternion.Euler(0, 0, firePoint.localEulerAngles.z + Random.Range(-recoilKickAngle, recoilKickAngle));
        yield return new WaitForSeconds(0.05f);
        firePoint.localRotation = start;
    }

    IEnumerator Reload()
    {
        reloading = true;
        if (audioSource && reloadSound) audioSource.PlayOneShot(reloadSound, 1f);
        yield return new WaitForSeconds(reloadTime);
        currentAmmo = magazineSize;
        reloading = false;
    }

    void AimToMouse()
    {
        if (!firePoint || cam == null) return;
        Vector3 mouse = cam.ScreenToWorldPoint(Input.mousePosition);
        Vector2 dir = (mouse - firePoint.position).normalized;
        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        firePoint.rotation = Quaternion.Euler(0f, 0f, angle);
    }

    void OnGUI()
    {
        // UI simple para debug
        GUI.Label(new Rect(10, 10, 200, 20), reloading ? "🔄 Reloading..." : $"Ammo: {currentAmmo}/{magazineSize}");
    }

    void OnDrawGizmosSelected()
    {
        if (!firePoint) return;
        Gizmos.color = Color.yellow;
        Gizmos.DrawLine(firePoint.position, firePoint.position + firePoint.right * 1.5f);
    }
}
