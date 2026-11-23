using UnityEngine;
using UnityEngine.AI;

public class TruckAttackPointManager : MonoBehaviour
{
    [Header("Attack Positions")]
    [SerializeField] private Transform[] attackPoints;

    private EnemyMovement[] occupants;

    private void Awake()
    {
        occupants = new EnemyMovement[attackPoints.Length];
    }

    public bool TryClaimNearest(NavMeshAgent agent,EnemyMovement enemy,out Transform chosenPoint)
    {
        chosenPoint = null;

        float shortestDistance = Mathf.Infinity;
        int bestIndex = -1;

        NavMeshPath path = new NavMeshPath();

        for (int i = 0; i < attackPoints.Length; i++)
        {
            if (occupants[i] != null)
                continue;

            if (attackPoints[i] == null)
                continue;

            //generate a path to the attack point
            bool pathFound = agent.CalculatePath(
                attackPoints[i].position,
                path
            );

            if (!pathFound || path.status != NavMeshPathStatus.PathComplete)
                continue;

            //get path length
            float pathDistance = 0f;

            for (int j = 1; j < path.corners.Length; j++)
            {
                pathDistance += Vector3.Distance(
                    path.corners[j - 1],
                    path.corners[j]
                );
            }

            if (pathDistance < shortestDistance)
            {
                shortestDistance = pathDistance;
                bestIndex = i;
            }
        }

        if (bestIndex == -1)
            return false;

        //make a reservation for enemy
        occupants[bestIndex] = enemy;
        chosenPoint = attackPoints[bestIndex];

        return true;
    }

    public void ReleasePoint(EnemyMovement enemy)
    {
        for (int i = 0; i < occupants.Length; i++)
        {
            if (occupants[i] == enemy)
            {
                occupants[i] = null;
                return;
            }
        }
    }
}
