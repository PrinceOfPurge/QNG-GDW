
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [Header("Spawn Settings")]
    [SerializeField] private GameObject enemyPrefab;
    [SerializeField] private Transform[] spawnPoints;
    [SerializeField] private float spawnInterval = 5f;
    [SerializeField] private int maxEnemies = 10;

    private float nextSpawnTime;
    private int currentEnemies;

    private void Update()
    {
        if (currentEnemies >= maxEnemies)
            return;

        if (Time.time >= nextSpawnTime)
        {
            SpawnEnemy();
            nextSpawnTime = Time.time + spawnInterval;
        }
    }

    private void SpawnEnemy()
    {
        if (enemyPrefab == null || spawnPoints.Length == 0)
            return;

        //pick random spawnpoint
        int randomIndex = Random.Range(0, spawnPoints.Length);
        Transform spawnPoint = spawnPoints[randomIndex];

        //create enemy
        GameObject enemy = Instantiate(enemyPrefab,spawnPoint.position,spawnPoint.rotation);

        currentEnemies++;

        // Tell spawner when this enemy is destroyed
        EnemySpawnerReference reference = enemy.AddComponent<EnemySpawnerReference>();
        reference.SetSpawner(this);
    }

    public void EnemyDied()
    {
        currentEnemies--;
    }
}
