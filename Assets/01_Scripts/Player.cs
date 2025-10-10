using System.Collections;
using System.Collections.Generic;
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

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        Move();
        CheckIfCanShoot();
        Shoot();

    }

    void Move()
    {
        float x = 0f;
        float y = 0f;

        if (Input.GetKey(KeyCode.W)) y += 1f;
        if (Input.GetKey(KeyCode.S)) y -= 1f;
        if (Input.GetKey(KeyCode.A)) x -= 1f;
        if (Input.GetKey(KeyCode.D)) x += 1f;

        moveInput = new Vector2(x, y).normalized;

        if (rb != null)
            rb.velocity = moveInput * moveSpeed;

        if (moveInput != Vector2.zero)
            lastMoveDir = moveInput;
    }


    void Shoot()
    {
        if (!canShoot) return;

        Vector2 attackDir = Vector2.zero;

        if (Input.GetKeyDown(KeyCode.UpArrow)) attackDir = Vector2.up;
        if (Input.GetKeyDown(KeyCode.DownArrow)) attackDir = Vector2.down;
        if (Input.GetKeyDown(KeyCode.LeftArrow)) attackDir = Vector2.left;
        if (Input.GetKeyDown(KeyCode.RightArrow)) attackDir = Vector2.right;

        if (attackDir != Vector2.zero)
        {
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
