using UnityEngine;
using System;

public class HealthSystem : MonoBehaviour
{
    [Header("Configuración de vida")]
    public float maxHealth = 100f;
    public float currentHealth;

    [Header("Efectos de muerte (opcional)")]
    public GameObject deathEffect;
    public bool destroyOnDeath = true;

    public event Action OnDeath;

    private bool isDead = false;

    public GameObject deathpanel;

    void Start()
    {
        currentHealth = maxHealth;
    }

    public void TakeDamage(float dmg)
    {
        if (isDead) return;

        currentHealth -= dmg;
        Debug.Log($"🔥 Daño recibido: {dmg}, vida restante: {currentHealth}");

        if (currentHealth <= 0)
        {
            currentHealth = 0;
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
        if (isDead) return;
        isDead = true;

        Debug.Log($"💀 {name} ha muerto");

        // 💥 Efecto visual opcional
        if (deathEffect != null)
            Instantiate(deathEffect, transform.position, Quaternion.identity);

        // 🔔 Notificar muerte a otros sistemas (BossUmbrax, UI, etc.)
        OnDeath?.Invoke();

        // 🧍 Si es jugador
        if (CompareTag("Player"))
        {
            var ctrl = GetComponent<PlayerJohnny>();
            if (ctrl != null) ctrl.enabled = false;
            deathpanel.SetActive(true);
        }

        // 👾 Si es enemigo, informar a su tracker
        if (CompareTag("Enemy"))
        {
            var tracker = GetComponent<EnemyTracker>();
            if (tracker != null)
                Destroy(tracker);
        }

        // 🧨 Destruir si corresponde
        if (destroyOnDeath)
            Destroy(gameObject);
    }
}
