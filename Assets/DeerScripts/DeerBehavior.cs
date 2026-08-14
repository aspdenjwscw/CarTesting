using UnityEngine;

// Handles what a spawned deer actually does, and cleans itself up once
// it's behind the car. Pulled out of DeerSpawner so the spawner only
// worries about *when/where* to spawn, and this only worries about
// *what the deer does once it exists*.
public class DeerBehavior : MonoBehaviour
{
    private Transform car;
    private Rigidbody carRigidbody;
    private float despawnDistanceBehind;

    private Vector3 moveDirection;
    private float moveSpeed;
    private bool isCharging;
    private bool isHit; // true once hit - stops normal movement/despawn logic

    // Ground follow, so charging deer don't walk off a slope/dip and end up
    // visibly floating - only the spawn point was snapped before, not every
    // frame while moving.
    private LayerMask groundLayer;
    private float raycastHeight;

    [Header("Ground Fit")]
    // If the model's pivot isn't exactly at its feet, snapping the pivot to
    // the ground surface makes it sink in (pivot mid-body = half the model
    // underground). Positive value lifts the whole deer up to compensate.
    // Negative value pushes it down if it's floating instead. Just nudge
    // this until the feet sit on the surface.
    public float groundOffset = 0f;

    [Header("Car Impact")]
    public float carBumpForce = 400f;        // small forward/outward push - a bump, not a launch
    [Range(0f, 1f)]
    public float carSlowdownFactor = 0.5f;   // car's velocity multiplied by this on hit (0.5 = lose half its speed)
    public float carBrakeDuration = 1f;      // how long to keep suppressing the car's speed after impact - counters the car's own engine re-accelerating it back up instantly

    [Header("Deer Knockback")]
    public float deerKnockbackForce = 6f;    // gentle knockback, not a launch - deer is light
    public float deerKnockbackUpwardBias = 0.3f;
    public float deerSpinTorque = 3f;        // light tumble on impact
    public float deerDestroyDelay = 3f;      // seconds before the knocked-back deer is cleaned up
    public Animator deerAnimator;            // optional - auto-found if not assigned, disabled on impact so it stops fighting physics

    [Header("Procedural Gait (stopgap - turn off now that real animations exist)")]
    public bool useProceduralBob = false;
    public float bobHeight = 0.08f;   // how much the body bounces up/down while running
    public float bobSpeed = 10f;      // how fast the bob cycles - tune to look leg-speed-ish
    public float swayAmount = 4f;     // degrees of side-to-side lean while running
    private float bobTimer;
    private Vector3 modelBaseLocalPos;
    private Quaternion modelBaseLocalRot;
    private Transform modelTransform; // the visual mesh child - bobbed independently so it doesn't affect the root's collider/position used for ground snapping and despawn checks

    [Header("Real Animation State Names")]
    // Must match the exact state names inside your DeerAnimator controller.
    public string idleStateName = "Deer_EatIdle";
    public string walkStateName = "Deer_Walk";
    public string runStateName = "Deer_Run";
    public float runSpeedThreshold = 4.5f; // unused now, kept for backwards compatibility - see walkMoveSpeed/runMoveSpeed below

    [Header("Movement Speed (tune these to match your animation's actual foot pace - too fast looks like sliding/skating)")]
    public float walkMoveSpeedMin = 1.2f;
    public float walkMoveSpeedMax = 2f;
    public float runMoveSpeedMin = 4f;
    public float runMoveSpeedMax = 6f;
    private bool isRunning;
    public string headbuttStateName = "Deer_Headbutt";
    public bool playHeadbuttOnHit = true;

    void Awake()
    {
        // Assumes the visual mesh/armature is the first child of the root
        // (typical for an imported FBX with a collider added on the root).
        // If your hierarchy is different, drag the correct child into this
        // field manually in the Inspector instead of relying on this guess.
        if (transform.childCount > 0)
        {
            modelTransform = transform.GetChild(0);
            modelBaseLocalPos = modelTransform.localPosition;
            modelBaseLocalRot = modelTransform.localRotation;
        }
    }

