using UnityEngine;

public class Player : MonoBehaviour
{
    public HealthBar healthBar;
    public int maxHealth = 150;
    public int currentHealth;
    public ParticleSystem engineSmoke;
    public CollisionDamage carCollisionDamage;
    public Car wheelsOnStatus;

    void Start()
    {
        currentHealth = maxHealth;
        healthBar.SetMaxHealth(maxHealth);
        //Get Engine 
        engineSmoke = wheelsOnStatus.smoke;
        engineSmoke.Stop();
    }
    void Update()
    {
        int currentDamage = carCollisionDamage.CarDamageTaken();
        if (currentDamage > 0)
        {
            TakeDamage(currentDamage);
        }

    }

    void TakeDamage(int damage)
    {
        currentHealth -= damage;

        healthBar.SetHealth(currentHealth, maxHealth);
        if (currentHealth <= 0)
        {
            engineSmoke.Play();
            wheelsOnStatus.WheelsActive(false);
        }
        if (currentHealth > 0)
        {
            engineSmoke.Stop();
        }
    }
}
