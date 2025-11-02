using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SpriteRenderer), typeof(BoxCollider2D))]
public class LaserActivatorButton : MonoBehaviour
{
    [Header("Colores del botón")]
    public Color inactiveColor = Color.gray;
    public Color activeColor = Color.red;

    [Header("Láser controlado")]
    public LaserBeamToggle controlledLaser;

    [Header("Movimiento visual")]
    public float pressDepth = 0.08f;
    public float pressSpeed = 6f;

    private SpriteRenderer sr;
    private Vector3 initialPos;
    private int objectsOnButton = 0;
    private bool isActive = false;

    void Start()
    {
        sr = GetComponent<SpriteRenderer>();
        sr.color = inactiveColor;

        GetComponent<BoxCollider2D>().isTrigger = true;
        initialPos = transform.position;
    }

    void Update()
    {
        Vector3 targetPos = isActive ? initialPos - Vector3.up * pressDepth : initialPos;
        transform.position = Vector3.Lerp(transform.position, targetPos, Time.deltaTime * pressSpeed);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player") || other.CompareTag("Cube"))
        {
            objectsOnButton++;
            if (!isActive)
                ActivateButton();
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player") || other.CompareTag("Cube"))
        {
            objectsOnButton--;
            if (objectsOnButton <= 0)
                DeactivateButton();
        }
    }

    void ActivateButton()
    {
        isActive = true;
        sr.color = activeColor;

        if (controlledLaser != null)
            controlledLaser.SetActive(true);
    }

    void DeactivateButton()
    {
        isActive = false;
        sr.color = inactiveColor;

        if (controlledLaser != null)
            controlledLaser.SetActive(false);
    }
}
