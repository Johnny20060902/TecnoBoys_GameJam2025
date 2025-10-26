using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [Header("Prefabs de enemigos")]
    public GameObject meleeEnemyPrefab;
    public GameObject shooterEnemyPrefab;

    [Header("Configuración de spawn")]
    [Tooltip("Tiempo entre spawns")]
    public float spawnRate = 5f;

    [Tooltip("Cantidad máxima de enemigos simultáneos")]
    public int maxEnemies = 3;

    [Tooltip("Distancia lateral desde el spawner donde aparecerán los enemigos")]
    public float spawnRadius = 3f;

    private float nextSpawn;
    private int currentEnemies;

    void Update()
    {
        if (Time.time > nextSpawn && currentEnemies < maxEnemies)
        {
            SpawnRandomEnemy();
            nextSpawn = Time.time + spawnRate;
        }
    }

    void SpawnRandomEnemy()
    {
        // 🔹 Elegir tipo de enemigo: 0 = melee, 1 = shooter
        int type = Random.Range(0, 2);
        GameObject prefab = (type == 0) ? meleeEnemyPrefab : shooterEnemyPrefab;

        // 🔹 Elegir lado (izquierda o derecha)
        float side = Random.value < 0.5f ? -1f : 1f;
        Vector3 spawnPos = transform.position + new Vector3(spawnRadius * side, 0f, 0f);

        // 🔹 Instanciar enemigo
        GameObject enemy = Instantiate(prefab, spawnPos, Quaternion.identity);
        currentEnemies++;

        // 🔹 Hacer que mire hacia el centro (o jugador si está)
        Transform player = GameObject.FindGameObjectWithTag("Player")?.transform;
        if (player != null)
        {
            Vector3 dir = player.position - enemy.transform.position;
            float desired = Mathf.Sign(dir.x);
            Vector3 scale = enemy.transform.localScale;
            scale.x = Mathf.Abs(scale.x) * desired;
            enemy.transform.localScale = scale;
        }
        else
        {
            // si no hay jugador, simplemente mira hacia el centro del mapa
            Vector3 scale = enemy.transform.localScale;
            scale.x = Mathf.Abs(scale.x) * -side;
            enemy.transform.localScale = scale;
        }

        // 🔹 Registrar al spawner
        EnemyTracker tracker = enemy.AddComponent<EnemyTracker>();
        tracker.Init(this);
    }

    // 🔹 Llamado por los enemigos al morir
    public void EnemyDied()
    {
        currentEnemies = Mathf.Max(0, currentEnemies - 1);
    }
}


// ===== Clase auxiliar =====
public class EnemyTracker : MonoBehaviour
{
    private EnemySpawner spawner;

    public void Init(EnemySpawner spawnerRef)
    {
        spawner = spawnerRef;
    }

    private void OnDestroy()
    {
        if (spawner != null)
            spawner.EnemyDied();
    }
}
