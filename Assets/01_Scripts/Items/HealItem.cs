using UnityEngine;

public class HealItem : MonoBehaviour
{
    public float healAmount = 50f;
    public float floatAmplitude = 0.3f;
    public float floatSpeed = 2f;

    private Vector3 startPos;

    void Start()
    {
        startPos = transform.position;
    }

    void Update()
    {
        // ✨ Efecto flotante suave
        transform.position = startPos + Vector3.up * Mathf.Sin(Time.time * floatSpeed) * floatAmplitude;
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            HealthSystem hs = other.GetComponent<HealthSystem>();
            if (hs != null)
            {
                hs.Heal(healAmount);
                Debug.Log($"💚 Jugador curado +{healAmount}");
            }
            Destroy(gameObject);
        }
    }
}
