using System;
using System.Collections;
using System.Collections.Generic;
using Chapter.Singleton;
using UnityEngine;
using UnityEngine.SceneManagement;

public class EnemyWaveManager : Singleton<EnemyWaveManager>
{

    private Array EnemyList;
    public int maxEnemyCount = 5;
    private int currentEnemyCount;


    [SerializeField]
    private float MaxZombieTimer = 5;
    private float zombietimer = 0;


    [SerializeField]
    GameObject player;



    [SerializeField]
    GameObject basicZombieSpawnerPrefab;
    GameObject basicZombieSP;

    [SerializeField]
    GameObject largeZombieSpawnerPrefab;
    GameObject largeZombieSP;

    void Start()
    {
        if (player)
        {
            basicZombieSP = Instantiate(basicZombieSpawnerPrefab,transform);


            largeZombieSP = Instantiate(largeZombieSpawnerPrefab, transform);

        }
        

    }

    // Update is called once per frame
    void Update()
    {
        zombietimer -= Time.deltaTime;

        basicZombieSP.transform.position = player.transform.position + new Vector3(15,0,10);
        largeZombieSP.transform.position = player.transform.position + new Vector3(-15, 0, 10);
        currentEnemyCount = GameObject.FindGameObjectsWithTag("Enemy").Length;

        if (currentEnemyCount < maxEnemyCount)
        {
            if (zombietimer <= 0)
            {
                BaseEnemy enemy = basicZombieSP.GetComponent<EnemySpawner>().SpawnEnemy();
                BaseEnemy largeEnemy = largeZombieSP.GetComponent<EnemySpawner>().SpawnEnemy();
                enemy.attack();
                largeEnemy.attack();


                
                EnemyList = GameObject.FindGameObjectsWithTag("Enemy");



                zombietimer = MaxZombieTimer;
            }
        }



            
    }

    
}
