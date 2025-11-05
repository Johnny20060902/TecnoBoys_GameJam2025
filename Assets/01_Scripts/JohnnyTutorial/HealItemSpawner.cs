using UnityEngine;

public class HealItemSpawner : MonoBehaviour
{
    [Header("Configuración del spawn")]
    public GameObject healItemPrefab;
    public Transform[] spawnPoints;

    [Tooltip("Cada cuánta vida perdida (del boss) aparece un item")]
    public int phaseThreshold = 200;

    [Tooltip("Frecuencia de chequeo en segundos")]
    public float checkInterval = 0.5f;

    private HealthSystem bossHealth;
    private int lastPhase = 0;

    void Start()
    {
        // Buscar automáticamente al boss
        BossUmbrax boss = FindObjectOfType<BossUmbrax>();
        if (boss != null)
        {
            bossHealth = boss.GetComponent<HealthSystem>();
            Debug.Log($"[Spawner] ✅ Boss detectado: {boss.name}");
        }
        else
        {
            Debug.LogWarning("[Spawner] ❌ No se encontró BossUmbrax en la escena");
        }

        // Iniciar revisión periódica
        InvokeRepeating(nameof(CheckHealth), checkInterval, checkInterval);
    }

    void CheckHealth()
    {
        if (bossHealth == null || healItemPrefab == null || spawnPoints == null || spawnPoints.Length == 0)
            return;

        float lost = bossHealth.maxHealth - bossHealth.currentHealth;
        int phase = Mathf.FloorToInt(lost / Mathf.Max(1, phaseThreshold));

        if (phase > lastPhase)
        {
            lastPhase = phase;
            SpawnHealItem();
        }
    }

    void SpawnHealItem()
    {
        // Escoge un punto de spawn aleatorio de la lista
        Transform chosenPoint = spawnPoints[Random.Range(0, spawnPoints.Length)];
        if (chosenPoint == null)
        {
            Debug.LogWarning("[Spawner] ⚠️ Punto de spawn nulo.");
            return;
        }

        // Crear el ítem en ese punto
        GameObject newItem = Instantiate(healItemPrefab, chosenPoint.position, Quaternion.identity);

        // Forzar escala y sorting
        newItem.transform.localScale = new Vector3(0.3f, 0.3f, 1f);
        SpriteRenderer sr = newItem.GetComponent<SpriteRenderer>();
        if (sr != null)
        {
            sr.sortingOrder = 10; // encima del boss
        }

        Debug.Log($"[Spawner] 💖 Curación generada en {chosenPoint.name} ({chosenPoint.position})");
    }
}
