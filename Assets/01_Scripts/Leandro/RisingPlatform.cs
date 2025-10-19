using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RisingPlatform : MonoBehaviour
{
    [Header("Movimiento vertical")]
    [Tooltip("Altura que se levanta la plataforma al activarse.")]
    public float riseHeight = 2.5f;

    [Tooltip("Velocidad del movimiento vertical.")]
    public float moveSpeed = 3f;

    private Vector3 startPos;
    private Vector3 targetPos;
    private bool isRising = false;

    void Start()
    {
        startPos = transform.position;
        targetPos = startPos + Vector3.up * riseHeight;
    }

    void Update()
    {
        Vector3 desiredPos = isRising ? targetPos : startPos;
        transform.position = Vector3.Lerp(transform.position, desiredPos, Time.deltaTime * moveSpeed);
    }

    public void Raise()
    {
        isRising = true;
    }

    public void Lower()
    {
        isRising = false;
    }
}






