using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class EnemyFactory : MonoBehaviour
{
    public abstract PooledEnemy CreateEnemy(Vector3 position, Quaternion rotation);
}
