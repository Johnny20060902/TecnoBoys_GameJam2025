using UnityEngine;

public class CameraFollowLeo : MonoBehaviour
{
    [Header("🎯 Objetivo a seguir")]
    public Transform target; // Jugador u objeto a seguir

    [Header("🎥 Suavizado del movimiento")]
    [Tooltip("Qué tan rápido la cámara sigue al objetivo (mayor = más rápido)")]
    [Range(0.01f, 20f)] 
    public float smoothSpeed = 5f;

    [Tooltip("Desplazamiento desde el jugador (por ejemplo, un poco más arriba)")]
    public Vector3 offset = new Vector3(0f, 1.5f, -10f);

    private Vector3 velocity = Vector3.zero;

    void Start()
    {
        // Si no se asignó el target manualmente, intenta buscar un objeto con tag "Player"
        if (target == null)
        {
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            if (player != null)
                target = player.transform;
        }

        // Centra la cámara al inicio sobre el jugador
        if (target != null)
            transform.position = target.position + offset;
    }

    void LateUpdate()
    {
        if (target == null) return;

        // 🔹 Posición deseada
        Vector3 desiredPos = target.position + offset;

        // 🔹 Movimiento suave (sin jitter)
        transform.position = Vector3.SmoothDamp(
            transform.position,
            desiredPos,
            ref velocity,
            1f / smoothSpeed
        );
    }
}
