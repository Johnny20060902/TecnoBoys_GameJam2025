using UnityEngine;

public class CameraFollow2D : MonoBehaviour
{
    public Transform target;
    public float smooth = 5f;
    public Vector2 minBounds = new Vector2(-100, -10);
    public Vector2 maxBounds = new Vector2( 100,  10);
    public float zOffset = -10f;

    [Header("Vertical")]
    public bool lockY = true;   // mantener Y fija
    public float fixedY;        // se toma del Y inicial de la cámara

    void Start()
    {
        // Mantener la altura actual de la cámara
        fixedY = transform.position.y;
    }

    void LateUpdate()
    {
        if (target == null) return;

        // Seguir solo en X; Y queda fija (o limitada por bounds si querés)
        float camY = lockY ? fixedY : Mathf.Clamp(target.position.y, minBounds.y, maxBounds.y);

        Vector3 wanted = new Vector3(
            Mathf.Clamp(target.position.x, minBounds.x, maxBounds.x),
            Mathf.Clamp(camY, minBounds.y, maxBounds.y),
            zOffset
        );

        transform.position = Vector3.Lerp(transform.position, wanted, smooth * Time.deltaTime);
    }
}
