using UnityEngine;

public class CollisionDamage : MonoBehaviour
{
    public static CollisionDamage Instance { get; private set; }
    public Rigidbody rb;
    private float damageThreshold = 6f;
    private float damageMultiplyer = 2.5f;
    public int damageTaken;
    public bool deerCollision;

    private void Awake()
    {
        Instance = this;
    }

    public void DeerCollision()
    {
        deerCollision = true;
        Debug.Log("DeerCollision");
        float impactSpeed = rb.linearVelocity.magnitude;
        int damage = 0;
        if (impactSpeed > damageThreshold)
        {
            float intialDamage = (impactSpeed * damageMultiplyer) - 18f;
            damage = Mathf.RoundToInt(Mathf.Max(0f, intialDamage));
            damageTaken = Mathf.Max(damageTaken, damage);
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        Debug.Log("Collision");
        float impactSpeed = collision.relativeVelocity.magnitude;
        Debug.Log(impactSpeed);
        int damage = 0;
        if (impactSpeed > damageThreshold && !deerCollision)
        {
            float intialDamage = (impactSpeed * damageMultiplyer) - 18f;
            damage = Mathf.RoundToInt(Mathf.Max(0f, intialDamage));
            damageTaken = Mathf.Max(damageTaken, damage);
        }
    }

    public int CarDamageTaken()
    {
        int tempDamage = damageTaken;
        damageTaken = 0;
        deerCollision = false;
        Debug.Log(tempDamage);
        return tempDamage;
    }   
}
