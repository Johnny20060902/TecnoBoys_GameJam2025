using UnityEngine;

public class LaserButtonDeactivator : MonoBehaviour
{
    [Header("Configuración del botón")]
    [Tooltip("Referencia a la plataforma que se desactivará al recibir el láser")]
    public GameObject plataformaObjetivo;

    [Tooltip("Tiempo continuo de láser para APAGAR la plataforma")]
    public float tiempoDesactivacion = 0.1f;

    [Tooltip("Tiempo sin láser para VOLVER A ENCENDER la plataforma")]
    public float tiempoReactivacion = 0.5f;

    private bool recibiendoLaser = false;
    private float tiempoSinLaser = 0f;
    private float tiempoConLaser = 0f;

    // 🔴 El láser llama esto cada frame que golpea el botón
    public void RecibirLaser()
    {
        recibiendoLaser = true;
    }

    void Update()
    {
        if (recibiendoLaser)
        {
            tiempoConLaser += Time.deltaTime;
            tiempoSinLaser = 0f;

            // 🔹 Desactiva cuando se cumple el tiempo requerido
            if (tiempoConLaser >= tiempoDesactivacion && plataformaObjetivo != null && plataformaObjetivo.activeSelf)
                plataformaObjetivo.SetActive(false);
        }
        else
        {
            tiempoConLaser = 0f;
            tiempoSinLaser += Time.deltaTime;

            // 🔹 Reactiva cuando pasa el tiempo sin láser
            if (tiempoSinLaser >= tiempoReactivacion && plataformaObjetivo != null && !plataformaObjetivo.activeSelf)
                plataformaObjetivo.SetActive(true);
        }

        // Reset del flag (el láser lo volverá a activar en el próximo frame si sigue tocando)
        recibiendoLaser = false;
    }
}
