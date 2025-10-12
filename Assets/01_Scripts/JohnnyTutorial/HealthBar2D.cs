using UnityEngine;

[DisallowMultipleComponent]
public class HealthBar2D : MonoBehaviour
{
    [Header("Refs")]
    public Transform target;            // Normalmente el Enemy (padre)
    public SpriteRenderer fill;         // Asigna el hijo Fill
    public SpriteRenderer bg;           // Asigna el hijo BG

    [Header("Medidas")]
    public float width = 1.2f;          // Debe coincidir con el Scale.x del BG/Fill
    public Vector3 worldOffset = new Vector3(0, 1.1f, 0);

    // Detecta automáticamente si es Enemy o Spawner
    Enemy enemy;
    DestructibleSpawner spawner;

    void Awake()
    {
        if (target == null) target = transform.parent;
        enemy   = target ? target.GetComponent<Enemy>() : null;
        spawner = target ? target.GetComponent<DestructibleSpawner>() : null;
    }

    void LateUpdate()
    {
        if (target == null || fill == null) return;

        // Seguir al objetivo en mundo
        transform.position = target.position + worldOffset;

        // Calcular porcentaje
        float pct = 1f;
        if (enemy != null)
        {
            float max = Mathf.Max(1f, enemy.maxHp);
            pct = Mathf.Clamp01(enemy.hp / max);
        }
        else if (spawner != null)
        {
            float hpMax = 10f; // cámbialo si tienes hpMax en tu spawner
            pct = Mathf.Clamp01(spawner.hp / hpMax);
        }

        // Escalar barra de izquierda a derecha (pivot centrado → compensar posición)
        fill.transform.localScale = new Vector3(Mathf.Max(0f, pct) * width, fill.transform.localScale.y, 1f);
        float leftOffset = -(width - (pct * width)) * 0.5f;
        fill.transform.localPosition = new Vector3(leftOffset, 0f, 0f);
    }
}
