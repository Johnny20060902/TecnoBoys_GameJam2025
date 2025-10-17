using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D), typeof(Collider2D))]
public class ElevatorPlatform : MonoBehaviour
{
    [Header("Configuración")]
    public float liftHeight = 5f;          // Cuánto sube
    public float liftSpeed = 2f;           // Velocidad de subida
    public float waitBeforeLift = 1f;      // Cuánto espera antes de subir
    public LayerMask playerMask;           // Capa del jugador
    public float detectionRadius = 2f;     // Distancia para detectar al jugador

    private Rigidbody2D rb;
    private Vector3 startPos;
    private bool isLifting = false;
    private bool hasLifted = false;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.bodyType = RigidbodyType2D.Kinematic;
        startPos = transform.position;
    }

    void Update()
    {
        if (!isLifting && !hasLifted)
        {
            Collider2D player = Physics2D.OverlapCircle(transform.position, detectionRadius, playerMask);
            if (player != null)
            {
                StartCoroutine(LiftAfterDelay());
            }
        }
    }

    private IEnumerator LiftAfterDelay()
    {
        isLifting = true;
        yield return new WaitForSeconds(waitBeforeLift);

        Vector3 targetPos = startPos + Vector3.up * liftHeight;

        // Subida suave
        while (Vector3.Distance(transform.position, targetPos) > 0.05f)
        {
            transform.position = Vector3.MoveTowards(transform.position, targetPos, liftSpeed * Time.deltaTime);
            yield return null;
        }

        hasLifted = true;
        isLifting = false;
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, detectionRadius);
    }
}
