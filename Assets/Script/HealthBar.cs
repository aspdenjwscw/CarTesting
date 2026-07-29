using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    public Slider healthBar;
    public Image healthColor;

    public void SetMaxHealth(int maxHealth)
    {
        healthBar.maxValue = maxHealth;
        healthBar.value = maxHealth;
        healthColor.color = Color.green;
    }

    public void SetHealth(int health, int maxhealth)
    {
        healthBar.value = health;
        float healthPercent = ((float)health / maxhealth);
        if (healthPercent >= 0.70) healthColor.color = Color.green;
        else if (healthPercent >= 0.50 && healthPercent < 0.7) healthColor.color = Color.yellow;
        else if (healthPercent >= 0.30) healthColor.color = new Color(1f, 0.5f, 0f);
        else if (healthPercent < 0.3) healthColor.color = Color.red;

    }
}