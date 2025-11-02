using UnityEngine;
using TMPro;

public class PlayerHealthUI : MonoBehaviour
{
    [Header("Referencias UI")]
    public Transform healthFill;       // el cuadrado (UI_FillSquare)
    public TMP_Text healthText;        // el texto de vida

    [Header("Referencias del sistema")]
    public HealthSystem playerHealth;

    private Vector3 originalScale;
    private float targetScaleX;

    void Start()
    {
        if (playerHealth == null)
            playerHealth = FindObjectOfType<PlayerJohnny>()?.GetComponent<HealthSystem>();

        if (healthFill != null)
            originalScale = healthFill.localScale;
    }

    void Update()
    {
        if (playerHealth == null || healthFill == null || healthText == null) return;

        // Calcular porcentaje
        float pct = Mathf.Clamp01(playerHealth.currentHealth / playerHealth.maxHealth);

        // Suavizado de transición de escala
        targetScaleX = Mathf.Lerp(targetScaleX, pct, Time.deltaTime * 8f);
        healthFill.localScale = new Vector3(originalScale.x * targetScaleX, originalScale.y, originalScale.z);

        // Cambiar color del SpriteRenderer
        SpriteRenderer sr = healthFill.GetComponent<SpriteRenderer>();
        if (sr != null)
        {
            if (pct > 0.5f)
                sr.color = Color.Lerp(Color.yellow, Color.green, (pct - 0.5f) * 2f);
            else
                sr.color = Color.Lerp(Color.red, Color.yellow, pct * 2f);
        }

        // Mostrar valor numérico
        healthText.text = Mathf.RoundToInt(playerHealth.currentHealth).ToString();
    }
}
