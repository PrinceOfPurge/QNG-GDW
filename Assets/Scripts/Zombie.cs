using UnityEngine;
using UnityEngine.AI;

public class Zombie : MonoBehaviour, IDamageable
{
    [Header("Health & Ragdoll")]
    [SerializeField] private float health = 25f;
    [SerializeField] private float despawnDelay = 5f;

    private Rigidbody[] ragdollBodies;
    private Collider[] ragdollColliders;
    private bool isDead = false;

    private void Awake()
    {
        ragdollBodies = GetComponentsInChildren<Rigidbody>();
        ragdollColliders = GetComponentsInChildren<Collider>();

        // Keep ragdoll off while walking
        SetRagdollState(false);
    }

    public void TakeDamage(float damage)
    {
        TakeDamageWithForce(damage, Vector3.zero, transform.position);
    }

    public void TakeDamageWithForce(float damage, Vector3 hitForce, Vector3 hitPoint)
    {
        if (isDead) return;

        health -= damage;
        if (health <= 0f)
        {
            Die(hitForce, hitPoint);
        }
    }

    private void Die(Vector3 hitForce, Vector3 hitPoint)
    {
        isDead = true;

        EnemyMovement movement = GetComponent<EnemyMovement>();
        if (movement != null) movement.enabled = false;

        NavMeshAgent agent = GetComponent<NavMeshAgent>();
        if (agent != null) agent.enabled = false;

        Animator animator = GetComponent<Animator>();
        if (animator != null) animator.enabled = false;

        // Activate ragdoll physics
        SetRagdollState(true);

        // Apply bullet knockback to the closest bone
        if (hitForce != Vector3.zero)
        {
            Rigidbody closestBone = GetClosestBone(hitPoint);
            if (closestBone != null)
            {
                closestBone.AddForceAtPosition(hitForce, hitPoint, ForceMode.Impulse);
            }
        }

        Destroy(gameObject, despawnDelay);
    }

    private void SetRagdollState(bool active)
    {
        foreach (Rigidbody rb in ragdollBodies)
        {
            if (rb.transform == transform) continue;
            rb.isKinematic = !active;
        }

        foreach (Collider col in ragdollColliders)
        {
            if (col.transform == transform) continue;
            col.isTrigger = !active;
        }
    }

    private Rigidbody GetClosestBone(Vector3 point)
    {
        Rigidbody closest = null;
        float minDist = Mathf.Infinity;

        foreach (Rigidbody rb in ragdollBodies)
        {
            if (rb.transform == transform) continue;
            float dist = Vector3.Distance(rb.position, point);
            if (dist < minDist)
            {
                minDist = dist;
                closest = rb;
            }
        }
        return closest;
    }
}