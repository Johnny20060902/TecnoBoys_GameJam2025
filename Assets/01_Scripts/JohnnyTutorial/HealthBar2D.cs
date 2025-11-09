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
        if (target == null) target = transform.parent;
        if (target != null) health = target.GetComponent<HealthSystem>();
        if (fill != null) fill.color = Color.green;
    }

    void LateUpdate()
    {
        if (target == null || fill == null || health == null) return;

        // 🔧 (1) Posicionar SIEMPRE por encima del sprite del boss (evita que se superponga)
        //     Si no hay SpriteRenderer, usa la posición + worldOffset como antes.
        Vector3 pos = target.position + worldOffset;
        var sr = target.GetComponent<SpriteRenderer>();
        if (sr != null)
        {
            float topY = sr.bounds.max.y;           // borde superior del sprite en mundo
            pos = new Vector3(target.position.x, topY, target.position.z) + worldOffset;
        }
        transform.position = pos;

        // 🔧 (2) Recalcular el porcentaje CADA FRAME (para que sí baje)
        float max = Mathf.Max(1f, health.maxHealth);             // evita /0
        float pct = Mathf.Clamp01(health.currentHealth / max);

        // Escala del fill manteniendo el origen a la izquierda
        fill.transform.localScale = new Vector3(pct * width, fill.transform.localScale.y, 1f);
        float leftOffset = -(width - (pct * width)) * 0.5f;
        fill.transform.localPosition = new Vector3(leftOffset, 0, 0);

        // Color dinámico
        fill.color = (pct > 0.5f)
            ? Color.Lerp(Color.yellow, Color.green, (pct - 0.5f) * 2f)
            : Color.Lerp(Color.red, Color.yellow, pct * 2f);

        // Ocultar si muere
        bool visible = health.currentHealth > 0f;
        if (bg != null) bg.enabled = visible;
        fill.enabled = visible;
    }
}
