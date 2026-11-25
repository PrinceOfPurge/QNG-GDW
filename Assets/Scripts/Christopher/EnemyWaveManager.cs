using System;
using System.Collections;
using System.Collections.Generic;
using Chapter.Singleton;
using UnityEngine;
using UnityEngine.SceneManagement;

public class EnemyWaveManager : Singleton<EnemyWaveManager>
{

    public GameObject player; 

    public GameObject EnemyPrefab;
    public int poolSize = 100;
    private Queue<GameObject> pool = new Queue<GameObject>();


    public float spawnTimerMax = 5f;
    float spawnTimer;

    void Start()
    {

        for (int i = 0; i < poolSize; i++)
        {
            GameObject enemy = Instantiate(EnemyPrefab);
            enemy.SetActive(false);  
            pool.Enqueue(enemy);      // Add to queue
        }
    }



    private void Update()
    {
        spawnTimer -= Time.deltaTime;


        if(spawnTimer<=0)
        {
            GameObject enemy = GetEnemy();
            enemy.transform.position = player.transform.position + new Vector3(0, 0, 10);

            spawnTimer = spawnTimerMax;
        }
    }



    public GameObject GetEnemy()
    {
        if (pool.Count > 0)
        {
            GameObject enemy = pool.Dequeue();
            enemy.SetActive(true);
            return enemy;
        }
        else
        {
            return Instantiate(EnemyPrefab);
        }
    }
    public void ReturnEnemy(GameObject enemy)
    {
        enemy.SetActive(false);
        pool.Enqueue(enemy);
    }


}
