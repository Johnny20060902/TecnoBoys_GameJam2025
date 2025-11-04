using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SpriteRenderer), typeof(BoxCollider2D))]
public class PressureButtonPortal : MonoBehaviour
{
    [Header("Colores del botón")]
    public Color inactiveColor = Color.gray;
    public Color activeColor = Color.red;

    [Header("Configuración del botón")]
    [Tooltip("Distancia que baja el botón visualmente al ser presionado.")]
    public float pressDepth = 0.08f;
    [Tooltip("Velocidad con la que el botón baja/sube.")]
    public float pressSpeed = 6f;

    [Header("Opciones adicionales")]
    [Tooltip("Si es true, los portales se desactivan (SetActive(false)). Si es false, se destruyen.")]
    public bool deactivatePortals = true;

    [Header("Grupo que controla este botón")]
    [Tooltip("Solo los portales con el mismo GroupID serán afectados.")]
    public string groupID = "A";

    [Header("Puerta opcional que controla este botón")]
    [Tooltip("Si se asigna, esta puerta desaparecerá mientras el botón esté presionado.")]
    public GameObject controlledDoor;
    public bool hideDoorWhenPressed = true;

    private SpriteRenderer sr;
    private Vector3 initialPosition;
    private int objectsOnButton = 0;
    private bool isActive = false;

    private List<GameObject> controlledPortals = new List<GameObject>();

    void Start()
    {
        sr = GetComponent<SpriteRenderer>();
        sr.color = inactiveColor;
        GetComponent<BoxCollider2D>().isTrigger = true;
        initialPosition = transform.position;

        // 🔹 Buscar solo portales tipo PortalOnlyCube del mismo grupo
        foreach (var p in FindObjectsOfType<PortalOnlyCube>())
        {
            if (p.groupID == groupID)
                controlledPortals.Add(p.gameObject);
        }
    }

    void Update()
    {
        Vector3 targetPos = isActive ? initialPosition - Vector3.up * pressDepth : initialPosition;
        transform.position = Vector3.Lerp(transform.position, targetPos, Time.deltaTime * pressSpeed);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Cube") || other.CompareTag("Player"))
        {
            objectsOnButton++;
            if (!isActive)
                ActivateButton();
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Cube") || other.CompareTag("Player"))
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

        // 🔹 Desactivar los portales del grupo correspondiente
        foreach (var p in controlledPortals)
        {
            if (p != null)
            {
                if (deactivatePortals)
                    p.SetActive(false);
                else
                    Destroy(p);
            }
        }

        // 🔹 Desactivar la puerta mientras está presionado
        if (controlledDoor != null && hideDoorWhenPressed)
            controlledDoor.SetActive(false);
    }

    void DeactivateButton()
    {
        isActive = false;
        sr.color = inactiveColor;

        // 🔹 Reactivar portales
        foreach (var p in controlledPortals)
        {
            if (p != null && deactivatePortals)
                p.SetActive(true);
        }

        // 🔹 Reactivar la puerta al soltar el botón
        if (controlledDoor != null && hideDoorWhenPressed)
            controlledDoor.SetActive(true);
    }
}
