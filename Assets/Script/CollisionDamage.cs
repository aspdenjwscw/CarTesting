using UnityEngine;

public class CollisionDamage : MonoBehaviour
{
    private float damageThreshold = 6f;
    private float damageMultiplyer = 2.5f;
    public int damageTaken;


    private void OnCollisionEnter(Collision collision)
    {
        float impactSpeed = collision.relativeVelocity.magnitude;
        int damage = 0;
        if (impactSpeed > damageThreshold)
        {
            float intialDamage = (impactSpeed * damageMultiplyer) - 18f;
            damage = Mathf.RoundToInt(Mathf.Max(0f, intialDamage));
            Debug.Log(impactSpeed);
            damageTaken = Mathf.Max(damageTaken, damage);
            Debug.Log(damageTaken);
        }
        

    }

    public int CarDamageTaken()
    {
        int tempDamage = damageTaken;
        damageTaken = 0;
        return tempDamage;
    }   
}
