using UnityEngine;

public class Player : MonoBehaviour
{
    public HealthBar healthBar;
    public int maxHealth = 150;
    public int currentHealth;

    public CollisionDamage carCollisionDamage;

    void Start()
    {
        currentHealth = maxHealth;
        healthBar.SetMaxHealth(maxHealth);
    }
    void Update()
    {
        int currentDamage = carCollisionDamage.CarDamageTaken();
        if (currentDamage > 0)
        {
            TakeDamage(currentDamage);
        }

    }

    void TakeDamage (int damage)
    {
        currentHealth -= damage;

        healthBar.SetHealth(currentHealth, maxHealth);
    }
}
