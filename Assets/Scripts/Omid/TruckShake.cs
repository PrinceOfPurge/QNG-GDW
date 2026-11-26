using UnityEngine;

public class TruckShake : MonoBehaviour
{
    public static TruckShake Instance { get; private set; }

    [Header("References")]
    [Tooltip("Assign the child object holding the truck meshes & interior camera rig")]
    [SerializeField] private Transform truckVisuals;

    [Header("Shake Settings")]
    [SerializeField] private float tiltAngle = 1.8f;       // How many degrees the truck rocks on hit
    [SerializeField] private float positionJolt = 0.04f;   // Slight sideways shove
    [SerializeField] private float springSpeed = 18f;      // How fast it snaps back
    [SerializeField] private float damping = 6f;           // How fast the wobble settles

    private Vector3 initialLocalPos;
    private Quaternion initialLocalRot;

    private Vector3 currentVelocity;
    private Vector3 currentOffset;
    private Vector3 angularVelocity;
    private Vector3 currentEuler;

    private void Awake()
    {
        Instance = this;

        if (truckVisuals == null)
        {
            truckVisuals = transform;
        }

        initialLocalPos = truckVisuals.localPosition;
        initialLocalRot = truckVisuals.localRotation;
    }

    private void Update()
    {
        // Spring-damper physics for smooth suspension rock
        Vector3 posForce = -springSpeed * currentOffset - damping * currentVelocity;
        currentVelocity += posForce * Time.deltaTime;
        currentOffset += currentVelocity * Time.deltaTime;

        Vector3 rotForce = -springSpeed * currentEuler - damping * angularVelocity;
        angularVelocity += rotForce * Time.deltaTime;
        currentEuler += angularVelocity * Time.deltaTime;

        truckVisuals.localPosition = initialLocalPos + currentOffset;
        truckVisuals.localRotation = initialLocalRot * Quaternion.Euler(currentEuler);
    }

    /// <summary>
    /// Call this whenever a zombie strikes the truck.
    /// </summary>
    public void TriggerHitShake(Vector3 attackerWorldPos)
    {
        // Convert zombie position into truck's local space (X = Left/Right, Z = Front/Back)
        Vector3 localDir = transform.InverseTransformPoint(attackerWorldPos);
        localDir.y = 0f;
        localDir.Normalize();

        // Push truck slightly away from the impact side
        currentVelocity += -localDir * (positionJolt * 10f);

        // Rock chassis: hitting Left (-X) tilts roll (-Z), hitting Front (+Z) pitches back (-X)
        float pitchImpulse = -localDir.z * tiltAngle * 12f;
        float rollImpulse = localDir.x * tiltAngle * 12f;

        angularVelocity += new Vector3(pitchImpulse, 0f, rollImpulse);
    }
}