using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public class LaserBeamToggle : MonoBehaviour
{
    [Header("Configuración del láser")]
    public int maxBounces = 10;
    public float maxDistance = 50f;
    public LayerMask reflectMask;
    public Color laserColor = Color.red;
    public float lineWidth = 0.05f;

    [Header("Estado inicial")]
    public bool isActive = false; // 🔹 comienza apagado

    private LineRenderer entradaRenderer;
    private LineRenderer salidaRenderer;

    private readonly List<Vector3> pointsEntrada = new();
    private readonly List<Vector3> pointsSalida = new();

    void Awake()
    {
        // 🔹 Primer LineRenderer (entrada)
        entradaRenderer = GetComponent<LineRenderer>();
        entradaRenderer.startWidth = lineWidth;
        entradaRenderer.endWidth = lineWidth;
        entradaRenderer.material = new Material(Shader.Find("Sprites/Default"));
        entradaRenderer.startColor = laserColor;
        entradaRenderer.endColor = laserColor;

        // 🔹 Segundo LineRenderer (salida)
        GameObject salidaObj = new GameObject("LaserSalida");
        salidaObj.transform.SetParent(transform);
        salidaRenderer = salidaObj.AddComponent<LineRenderer>();
        salidaRenderer.startWidth = lineWidth;
        salidaRenderer.endWidth = lineWidth;
        salidaRenderer.material = new Material(Shader.Find("Sprites/Default"));
        salidaRenderer.startColor = laserColor;
        salidaRenderer.endColor = laserColor;

        SetLaserVisible(isActive);
    }

    void Update()
    {
        if (isActive)
            SimulateLaser(transform.position, transform.right);
        else
            ClearLaser();
    }

    // =======================================================
    // 🔹 Encender o apagar el láser desde otro script
    // =======================================================
    public void SetActive(bool active)
    {
        isActive = active;
        SetLaserVisible(active);
    }

    private void SetLaserVisible(bool visible)
    {
        entradaRenderer.enabled = visible;
        salidaRenderer.enabled = visible;
    }

    private void ClearLaser()
    {
        entradaRenderer.positionCount = 0;
        salidaRenderer.positionCount = 0;
    }

    // =======================================================
    // 🔹 Simulación del rayo (con soporte para activadores y desactivadores)
    // =======================================================
    void SimulateLaser(Vector3 origin, Vector2 direction)
    {
        pointsEntrada.Clear();
        pointsSalida.Clear();

        Vector2 dir = direction.normalized;
        Vector3 currentPos = origin;
        bool teleported = false;

        pointsEntrada.Add(origin);

        for (int i = 0; i < maxBounces; i++)
        {
            RaycastHit2D hit = Physics2D.Raycast(currentPos, dir, maxDistance, reflectMask);

            if (!hit.collider)
            {
                pointsEntrada.Add(currentPos + (Vector3)dir * maxDistance);
                break;
            }

            pointsEntrada.Add(hit.point);

            // 🔹 Rebote
            if (hit.collider.CompareTag("Mirror") || hit.collider.CompareTag("Glass"))
            {
                dir = Vector2.Reflect(dir, hit.normal);
                currentPos = hit.point + hit.normal * 0.02f;
                continue;
            }

            // 🔹 Portal azul → teletransporte
            Portal2D portal = hit.collider.GetComponent<Portal2D>();
            if (!teleported && portal != null && portal.isBlue && portal.linkedPortal != null)
            {
                teleported = true;
                Portal2D salida = portal.linkedPortal;
                Vector2 exitDir = salida.transform.right.normalized;
                Vector3 exitPos = salida.transform.position + (Vector3)(exitDir * salida.exitOffset);

                SimulateLaserFromPortal(exitPos, exitDir);
                break;
            }

            // 🔹 Si golpea un botón láser activador
            LaserButton boton = hit.collider.GetComponent<LaserButton>();
            if (boton != null)
            {
                boton.RecibirLaser();
                break;
            }

            // 🔹 Si golpea un botón láser desactivador
            LaserButtonDeactivator botonDes = hit.collider.GetComponent<LaserButtonDeactivator>();
            if (botonDes != null)
            {
                botonDes.RecibirLaser();
                break;
            }

            // 🔹 Si golpea una superficie sólida
            if (hit.collider.CompareTag("PortalOrange") || hit.collider.CompareTag("Ground") || !hit.collider.isTrigger)
                break;
        }

        entradaRenderer.positionCount = pointsEntrada.Count;
        entradaRenderer.SetPositions(pointsEntrada.ToArray());

        salidaRenderer.positionCount = pointsSalida.Count;
        salidaRenderer.SetPositions(pointsSalida.ToArray());
    }

    void SimulateLaserFromPortal(Vector3 origin, Vector2 direction)
    {
        Vector2 dir = direction.normalized;
        Vector3 currentPos = origin;

        pointsSalida.Add(origin);

        for (int i = 0; i < maxBounces; i++)
        {
            RaycastHit2D hit = Physics2D.Raycast(currentPos, dir, maxDistance, reflectMask);

            if (!hit.collider)
            {
                pointsSalida.Add(currentPos + (Vector3)dir * maxDistance);
                break;
            }

            pointsSalida.Add(hit.point);

            // 🔹 Rebote
            if (hit.collider.CompareTag("Mirror") || hit.collider.CompareTag("Glass"))
            {
                dir = Vector2.Reflect(dir, hit.normal);
                currentPos = hit.point + hit.normal * 0.02f;
                continue;
            }

            // 🔹 Botón activador
            LaserButton boton = hit.collider.GetComponent<LaserButton>();
            if (boton != null)
            {
                boton.RecibirLaser();
                break;
            }

            // 🔹 Botón desactivador
            LaserButtonDeactivator botonDes = hit.collider.GetComponent<LaserButtonDeactivator>();
            if (botonDes != null)
            {
                botonDes.RecibirLaser();
                break;
            }

            // 🔹 Superficie sólida
            if (hit.collider.CompareTag("PortalOrange") || hit.collider.CompareTag("Ground") || !hit.collider.isTrigger)
                break;
        }
    }
}
