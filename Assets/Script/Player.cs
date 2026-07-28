using UnityEngine;

public class Player : MonoBehaviour
{
    public HealthBar healthBar;
    public int maxHealth = 150;
    public int currentHealth;

    void Start()
    {
        currentHealth = maxHealth;
        healthBar.SetMaxHealth(maxHealth);
    }
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Y))
        {
            TakeDamage(20);
        }
    }

    void TakeDamage (int damage)
    {
        currentHealth -= damage;

        healthBar.SetHealth(currentHealth);
    }
}
