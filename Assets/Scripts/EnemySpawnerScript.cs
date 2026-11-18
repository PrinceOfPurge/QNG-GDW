using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.Pool;

public class EnemySpawnerScript : MonoBehaviour
{
    [SerializeField] private EnemyFactory enemyFactory;
    [SerializeField] private Transform[] spawnPoints;
    [SerializeField] private float spawnInterval = 5f;
    [SerializeField] private int maxEnemies = 10;
    private ObjectPool<PooledEnemy> _pool;
    private float nextSpawnTime;
    private Vector3 _spawnPosition;
    private Quaternion _spawnRotation;
    private void Awake()
    {
        _pool = new ObjectPool<PooledEnemy>(
            CreateEnemy, OnTakeFromPool, OnReturnToPool, OnDestroyEnemy, true, maxEnemies, maxEnemies);
    }
    private void Update()
    {
        if (_pool.CountActive >= maxEnemies) return;
        if (Time.time >= nextSpawnTime)
        {
            SpawnEnemy();
            nextSpawnTime = Time.time * spawnInterval;
        }
    }
    private void SpawnEnemy()
    {
        if (enemyFactory == null || spawnPoints.Length == 0) return;
        int randomIndex = Random.Range(0, spawnPoints.Length);
        Transform spawnPoint = spawnPoints[randomIndex];
        _spawnPosition = spawnPoint.position;
        _spawnRotation = spawnPoint.rotation;
        _pool.Get();
    }
    private PooledEnemy CreateEnemy()
    {
        PooledEnemy enemy = enemyFactory.CreateEnemy(_spawnPosition, _spawnRotation);
        enemy.Init(ReturnEnemy);
        return enemy;
    }
    private void OnTakeFromPool(PooledEnemy enemy)
    {
        enemy.transform.SetPositionAndRotation(_spawnPosition, _spawnRotation);
        enemy.gameObject.SetActive(true);
        enemy.OnSpawned();
    }
    private void OnReturnToPool(PooledEnemy enemy)
    {
        enemy.gameObject.SetActive(false);
    }
    private void OnDestroyEnemy(PooledEnemy enemy)
    {
        Destroy(enemy.gameObject);
    }
    private void ReturnEnemy(PooledEnemy enemy)
    {
        _pool.Release(enemy);
    }
}