    public void Init(Transform carTransform, Rigidbody carRb, float despawnDistance,
                      bool facingRight, LayerMask ground, float rayHeight)
    {
        car = carTransform;
        carRigidbody = carRb;
        despawnDistanceBehind = despawnDistance;
        groundLayer = ground;
        raycastHeight = rayHeight;

        if (deerAnimator == null)
        {
            deerAnimator = GetComponentInChildren<Animator>();
        }

        // 50/50 for now: either the deer bolts across the road, or freezes
        // in place ("deer in headlights"). Weighting this properly is a
        // later pass once playtesting shows which feels more common irl.
        isCharging = Random.value > 0.5f;

        if (isCharging)
        {
            // Move the direction the deer is already facing (set by the
            // spawner) rather than picking independently - otherwise it
            // could visually run backward.
            moveDirection = facingRight ? car.right : -car.right;

            // Decide Walk vs Run FIRST, then pick a speed within that
            // specific animation's natural pace - picking one random speed
            // and guessing which clip fits it afterward caused foot sliding
            // whenever the number didn't actually match either clip's speed.
            isRunning = Random.value > 0.5f;
            moveSpeed = isRunning
                ? Random.Range(runMoveSpeedMin, runMoveSpeedMax)
                : Random.Range(walkMoveSpeedMin, walkMoveSpeedMax);
        }

        // Place correctly on the very first frame too, not just after the
        // first Update() runs.
        transform.position = SnapToGround(transform.position);

        PlayStateAnimation();
    }

    void PlayStateAnimation()
    {
        if (deerAnimator == null) return;

        if (!isCharging)
        {
            deerAnimator.Play(idleStateName);
        }
        else
        {
            deerAnimator.Play(isRunning ? runStateName : walkStateName);
        }
    }

    void Update()
    {
        // Once hit, physics is fully in control - don't fight it with
        // manual transform movement, bobbing, or despawn checks.
        if (isHit) return;

        if (isCharging)
        {
            Vector3 nextPos = transform.position + moveDirection * moveSpeed * Time.deltaTime;
            transform.position = SnapToGround(nextPos);

            if (useProceduralBob)
            {
                ApplyProceduralBob();
            }
        }

        // Despawn once the deer is well behind the car so we're not
        // accumulating dead GameObjects over a long drive.
        Vector3 toDeer = transform.position - car.position;
        if (Vector3.Dot(toDeer, car.forward) < -despawnDistanceBehind)
        {
            Destroy(gameObject);
        }
    }

    void ApplyProceduralBob()
    {
        if (modelTransform == null) return;

        bobTimer += Time.deltaTime * bobSpeed;

        // Vertical bounce - two bounces per stride, like alternating leg pairs
        float bob = Mathf.Abs(Mathf.Sin(bobTimer)) * bobHeight;

        // Side-to-side lean, opposite phase to the bounce for a running feel
        float sway = Mathf.Sin(bobTimer * 0.5f) * swayAmount;

        modelTransform.localPosition = modelBaseLocalPos + Vector3.up * bob;
        modelTransform.localRotation = modelBaseLocalRot * Quaternion.Euler(0f, 0f, sway);
    }

