using System.Collections;
using System.Collections.Generic;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SpriteRenderer), typeof(BoxCollider2D))]
public class PressureButtonPlatform : MonoBehaviour
{
    [Header("Colores del botón")]
    public Color inactiveColor = Color.gray;
    public Color activeColor = Color.green;

    [Header("Movimiento visual del botón")]
    [Tooltip("Distancia que baja el botón cuando se presiona.")]
    public float pressDepth = 0.08f;
    [Tooltip("Velocidad del movimiento de bajada/subida.")]
    public float pressSpeed = 6f;

    [Header("Plataforma controlada")]
    [Tooltip("Plataforma que se activa/desactiva con este botón.")]
    public GameObject controlledPlatform;

    private SpriteRenderer sr;
    private Vector3 initialPosition;
    private int objectsOnButton = 0;
    private bool isActive = false;

    void Start()
    {
        sr = GetComponent<SpriteRenderer>();
        sr.color = inactiveColor;

        BoxCollider2D col = GetComponent<BoxCollider2D>();
        col.isTrigger = true;

        initialPosition = transform.position;

        if (controlledPlatform != null)
            controlledPlatform.SetActive(false); // inicia apagada
    }

    void Update()
    {
        // 🔹 Movimiento visual (baja al presionar)
        Vector3 targetPos = isActive ? initialPosition - Vector3.up * pressDepth : initialPosition;
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

        if (controlledPlatform != null)
            controlledPlatform.SetActive(true);
    }

    void DeactivateButton()
    {
        isActive = false;
        sr.color = inactiveColor;

        if (controlledPlatform != null)
            controlledPlatform.SetActive(false);
    }
}
