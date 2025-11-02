using System.Collections;
using System.Collections.Generic;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SpriteRenderer), typeof(BoxCollider2D))]
public class PressureButtonPlatformActivator : MonoBehaviour
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
    [Tooltip("Plataforma que se activará con este botón.")]
    public GameObject controlledPlatform;

    private SpriteRenderer sr;
    private Vector3 initialPosition;
    private int objectsOnButton = 0;
    private bool isActive = false;
    private bool alreadyActivated = false;

    void Start()
    {
        sr = GetComponent<SpriteRenderer>();
        sr.color = inactiveColor;

        BoxCollider2D col = GetComponent<BoxCollider2D>();
        col.isTrigger = true;

        initialPosition = transform.position;

        // 🔹 La plataforma empieza desactivada
        if (controlledPlatform != null)
            controlledPlatform.SetActive(false);
    }

    void Update()
    {
        // 🔹 Movimiento visual del botón
        Vector3 targetPos = isActive ? initialPosition - Vector3.up * pressDepth : initialPosition;
        transform.position = Vector3.Lerp(transform.position, targetPos, Time.deltaTime * pressSpeed);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if ((other.CompareTag("Player") || other.CompareTag("Cube")) && !alreadyActivated)
        {
            objectsOnButton++;
            if (!isActive)
                ActivateButton();
        }
    }

    void ActivateButton()
    {
        isActive = true;
        alreadyActivated = true;
        sr.color = activeColor;

        if (controlledPlatform != null)
            controlledPlatform.SetActive(true);
    }
}
