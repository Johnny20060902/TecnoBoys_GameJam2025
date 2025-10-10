using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Boss : MonoBehaviour
{
    [Header("Movimiento")]
    public Transform player;
    public float moveSpeed = 5f;
    public float minY = -4f;
    public float maxY = 4f;
    public float minX = -7f;
    public float maxX = 7f;
    public float changeTime = 1f;
    public float teleportInterval = 5f;

    private float moveTimer = 0f;
    private float teleportTimer = 0f;
    private float targetOffsetY = 0f;

    [Header("Disparo")]
    public GameObject bulletBoss;
    public GameObject bulletBossPortal;
    public Transform positionShoot;
    public float timeBtwShoot = 1f;
    private float shootTimer = 0f;

    [Header("Fases")]
    public float phaseTimer = 0f;

    void Update()
    {
        HandleMovement();
        HandleTeleport();
        HandleShooting();
        HandlePhaseActions();
    }

    #region Movimiento y Teletransporte
    void HandleMovement()
    {
        moveTimer += Time.deltaTime;

        if (moveTimer >= changeTime)
        {

            targetOffsetY = Random.Range(-1f, 1f);
            moveTimer = 0f;
        }

        float targetY = player.position.y + targetOffsetY;
        Vector3 pos = transform.position;
        pos.y = Mathf.MoveTowards(pos.y, targetY, moveSpeed * Time.deltaTime);
        pos.y = Mathf.Clamp(pos.y, minY, maxY);
        transform.position = pos;
    }

    void HandleTeleport()
    {
        teleportTimer += Time.deltaTime;

        if (teleportTimer >= teleportInterval)
        {
            Teleport();
            teleportTimer = 0f;
        }
    }

    void Teleport()
    {
        float randomX = Random.Range(minX, maxX);
        float randomY = Random.Range(minY, maxY);
        transform.position = new Vector3(randomX, randomY, transform.position.z);
    }
    #endregion

    #region Disparo
    void HandleShooting()
    {
        shootTimer += Time.deltaTime;

        if (shootTimer >= timeBtwShoot)
        {
            Shoot();
            shootTimer = 0f;
        }
    }

    void Shoot()
    {
        if (bulletBoss == null || positionShoot == null || player == null)
            return;

        Vector2 direction = ((Vector2)player.position - (Vector2)positionShoot.position).normalized;

        GameObject bullet = Instantiate(bulletBoss, positionShoot.position, Quaternion.identity);
        Rigidbody2D rb = bullet.GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            rb.velocity = direction * 7f;
        }
    }
    #endregion

    #region Fases y acciones especiales
    void HandlePhaseActions()
    {
        phaseTimer += Time.deltaTime;

        if (SceneManager.GetActiveScene().name == "Raul_Introduction" && phaseTimer >= 20f)
        {
            ShootPortal();
            phaseTimer = 0f;
        }
    }

    void ShootPortal()
    {
        if (bulletBossPortal == null || positionShoot == null) return;

        GameObject portal = Instantiate(bulletBossPortal, positionShoot.position, Quaternion.identity);

        ScreenFader fader = FindObjectOfType<ScreenFader>();
        if (fader != null)
        {
            Portal portalScript = portal.GetComponent<Portal>();
            if (portalScript != null)
            {
                portalScript.screenFader = fader;
            }
        }
    }
    #endregion
}
