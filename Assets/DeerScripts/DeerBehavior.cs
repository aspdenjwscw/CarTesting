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
    private bool isFlung; // true once hit - stops normal movement/despawn logic

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
    // this until the feet sit on the surface - there's no way to compute it
    // automatically without knowing the model's exact pivot offset.
    public float groundOffset = 0f;

    [Header("Car Impact")]
    public float carBumpForce = 400f;        // small forward/outward push - a bump, not a launch
    [Range(0f, 1f)]
    public float carSlowdownFactor = 0.7f;   // car's velocity multiplied by this on hit (0.7 = lose 30% speed)

    [Header("Deer Ragdoll")]
    public float deerFlingForce = 6f;        // gentle knockback, not a launch - deer is light
    public float deerFlingUpwardBias = 0.3f;
    public float deerDestroyDelay = 4f;      // seconds before the ragdolled deer is cleaned up
    public Animator deerAnimator;            // optional - auto-found if not assigned, disabled on impact so it stops fighting physics

    [Header("Procedural Gait (stopgap until a real running animation exists)")]
    public bool useProceduralBob = true;
    public float bobHeight = 0.08f;   // how much the body bounces up/down while running
    public float bobSpeed = 10f;      // how fast the bob cycles - tune to look leg-speed-ish
    public float swayAmount = 4f;     // degrees of side-to-side lean while running
    private float bobTimer;
    private Vector3 modelBaseLocalPos;
    private Quaternion modelBaseLocalRot;
    private Transform modelTransform; // the visual mesh child - bobbed independently so it doesn't affect the root's collider/position used for ground snapping and despawn checks

    // The Ragdoll Wizard's bone colliders are solid (non-trigger) by default.
    // Even with their Rigidbody set to kinematic, a solid collider still
    // physically blocks anything that touches it - so every deer was
    // sitting there as a cluster of invisible walls before ever being hit.
    // Fix: force them all to triggers at startup, then switch them solid
    // only once the deer is actually hit and ragdolling.
    private Collider[] ragdollColliders;

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

        // Grab every collider under this object except the root hit-detection
        // trigger, and force them to be triggers too until impact.
        Collider rootCollider = GetComponent<Collider>();
        Collider[] allColliders = GetComponentsInChildren<Collider>();
        System.Collections.Generic.List<Collider> boneColliders = new System.Collections.Generic.List<Collider>();
        foreach (Collider col in allColliders)
        {
            if (col == rootCollider) continue;
            col.isTrigger = true;
            boneColliders.Add(col);
        }
        ragdollColliders = boneColliders.ToArray();
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
            moveSpeed = Random.Range(3f, 6f);
        }

        // Place correctly on the very first frame too, not just after the
        // first Update() runs.
        transform.position = SnapToGround(transform.position);
    }

    void Update()
    {
        // Once flung, ragdoll physics is fully in control - don't fight
        // it with manual transform movement, bobbing, or despawn checks.
        if (isFlung) return;

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
            return hit.point + Vector3.up * groundOffset;
        }
        // Nothing hit (e.g. ran off the edge of the terrain entirely) - hold
        // the previous height rather than falling through to nothing.
        return new Vector3(guessPos.x, transform.position.y, guessPos.z);
    }

    // Deer's root collider is a trigger (sphere collider, Is Trigger checked),
    // so the car passes through it physically by default - this is what
    // actually detects the hit and reacts to it. The ragdoll bone colliders
    // (added by the Ragdoll Wizard) are separate, non-trigger colliders that
    // handle real physical collision with the terrain once activated below.
    void OnTriggerEnter(Collider other)
    {
        if (isFlung) return; // already hit once, ignore re-triggers mid-launch

        Rigidbody hitRb = other.attachedRigidbody;
        if (hitRb != null && hitRb == carRigidbody)
        {
            FlingCar(hitRb);
            ActivateRagdoll();
        }
    }

    void FlingCar(Rigidbody rb)
    {
        // Small horizontal nudge away from the deer, no upward launch and
        // no spin - just a normal "you hit something" bump.
        Vector3 outward = (rb.position - transform.position);
        outward.y = 0f;
        outward.Normalize();

        rb.AddForce(outward * carBumpForce, ForceMode.Impulse);

        // Bleed off some of the car's speed too - a car that just hit a
        // deer shouldn't keep cruising at full speed unaffected.
        rb.linearVelocity *= carSlowdownFactor;
    }

    void ActivateRagdoll()
    {
        isFlung = true;

        // Reset the procedural bob offset so the model isn't stuck mid-bob
        // when physics takes over.
        if (modelTransform != null)
        {
            modelTransform.localPosition = modelBaseLocalPos;
            modelTransform.localRotation = modelBaseLocalRot;
        }

        // Stop the Animator from overriding bone transforms - otherwise it
        // fights the ragdoll physics and the deer snaps back to its
        // animation pose instead of actually going limp.
        if (deerAnimator != null)
        {
            deerAnimator.enabled = false;
        }

        // Now that the deer is actually flying, switch its bone colliders
        // from trigger back to solid so they physically collide with the
        // terrain (bounce/tumble/land) instead of passing through it.
        if (ragdollColliders != null)
        {
            foreach (Collider col in ragdollColliders)
            {
                if (col != null) col.isTrigger = false;
            }
        }

        // The Ragdoll Wizard adds a Rigidbody to every assigned bone. They
        // should have been left as Is Kinematic = true so the deer stays
        // rigid/controlled by this script until now - flip them all to
        // non-kinematic so real physics takes over on every limb.
        Rigidbody[] ragdollBodies = GetComponentsInChildren<Rigidbody>();
        foreach (Rigidbody rb in ragdollBodies)
        {
            if (rb == null || rb.gameObject == this.gameObject) continue; // skip a root rb if one exists
            rb.isKinematic = false;
            rb.useGravity = true;
        }

        // Apply the launch force to whichever bone is closest to the root
        // (usually the spine/pelvis) so the whole ragdoll flies together
        // rather than just one random limb yanking off.
        Rigidbody mainBody = GetClosestRigidbody(ragdollBodies, transform.position);
        if (mainBody != null)
        {
            Vector3 outward = (transform.position - car.position);
            outward.y = 0f;
            outward.Normalize();

            Vector3 flingDirection = (outward + Vector3.up * deerFlingUpwardBias).normalized;
            mainBody.AddForce(flingDirection * deerFlingForce, ForceMode.Impulse);
        }

        Destroy(gameObject, deerDestroyDelay);
    }

    Rigidbody GetClosestRigidbody(Rigidbody[] bodies, Vector3 point)
    {
        Rigidbody closest = null;
        float closestDist = float.MaxValue;
        foreach (Rigidbody rb in bodies)
        {
            float dist = Vector3.Distance(rb.position, point);
            if (dist < closestDist)
            {
                closestDist = dist;
                closest = rb;
            }
        }
        return closest;
    }
}