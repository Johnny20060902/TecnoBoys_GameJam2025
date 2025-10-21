using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoldierAlien : MonoBehaviour, ITakeDamage
{
    public float life = 3;

    public bool isActive = false;
    public float moveSpeed = 3f;
    public float stopDistance = 1.5f;
    public Transform player;

    private Rigidbody2D rb;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        player = GameObject.FindGameObjectWithTag("Player").transform;

        string scene = UnityEngine.SceneManagement.SceneManager.GetActiveScene().name;
        if (scene == "Raul_SecondWorldLevel4")
        {
            isActive = true;
        }
        else
        {
            isActive = false;
        }


    }

    void Update()
    {
        if (!isActive || player == null) return;

        Vector2 direction = (player.position - transform.position);
        float distance = direction.magnitude;

        if (distance > stopDistance)
        {
            direction.Normalize();
            rb.velocity = direction * moveSpeed;
        }
        else
        {
            rb.velocity = Vector2.zero;
        }
    }

    public void EnableAttack(bool enable)
    {
        isActive = enable;
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
