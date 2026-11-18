using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasicEnemyFactory : EnemyFactory
{
    [SerializeField] private GameObject enemyPrafab;
    public override PooledEnemy CreateEnemy(Vector3 position, Quaternion rotation)
    {
        GameObject enemyObject = Instantiate(enemyPrafab, position, rotation);
        if(!enemyObject.TryGetComponent(out PooledEnemy pooled))
        {
            pooled = enemyObject.AddComponent<PooledEnemy>();
        }
        return pooled;
    }
}
