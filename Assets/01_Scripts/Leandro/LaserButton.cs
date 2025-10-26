using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LaserButton : MonoBehaviour
{
    [Header("Configuración del botón")]
    [Tooltip("Referencia a la plataforma que se activará al recibir el láser")]
    public GameObject plataformaObjetivo;

    [Tooltip("Tiempo que debe estar recibiendo el láser para activarse (segundos)")]
    public float tiempoActivacion = 0.1f;

    [Tooltip("Tiempo que tarda en apagarse si el láser deja de tocarlo")]
    public float tiempoDesactivacion = 0.5f;

    private float timer = 0f;
    private bool recibiendoLaser = false;

    // Llamado externamente por el rayo
    public void RecibirLaser()
    {
        recibiendoLaser = true;
        timer = tiempoActivacion;
    }

    void Update()
    {
        if (recibiendoLaser)
        {
            // 🔹 Activar la plataforma si no está ya activa
            if (plataformaObjetivo != null && !plataformaObjetivo.activeSelf)
                plataformaObjetivo.SetActive(true);

            recibiendoLaser = false;
        }
        else
        {
            // 🔹 Cuenta regresiva cuando el láser deja de tocar el botón
            if (timer > 0)
            {
                timer -= Time.deltaTime;
                if (timer <= 0f && plataformaObjetivo != null)
                {
                    plataformaObjetivo.SetActive(false);
                }
            }
        }
    }
}
