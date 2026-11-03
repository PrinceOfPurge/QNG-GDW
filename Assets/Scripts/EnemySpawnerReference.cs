
using UnityEngine;

public class EnemySpawnerReference : MonoBehaviour
{
    private EnemySpawner spawner;

    public void SetSpawner(EnemySpawner enemySpawner)
    {
        spawner = enemySpawner;
    }

    private void OnDestroy()
    {
        if (spawner != null)
        {
            spawner.EnemyDied();
        }
    }
}
