using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NpcBasicControllerW2 : MonoBehaviour
{
    public float moveSpeed = 5f;     
    public bool moveRight = false;
    public Transform stopPoint;

    private Rigidbody2D rb;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        if (moveRight)
        {
            rb.velocity = new Vector2(moveSpeed, rb.velocity.y);

            if (stopPoint != null && transform.position.x >= stopPoint.position.x)
            {
                StopWalking();
            }
        }
        else
        {
            rb.velocity = Vector2.zero;
        }
    }

    public void StartWalking()
    {
        moveRight = true;
    }

    public void StopWalking()
    {
        moveRight = false;
    }
}
