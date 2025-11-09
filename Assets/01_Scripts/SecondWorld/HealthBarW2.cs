using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class HealthBarW2 : MonoBehaviour
{
    public Image fillImage;
    private float maxHealth;
    public Text TextHealth;

    public void SetMaxHealth(float health)
    {
        maxHealth = health;
        fillImage.fillAmount = 1f;
        TextHealth.text = health.ToString();
    }

    public void SetHealth(float currentHealth)
    {
        fillImage.fillAmount = currentHealth / maxHealth;
        TextHealth.text = currentHealth.ToString();

        if (currentHealth <= 0)
        {
            gameObject.SetActive(false);
        }

    }
}
