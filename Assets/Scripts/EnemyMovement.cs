using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class EnemyMovement : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] private float moveSpeed = 2f;
    [SerializeField] private float stoppingDistance = 0.5f;

    [Header("Attack Settings")]
    [SerializeField] private float attackDamage = 10f;
    [SerializeField] private float attackCooldown = 1f;

    [Header("Roaming Settings")]
    [SerializeField] private float roamRadius = 10f;
    [SerializeField] private float roamInterval = 2f;

    private NavMeshAgent agent;
    private Animator animator;

    private Transform target;
    private TruckAttackPointManager attackPointManager;
    private Transform currentAttackPoint;

    private IDamageable playerDamageable;

    private float nextAttackTime;
    private float nextRoamTime;

    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();
    }

    private void Start()
    {
        agent.speed = moveSpeed;
        agent.stoppingDistance = stoppingDistance;

        // Zombie has to be on the NavMesh
        if (!agent.isOnNavMesh)
        {
            if (NavMesh.SamplePosition(
                transform.position,
                out NavMeshHit hit,
                5f,
                NavMesh.AllAreas))
            {
                agent.Warp(hit.position);
            }
        }

        GameObject truck = GameObject.FindGameObjectWithTag("Truck");

        if (truck != null)
        {
            target = truck.transform;

            attackPointManager =
                truck.GetComponent<TruckAttackPointManager>();

            if (attackPointManager == null)
            {
                Debug.LogError(
                    "missing TruckAttackPointManager"
                );
            }
        }
        else
        {
            Debug.LogError(
                "truck not found"
            );
        }

        GameObject player = GameObject.FindGameObjectWithTag("Player");

        if (player != null)
        {
            playerDamageable = player.GetComponent<IDamageable>();
        }
    }

    private void Update()
    {
        if (target == null ||
            attackPointManager == null ||
            !agent.enabled ||
            !agent.isOnNavMesh)
        {
            return;
        }

        // Claim attack pos
        if (currentAttackPoint == null)
        {
            TryClaimAttackPoint();

            // Roam if not attacking
            if (currentAttackPoint == null)
            {
                Roam();
            }
        }
        else
        {
            MoveToAttackPoint();
        }

        if (animator != null)
        {
            animator.SetFloat("Speed", agent.velocity.magnitude);
        }
    }

    private void TryClaimAttackPoint()
    {
        if (attackPointManager.TryClaimNearest(
            agent,
            this,
            out Transform point))
        {
            currentAttackPoint = point;

            agent.isStopped = false;
            agent.SetDestination(currentAttackPoint.position);
        }
    }

    private void MoveToAttackPoint()
    {
        if (agent.pathPending)
            return;

        // The zombie has reached its assigned position.
        if (agent.remainingDistance <= agent.stoppingDistance + 0.15f)
        {
            agent.isStopped = true;
            Attack();
        }
        else
        {
            agent.isStopped = false;
        }
    }

    private void Roam()
    {
        agent.isStopped = false;

        // Change roaming position periodically
        if (Time.time < nextRoamTime)
            return;

        nextRoamTime = Time.time + roamInterval;

        Vector3 randomPosition =
            target.position +
            new Vector3(
                Random.Range(-roamRadius, roamRadius),
                0f,
                Random.Range(-roamRadius, roamRadius)
            );

        if (NavMesh.SamplePosition(
            randomPosition,
            out NavMeshHit hit,
            roamRadius,
            NavMesh.AllAreas))
        {
            NavMeshPath path = new NavMeshPath();

            bool pathFound = agent.CalculatePath(
                hit.position,
                path
            );

            if (pathFound &&
                path.status == NavMeshPathStatus.PathComplete)
            {
                agent.SetDestination(hit.position);
            }
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

        // Pulse the directional screen-edge glow relative to the player's camera
        if (ScreenEdgeAttackGlow.Instance != null)
        {
            ScreenEdgeAttackGlow.Instance.TriggerEdgeGlow(transform.position);
        }

        // Rock/shake the truck from the side the zombie hit
        if (TruckShake.Instance != null)
        {
            TruckShake.Instance.TriggerHitShake(transform.position);
        }
    }

    private void FaceTarget()
    {
        Vector3 lookPosition = new Vector3(
            target.position.x,
            transform.position.y,
            target.position.z
        );

        transform.LookAt(lookPosition);
    }

    private void OnDisable()
    {
        // Free attack pos on death
        if (attackPointManager != null)
        {
            attackPointManager.ReleasePoint(this);
        }
    }
}