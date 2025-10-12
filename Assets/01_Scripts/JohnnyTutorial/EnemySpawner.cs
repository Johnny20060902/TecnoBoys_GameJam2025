using UnityEngine;
using System.Collections.Generic;

[DisallowMultipleComponent]
public class EnemySpawner : MonoBehaviour
{
    [Header("Prefabs & Refs")]
    public Enemy enemyPrefab;             // Asigna tu prefab Enemy
    public Transform player;              // Player_Johnny
    public Collider2D spawnerCollider;    // Asigna el collider del Spawner (el BoxCollider2D de la "casa")

    [Header("Lógica de Spawn")]
    public float radiusActivacion = 8f;   // Distancia para activar
    public float intervalo = 10f;         // Segundos entre spawns
    public int maxVivos = 5;              // Límite simultáneo

    [Header("Punto de salida")]
    public Vector2 spawnOffset = new Vector2(-1.5f, 0f); // SIEMPRE a la IZQUIERDA del spawner

    private float timer = 0f;
    private readonly List<Enemy> vivos = new();

    // control primer spawn
    private bool wasInRange = false;
    private bool firstSpawnDone = false;

    void Update()
    {
        vivos.RemoveAll(e => e == null);
        if (player == null || enemyPrefab == null) return;

        float dist = Vector2.Distance(transform.position, player.position);
        bool inRange = dist <= radiusActivacion;

        // primer spawn instantáneo al entrar por primera vez
        if (inRange && !wasInRange)
        {
            if (!firstSpawnDone && vivos.Count < maxVivos)
            {
                Spawn();
                firstSpawnDone = true;
                timer = 0f;
            }
        }

        if (inRange)
        {
            timer += Time.deltaTime;
            if (timer >= intervalo && vivos.Count < maxVivos)
            {
                Spawn();
                timer = 0f;
            }
        }
        else
        {
            timer = 0f;
        }

        wasInRange = inRange;
    }

    private void Spawn()
    {
        // Posición de salida: a la IZQUIERDA del spawner
        Vector3 pos = transform.position + (Vector3)spawnOffset;
        Enemy e = Instantiate(enemyPrefab, pos, Quaternion.identity);
        e.target = player;

        // Evita empujones: el enemigo NO colisiona con la “casa”
        if (spawnerCollider != null)
        {
            var enemyCol = e.GetComponent<Collider2D>();
            if (enemyCol != null)
                Physics2D.IgnoreCollision(enemyCol, spawnerCollider, true);
        }

        vivos.Add(e);
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, radiusActivacion);

        // Dibuja dónde saldrán los enemigos
        Gizmos.color = Color.magenta;
        Gizmos.DrawWireCube(transform.position + (Vector3)spawnOffset, new Vector3(0.3f, 0.3f, 0f));
    }
}
