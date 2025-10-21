using UnityEngine;
using System;

public class HealthSystem : MonoBehaviour
{
    [Header("Configuración de vida")]
    public float maxHealth = 100f;
    public float currentHealth;

    [Header("Efectos de muerte (opcional)")]
    public GameObject deathEffect; // puedes dejar null si no quieres
    public bool destroyOnDeath = true;

    // Evento para que otros scripts (HUD, animaciones, etc.) se suscriban
    public event Action OnDeath;

    private bool isDead = false;

    void Start()
    {
        currentHealth = maxHealth;
    }

    public void TakeDamage(float amount)
    {
        if (isDead) return;

        currentHealth -= amount;
        currentHealth = Mathf.Clamp(currentHealth, 0f, maxHealth);

        // Si la vida llega a cero, ejecuta muerte
        if (currentHealth <= 0f && !isDead)
        {
            Die();
        }
    }

    public void Heal(float amount)
    {
        if (isDead) return;

        currentHealth += amount;
        currentHealth = Mathf.Clamp(currentHealth, 0f, maxHealth);
    }

    private void Die()
    {
        isDead = true;

        // Efecto de partículas o animación
        if (deathEffect != null)
            Instantiate(deathEffect, transform.position, Quaternion.identity);

        // Llamar eventos (HUD, managers, etc.)
        OnDeath?.Invoke();

        // Si es el jugador, desactivamos controles
        if (CompareTag("Player"))
        {
            // Intenta desactivar control y mostrar mensaje
            var ctrl = GetComponent<PlayerJohnny>();
            if (ctrl != null)
                ctrl.enabled = false;

            Debug.Log("💀 El jugador ha muerto 💀");
        }

        // Si es enemigo, eliminar o reportar al spawner
        if (CompareTag("Enemy"))
        {
            var spawnerTracker = GetComponent<EnemyTracker>();
            if (spawnerTracker != null)
                Destroy(spawnerTracker); // el spawner actualiza conteo en OnDestroy()
        }

        // Destruir objeto si corresponde
        if (destroyOnDeath)
            Destroy(gameObject);
    }
}
