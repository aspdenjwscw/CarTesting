using UnityEngine;

// Second pass: fixed 20m spawn distance felt unfair at high speed (barely any
// reaction time) and pointless at low speed (deer just sits there forever).
// Now scaling spawn distance off the car's current velocity, plus adding a
// random lateral offset so the deer doesn't always spawn dead-center on the road.
public class DeerSpawner : MonoBehaviour
{
    public Transform car;
    public GameObject deerPrefab;
    public Rigidbody carRigidbody; // used to read current speed

    [Range(0f, 1f)] public float baseChancePerCheck = 0.01f;
    public float checkInterval = 2f;

    public float minSpawnDistance = 15f;
    public float maxSpawnDistance = 30f;
    public float speedDistanceMultiplier = 0.5f; // extra distance per unit of speed
    public float lateralSpread = 6f;

    [Header("Despawn")]
    public float despawnDistanceBehindCar = 20f;

    [Header("Ground Placement")]
    public LayerMask groundLayer;      // set this to whatever layer your road/terrain is on
    public float raycastHeight = 50f;  // start the downward ray this high above the guess position

    private float timer;

    void Update()
    {
        timer += Time.deltaTime;
        if (timer >= checkInterval)
        {
            timer = 0f;
            TryTriggerDeerEvent();
        }
    }

    void TryTriggerDeerEvent()
    {
        if (Random.value <= baseChancePerCheck)
        {
            SpawnDeer();
        }
    }

    void SpawnDeer()
    {
        float speed = carRigidbody != null ? carRigidbody.linearVelocity.magnitude : 10f;

        // Faster car = spawn further ahead, so reaction time stays roughly constant
        float distanceAhead = Random.Range(minSpawnDistance, maxSpawnDistance)
                               + speed * speedDistanceMultiplier;

        Vector3 forwardOffset = car.forward * distanceAhead;
        Vector3 lateralOffset = car.right * Random.Range(-lateralSpread, lateralSpread);
        Vector3 spawnPos = car.position + forwardOffset + lateralOffset;
        spawnPos = SnapToGround(spawnPos);

        // Face perpendicular to the road (car.right or -car.right) rather than
        // facing the car head-on - reads as "crossing the road" / t-bone risk,
        // not a deer staring down the headlights.
        bool facingRight = Random.value > 0.5f;
        Quaternion spawnRotation = Quaternion.LookRotation(facingRight ? car.right : -car.right);

        GameObject deer = Instantiate(deerPrefab, spawnPos, spawnRotation);

        // Hand the deer its own logic (charge/freeze + despawn) rather than
        // the spawner babysitting every deer that exists in the scene.
        DeerBehavior behavior = deer.GetComponent<DeerBehavior>();
        if (behavior == null)
        {
            behavior = deer.AddComponent<DeerBehavior>();
        }
        behavior.Init(car, carRigidbody, despawnDistanceBehindCar, facingRight, groundLayer, raycastHeight);
    }

    // Using the car's Y position for the spawn point broke on any slope or
    // uneven terrain (deer would spawn floating or half-buried). Raycasting
    // straight down from above the guessed position fixes that.
    Vector3 SnapToGround(Vector3 guessPos)
    {
        Vector3 rayStart = guessPos + Vector3.up * raycastHeight;
        if (Physics.Raycast(rayStart, Vector3.down, out RaycastHit hit, raycastHeight * 2f, groundLayer))
        {
            return hit.point;
        }
        // Fall back to the original guess if nothing was hit (e.g. groundLayer not set up yet)
        return guessPos;
    }
}