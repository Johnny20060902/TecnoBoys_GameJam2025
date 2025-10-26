using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PortalCubeVanish : MonoBehaviour
{
    [Header("Portal asignado a este botón")]
    [Tooltip("Referencia al botón que genera el cubo y activa este portal.")]
    public PressureButtonActivator linkedButton;

    private void OnTriggerEnter2D(Collider2D other)
    {
        // Solo afecta a cubos
        if (!other.CompareTag("Cube")) return;

        // 🔹 Desaparece el cubo al entrar
        Destroy(other.gameObject);

        // 🔹 Avisar al botón que puede generar otro
        if (linkedButton != null)
            linkedButton.OnCubeDestroyed();
    }
}
