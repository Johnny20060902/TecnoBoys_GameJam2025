using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public enum WeaponTypeW2
{
    Sword,
    Gun,
    AlienGun
}

public class Player : MonoBehaviour, ITakeDamage
{
    public float life = 100f;
    public float moveSpeed = 5f;
    public Rigidbody2D rb;

    public WeaponTypeW2 currentWeapon = WeaponTypeW2.Gun;

    public float timer = 0;
    public float timeBtwShoot = 1f;
    public bool canShoot = false;
    public bool canShootWorld;
    public Transform FirePoint;
    public GameObject bulletPrefab;
    public GameObject AlienbulletPrefab;

    private Vector2 moveInput;
    private Vector2 lastMoveDir = Vector2.down;

    private bool canMove = true;

    private bool styleMoveY;

    public bool canJump = false;
    public float forceJump = 5f;
    int numberJumps;


    public float swordAttackRange = 1f;
    public int swordDamage = 1;
    public LayerMask enemyLayers;
    public Transform attackPoint;
    public GameObject swordObject;

    public GameObject alienGunObject;

    int damage = 1;
    private bool hasAlienGun = false;
    // Start is called before the first frame update
    void Start()
    {
        string scene = UnityEngine.SceneManagement.SceneManager.GetActiveScene().name;
        if (scene == "Raul_Introduction")
        {
            styleMoveY = true;
            canShootWorld = true;
            canJump = false;
        }
        else if (scene == "Raul_SecondWorldLevel1")
        {
            styleMoveY = false;
            canShootWorld = false;
            canJump = true;
        }
        else if (scene == "Raul_SecondWorldLevel2")
        {
            styleMoveY = false;
            canShootWorld = false;
            canJump = true;
        }
        else if (scene == "Raul_SecondWorldLevel3")
        {
            styleMoveY = true;
            canShootWorld = true;
            canJump = false;
        }
        else if (scene == "Raul_SecondWorldLevel4")
        {
            styleMoveY = true;
            canShootWorld = true;
            canJump = false;
        }
        else if (scene == "Raul_SecondWorldLevel5")
        {
            styleMoveY = true;
            canShootWorld = true;
            canJump = false;
            hasAlienGun= true;
        }
        UpdateWeaponVisibility();
    }

    // Update is called once per frame
    void Update()
    {
        if (!canMove) return;

        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            currentWeapon = WeaponTypeW2.Sword;
            UpdateWeaponVisibility();
        }

        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            currentWeapon = WeaponTypeW2.Gun;
            UpdateWeaponVisibility();
        }
        if (Input.GetKeyDown(KeyCode.Alpha3) && hasAlienGun)
        {
            currentWeapon = WeaponTypeW2.AlienGun;
            UpdateWeaponVisibility();
        }

        Move();
        RotateSword(); 
        CheckIfCanShoot();
        Attack();
        Jump();
    }
    public void UnlockThirdWeapon()
    {
        hasAlienGun = true;
    }

    void UpdateWeaponVisibility()
    {
        if (swordObject != null)
            swordObject.SetActive(currentWeapon == WeaponTypeW2.Sword);

        if (alienGunObject != null)
            alienGunObject.SetActive(currentWeapon == WeaponTypeW2.AlienGun);
    }



    void RotateSword()
    {
        if (currentWeapon != WeaponTypeW2.Sword) return;

        if (lastMoveDir == Vector2.zero) return;

        float angle = Mathf.Atan2(lastMoveDir.y, lastMoveDir.x) * Mathf.Rad2Deg;
        FirePoint.parent.rotation = Quaternion.Euler(0, 0, angle - 90f);
    }

    void Attack()
    {
        if (currentWeapon == WeaponTypeW2.Gun)
        {
            Shoot();
        }
        else if (currentWeapon == WeaponTypeW2.Sword)
        {
            SwordAttack();
        }
        if (currentWeapon == WeaponTypeW2.AlienGun)
        {
            ShootAlienAcid();
        }
    }

    void Jump()
    {
        if (Mathf.Abs(rb.velocity.y) < 0.01f)
        {
            numberJumps = 1;
        }

        if ( Input.GetKey(KeyCode.Space) && canJump && numberJumps > 0)
        {
            rb.velocity = new Vector2(rb.velocity.x, forceJump);
            numberJumps--;
        }
    }

    public void EnableMovement(bool enable)
    {
        canMove = enable;
        if (!enable)
        {
            rb.velocity = Vector2.zero;
        }
    }

    void Move()
    {
        float x = 0f;
        float y = 0f;

        if (styleMoveY)
        {
            if (Input.GetKey(KeyCode.W)) y += 1f;
            if (Input.GetKey(KeyCode.S)) y -= 1f;
            if (Input.GetKey(KeyCode.A)) x -= 1f;
            if (Input.GetKey(KeyCode.D)) x += 1f;

            moveInput = new Vector2(x, y).normalized;
            rb.velocity = moveInput * moveSpeed;
        }
        else
        {
            if (Input.GetKey(KeyCode.A)) x -= 1f;
            if (Input.GetKey(KeyCode.D)) x += 1f;

            rb.velocity = new Vector2(x * moveSpeed, rb.velocity.y);
        }

        if (moveInput != Vector2.zero)
            lastMoveDir = moveInput;
    }


    void Shoot()
    {
        if (!canShoot) return;
        if (Input.GetMouseButtonDown(0))
        {
            Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            mousePos.z = 0f;
            Vector2 attackDir = (mousePos - FirePoint.position).normalized;

            if (bulletPrefab != null && FirePoint != null)
            {
                GameObject attack = Instantiate(bulletPrefab, FirePoint.position, Quaternion.identity);
                attack.transform.up = attackDir;
            }

            canShoot = false;
            timer = 0f;
        }
    }

    void ShootAlienAcid()
    {
        if (!canShoot) return;

        if (Input.GetMouseButtonDown(0))
        {
            Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            mousePos.z = 0f;
            Vector2 attackDir = (mousePos - FirePoint.position).normalized;

            if (AlienbulletPrefab != null && FirePoint != null)
            {
                float spreadAngle = 15f;
                int bulletCount = 3;

                for (int i = 0; i < bulletCount; i++)
                {
                    float angle = (i - 1) * spreadAngle;

                    Vector2 rotatedDir = Quaternion.Euler(0, 0, angle) * attackDir;

                    GameObject bullet = Instantiate(AlienbulletPrefab, FirePoint.position, Quaternion.identity);
                    bullet.transform.up = rotatedDir;
                }
            }

            canShoot = false;
            timer = 0f;
        }
    }

    void CheckIfCanShoot()
    {
        if (!canShootWorld) return;
        if (!canShoot)
        {
            timer += Time.deltaTime;
            if (timer >= timeBtwShoot)
            {
                timer = 0f;
                canShoot = true;
            }
        }
    }

    void SwordAttack()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Debug.Log("Ataque con espada");
            Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(attackPoint.position, swordAttackRange, enemyLayers);

            foreach (Collider2D enemy in hitEnemies)
            {
                ITakeDamage damageable = enemy.GetComponent<ITakeDamage>();
                if (damageable != null)
                {
                    damageable.TakeDamage(damage);
                }
            }
        }
    }

    void OnDrawGizmosSelected()
    {
        if (attackPoint == null) return;
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(attackPoint.position, swordAttackRange);
    }

    public void TakeDamage(float dmg)
    {
        life -= dmg;
        if (life <= 0)
        {
            Destroy(gameObject);
        }
    }
}
