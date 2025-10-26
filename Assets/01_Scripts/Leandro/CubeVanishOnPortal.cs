using UnityEngine;

public class CubeVanishOnPortal : MonoBehaviour
{
    [Tooltip("Si el cubo está vinculado a un botón generador, se asigna automáticamente al generarse.")]
    public PressureButtonActivator linkedButton;

    private void OnTriggerEnter2D(Collider2D other)
    {
        // 🔹 Solo destruye el cubo si entra a un portal del tipo PortalOnlyCube
        if (other.GetComponent<PortalOnlyCube>() != null)
        {
            // Destruir este cubo
            Destroy(gameObject);

            // Avisar al botón generador (si existe)
            if (linkedButton != null)
                linkedButton.OnCubeDestroyed();
        }
    }
}
