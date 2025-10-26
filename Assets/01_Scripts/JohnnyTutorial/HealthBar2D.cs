using UnityEngine;

[DisallowMultipleComponent]
public class HealthBar2D : MonoBehaviour
{
    [Header("Referencias")]
    public Transform target;             // objeto al que sigue (enemigo o jugador)
    public SpriteRenderer fill;          // el sprite verde
    public SpriteRenderer bg;            // el fondo

    [Header("Ajustes visuales")]
    public float width = 1.2f;
    public Vector3 worldOffset = new Vector3(0, 1.1f, 0);

    private HealthSystem health;

    void Start()
    {
        if (target == null)
            target = transform.parent;

        if (target != null)
            health = target.GetComponent<HealthSystem>();

        if (fill != null)
            fill.color = Color.green;
    }

    void LateUpdate()
    {
        if (target == null || fill == null || health == null) return;

        // Posición siempre sobre el personaje
        transform.position = target.position + worldOffset;

        // Calcular porcentaje de vida
        float pct = Mathf.Clamp01(health.currentHealth / health.maxHealth);

        // Escala del fill (manteniendo el centro a la izquierda)
        fill.transform.localScale = new Vector3(pct * width, fill.transform.localScale.y, 1f);
        float leftOffset = -(width - (pct * width)) * 0.5f;
        fill.transform.localPosition = new Vector3(leftOffset, 0, 0);

        // Cambiar color dinámico según la vida
        if (pct > 0.5f)
            fill.color = Color.Lerp(Color.yellow, Color.green, (pct - 0.5f) * 2f);
        else
            fill.color = Color.Lerp(Color.red, Color.yellow, pct * 2f);
    }
}
