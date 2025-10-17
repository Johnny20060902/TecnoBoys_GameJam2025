using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Player : MonoBehaviour
{
    public float moveSpeed = 5f;
    public Rigidbody2D rb;

    public float timer = 0;
    public float timeBtwShoot = 1f;
    public bool canShoot = true;
    public Transform FirePoint;
    public GameObject bulletPrefab;

    private Vector2 moveInput;
    private Vector2 lastMoveDir = Vector2.down;

    private bool canMove = true;

    private bool styleMoveY;

    // Start is called before the first frame update
    void Start()
    {
        string scene = UnityEngine.SceneManagement.SceneManager.GetActiveScene().name;
        if (scene == "Raul_Introduction")
        {
            styleMoveY = true;
        }
        else if (scene == "Raul_SecondWorldLevel1")
        {
            styleMoveY = false;
        }
        else if (scene == "Raul_SecondWorldLevel3")
        {
            styleMoveY = true;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (!canMove) return;
        Move();
        CheckIfCanShoot();
        Shoot();

    }

    public void EnableMovement(bool enable)
    {
        canMove = enable;
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

    void CheckIfCanShoot()
    {
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
}