    Vector3 SnapToGround(Vector3 guessPos)
    {
        Vector3 rayStart = guessPos + Vector3.up * raycastHeight;
        if (Physics.Raycast(rayStart, Vector3.down, out RaycastHit hit, raycastHeight * 2f, groundLayer))
        {
            // Also lean the deer to match the slope, not just its height.
            // Keep facing the same direction it was already facing, just
            // tilt "up" to match the ground normal instead of staying
            // perfectly vertical on an incline.
            Quaternion targetRot = Quaternion.FromToRotation(transform.up, hit.normal) * transform.rotation;
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRot, Time.deltaTime * 8f);

            return hit.point + Vector3.up * groundOffset;
        }
        // Nothing hit (e.g. ran off the edge of the terrain entirely) - hold
        // the previous height rather than falling through to nothing.
        return new Vector3(guessPos.x, transform.position.y, guessPos.z);
    }

    // Deer's collider is a trigger (sphere collider, Is Trigger checked),
    // so the car passes through it physically by default - this is what
    // actually detects the hit and reacts to it.
    void OnTriggerEnter(Collider other)
    {
        if (isHit) return; // already hit once, ignore re-triggers mid-knockback

        CollisionDamage.Instance.DeerCollision();
        Rigidbody hitRb = other.attachedRigidbody;
        if (hitRb != null && hitRb == carRigidbody)
        {
            BumpCar(hitRb);
            KnockbackDeer();
        }
    }

    void BumpCar(Rigidbody rb)
    {
        // Small horizontal nudge away from the deer, no upward launch and
        // no spin - just a normal "you hit something" bump.
        Vector3 outward = (rb.position - transform.position);
        outward.y = 0f;
        outward.Normalize();

        rb.AddForce(outward * carBumpForce, ForceMode.Impulse);

        // A single instant velocity cut can get immediately overridden if
        // the car's own drive script keeps pushing it back up to speed on
        // the very next physics step. Holding the cap for a short duration
        // makes the slowdown actually noticeable instead of disappearing
        // within a single frame.
        StartCoroutine(BrakeCar(rb));
    }

    System.Collections.IEnumerator BrakeCar(Rigidbody rb)
    {
        rb.linearVelocity *= carSlowdownFactor;
        float capSpeed = rb.linearVelocity.magnitude;

        float elapsed = 0f;
        while (elapsed < carBrakeDuration)
        {
            if (rb.linearVelocity.magnitude > capSpeed)
            {
                rb.linearVelocity = rb.linearVelocity.normalized * capSpeed;
            }
            elapsed += Time.deltaTime;
            yield return null;
        }
    }

    void KnockbackDeer()
    {
        isHit = true;

        // Reset the procedural bob offset so the model isn't stuck mid-bob
        // when physics takes over.
        if (modelTransform != null)
        {
            modelTransform.localPosition = modelBaseLocalPos;
            modelTransform.localRotation = modelBaseLocalRot;
        }

        // Play the headbutt reaction instead of just freezing the animator.
        // As long as "Apply Root Motion" is off on the Animator (should be),
        // this only affects the visual pose, not the actual transform - so
        // it's safe to let it keep playing while physics handles the real
        // knockback movement below.
        if (deerAnimator != null && playHeadbuttOnHit)
        {
            deerAnimator.Play(headbuttStateName);
        }
        else if (deerAnimator != null)
        {
            deerAnimator.enabled = false;
        }

        // The collider used for hit detection is a trigger, and triggers
        // never physically collide with anything - including the ground.
        // Without this, the deer falls through the terrain forever once
        // gravity kicks in below. Switch it (and any other colliders on
        // this object or its children) solid now that it's been hit.
        Collider[] cols = GetComponentsInChildren<Collider>();
        foreach (Collider col in cols)
        {
            col.isTrigger = false;
        }

        // Give the deer a real Rigidbody so physics (gravity + the impulse
        // below) can take over and it visibly reacts instead of just
        // vanishing. It was moved manually via transform up to now.
        Rigidbody deerRb = GetComponent<Rigidbody>();
        if (deerRb == null)
        {
            deerRb = gameObject.AddComponent<Rigidbody>();
        }
        deerRb.isKinematic = false;
        deerRb.useGravity = true;

        Vector3 outward = (transform.position - car.position);
        outward.y = 0f;
        outward.Normalize();

        Vector3 knockbackDirection = (outward + Vector3.up * deerKnockbackUpwardBias).normalized;

        deerRb.AddForce(knockbackDirection * deerKnockbackForce, ForceMode.Impulse);
        deerRb.AddTorque(Random.insideUnitSphere * deerSpinTorque, ForceMode.Impulse);

        Destroy(gameObject, deerDestroyDelay);
    }
}