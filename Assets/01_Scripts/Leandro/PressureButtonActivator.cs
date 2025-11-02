using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SpriteRenderer), typeof(BoxCollider2D))]
public class PressureButtonActivator : MonoBehaviour
{
    [Header("Colores del botón")]
    public Color inactiveColor = Color.gray;
    public Color activeColor = Color.green;

    [Header("Configuración del botón")]
    [Tooltip("Distancia que baja el botón visualmente al ser presionado.")]
    public float pressDepth = 0.08f;
    [Tooltip("Velocidad con la que el botón baja/sube.")]
    public float pressSpeed = 6f;

    [Header("Objetos controlados")]
    [Tooltip("Portal que se activará al presionar el botón.")]
    public GameObject targetPortal;

    [Tooltip("Prefab del cubo que se generará al presionar el botón.")]
    public GameObject cubePrefab;

    [Tooltip("Punto donde aparecerá el cubo generado.")]
    public Transform cubeSpawnPoint;

    [Tooltip("Si es true, el portal se desactiva al soltar el botón.")]
    public bool deactivatePortalOnRelease = true;

    private SpriteRenderer sr;
    private Vector3 initialPosition;
    private bool isActive = false;
    private bool cubeExists = false; // 🔹 controla si ya hay un cubo en escena
    private int objectsOnButton = 0;

    void Start()
    {
        sr = GetComponent<SpriteRenderer>();
        sr.color = inactiveColor;
        GetComponent<BoxCollider2D>().isTrigger = true;
        initialPosition = transform.position;

        // 🔹 Asegurarse que el portal empiece desactivado
        if (targetPortal != null)
            targetPortal.SetActive(false);
    }

    void Update()
    {
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

        // 🔹 Activar el portal
        if (targetPortal != null)
            targetPortal.SetActive(true);

        // 🔹 Generar cubo solo si no hay uno actualmente
        if (!cubeExists && cubePrefab != null && cubeSpawnPoint != null)
        {
            GameObject newCube = Instantiate(cubePrefab, cubeSpawnPoint.position, cubeSpawnPoint.rotation);
            cubeExists = true;

            // ✅ Vincular automáticamente el botón al cubo generado
            CubeVanishOnPortal vanishScript = newCube.GetComponent<CubeVanishOnPortal>();
            if (vanishScript != null)
                vanishScript.linkedButton = this;
        }
    }

    void DeactivateButton()
    {
        isActive = false;
        sr.color = inactiveColor;

        // 🔹 Desactivar el portal al soltar (si se desea)
        if (targetPortal != null && deactivatePortalOnRelease)
            targetPortal.SetActive(false);
    }

    // 🔸 Llamado por el portal cuando el cubo desaparece
    public void OnCubeDestroyed()
    {
        cubeExists = false;
    }
}
