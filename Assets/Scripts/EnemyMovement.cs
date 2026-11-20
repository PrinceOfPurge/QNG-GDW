using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class EnemyMovement : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] private float moveSpeed = 2f;
    [SerializeField] private float stoppingDistance = 0.5f;

    [Header("Attack Settings")]
    [SerializeField] private float attackRange = 1.8f;
    [SerializeField] private float attackDamage = 10f;
    [SerializeField] private float attackCooldown = 1f;

    private NavMeshAgent agent;
    private Animator animator;
    private Transform target;
    private Collider targetCollider;
    private IDamageable playerDamageable;
    private float nextAttackTime;

    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();
    }

    private void Start()
    {
        agent.speed = moveSpeed;
        agent.stoppingDistance = stoppingDistance;

        if (!agent.isOnNavMesh)
        {
            if (NavMesh.SamplePosition(transform.position, out NavMeshHit hit, 5f, NavMesh.AllAreas))
            {
                agent.Warp(hit.position);
            }
        }

        GameObject truck = GameObject.FindGameObjectWithTag("Truck");
        if (truck != null)
        {
            target = truck.transform;

            // Find the SOLID (non-trigger) collider on the truck so triggers don't trick distance checks
            Collider[] truckColliders = truck.GetComponentsInChildren<Collider>();
            foreach (Collider col in truckColliders)
            {
                if (!col.isTrigger)
                {
                    targetCollider = col;
                    break;
                }
            }
        }
        else
        {
            Debug.LogError("Truck not found! Make sure the truck has the 'Truck' tag.");
        }

        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            playerDamageable = player.GetComponent<IDamageable>();
        }
    }
    
    private void Update()
    {
        if (target == null || !agent.enabled || !agent.isOnNavMesh)
            return;

        Vector3 rawTarget = targetCollider != null
            ? targetCollider.ClosestPoint(transform.position)
            : target.position;

        Vector3 targetPoint = rawTarget;
        if (NavMesh.SamplePosition(rawTarget, out NavMeshHit hit, 3.0f, NavMesh.AllAreas))
        {
            targetPoint = hit.position;
        }

        float distance = Vector3.Distance(transform.position, targetPoint);

        if (distance <= attackRange)
        {
            agent.isStopped = true;
            Attack();
        }
        else
        {
            agent.isStopped = false;
            agent.SetDestination(targetPoint);
        }

        if (animator != null)
        {
            animator.SetFloat("Speed", agent.velocity.magnitude);
        }
    }

    private void Attack()
    {
        FaceTarget();

        if (Time.time < nextAttackTime)
            return;

        nextAttackTime = Time.time + attackCooldown;

        if (playerDamageable != null)
        {
            playerDamageable.TakeDamage(attackDamage);
        }
    }

    private void FaceTarget()
    {
        Vector3 lookPos = new Vector3(target.position.x, transform.position.y, target.position.z);
        transform.LookAt(lookPos);
    }
}