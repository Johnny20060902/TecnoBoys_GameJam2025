using UnityEngine;

public class PressureButtonMirror : MonoBehaviour
{
    [Header("Configuración del botón")]
    public Transform mirrorToRotate;      // el espejo a rotar
    public float rotationAngle = 25f;     // cuánto gira al activarse
    public float rotationSpeed = 3f;      // velocidad de rotación
    public Color pressedColor = Color.green;
    public Color defaultColor = Color.red;

    private SpriteRenderer sr;
    private bool isPressed = false;
    private Quaternion originalRotation;
    private Quaternion targetRotation;
    private int pressCount = 0; // para evitar múltiples activaciones si hay más de un objeto encima

    void Start()
    {
        sr = GetComponent<SpriteRenderer>();
        if (mirrorToRotate != null)
            originalRotation = mirrorToRotate.rotation;
        targetRotation = originalRotation;
        sr.color = defaultColor;
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player") || other.CompareTag("Cube"))
        {
            pressCount++;
            if (!isPressed)
            {
                isPressed = true;
                sr.color = pressedColor;

                // 🔹 calcular nueva rotación destino
                if (mirrorToRotate != null)
                    targetRotation = Quaternion.Euler(0, 0, mirrorToRotate.eulerAngles.z - rotationAngle);
            }
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player") || other.CompareTag("Cube"))
        {
            pressCount--;
            if (pressCount <= 0)
            {
                isPressed = false;
                sr.color = defaultColor;

                // 🔹 volver a la rotación original
                if (mirrorToRotate != null)
                    targetRotation = originalRotation;
            }
        }
    }

    void Update()
    {
        if (mirrorToRotate != null)
        {
            // 🔄 rotación suave del espejo
            mirrorToRotate.rotation = Quaternion.Lerp(
                mirrorToRotate.rotation,
                targetRotation,
                Time.deltaTime * rotationSpeed
            );
        }
    }
}
