using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SpriteRenderer), typeof(BoxCollider2D))]
public class PressureButton : MonoBehaviour
{
    [Header("Colores del botón")]
    public Color inactiveColor = Color.gray;
    public Color activeColor = Color.green;

    [Header("Plataformas conectadas")]
    [Tooltip("Arrastra aquí las plataformas que deben levantarse al presionar el botón.")]
    public List<RisingPlatform> connectedPlatforms = new List<RisingPlatform>();

    [Header("Configuración del botón")]
    [Tooltip("Distancia que baja el botón visualmente al ser presionado.")]
    public float pressDepth = 0.08f;
    [Tooltip("Velocidad con la que el botón baja/sube.")]
    public float pressSpeed = 6f;

    [HideInInspector] public bool isActive = false;

    private SpriteRenderer sr;
    private Vector3 initialPosition;
    private int cubesOnButton = 0;

    void Start()
    {
        sr = GetComponent<SpriteRenderer>();
        sr.color = inactiveColor;
        GetComponent<BoxCollider2D>().isTrigger = true;
        initialPosition = transform.position;
    }

    void Update()
    {
        // Movimiento visual del botón (baja o sube según estado)
        Vector3 targetPos = isActive
            ? initialPosition - Vector3.up * pressDepth
            : initialPosition;

        transform.position = Vector3.Lerp(transform.position, targetPos, Time.deltaTime * pressSpeed);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Cube"))
        {
            cubesOnButton++;
            if (!isActive)
                ActivateButton();
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Cube"))
        {
            cubesOnButton--;
            if (cubesOnButton <= 0)
                DeactivateButton();
        }
    }

    void ActivateButton()
    {
        isActive = true;
        sr.color = activeColor;

        // 🔹 Activa las plataformas conectadas
        foreach (var platform in connectedPlatforms)
        {
            if (platform != null)
                platform.Raise();
        }
    }

    void DeactivateButton()
    {
        isActive = false;
        sr.color = inactiveColor;

        // 🔹 Desactiva las plataformas conectadas
        foreach (var platform in connectedPlatforms)
        {
            if (platform != null)
                platform.Lower();
        }
    }
}
