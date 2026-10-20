using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TruckAttackPoints : MonoBehaviour
{
    public List<Target> targets;
    private Transform target;


    public void SelectTarget()
    {
        List<Transform> freeAttackPoints = new List<Transform>();

        foreach (var target in targets)
        {
            if (target.currentAttackingEnemy == null)
            {
                //add this target to list off possible attack points
                freeAttackPoints.Add(target.transform);
            }
        }
    }
}
