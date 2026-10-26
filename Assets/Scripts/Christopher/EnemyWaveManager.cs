using System.Collections;
using System.Collections.Generic;
using Chapter.Singleton;
using UnityEngine;
using UnityEngine.SceneManagement;

public class EnemyWaveManager : Singleton<EnemyWaveManager>
{
    // Start is called before the first frame update
    Scene currentScene;
    private string sceneName = "Prototype";

    private List<GameObject> EnemyList;
    public int maxEnemyCount = 5;


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

        currentScene = SceneManager.GetActiveScene();

        if(currentScene.name == sceneName)
        {
            basicZombieSP.transform.position = player.transform.position + new Vector3(15,0,10);
            largeZombieSP.transform.position = player.transform.position + new Vector3(-15, 0, 10);

            if (zombietimer <= 0)
            {
                BaseEnemy enemy = basicZombieSP.GetComponent<EnemySpawner>().SpawnEnemy();
                BaseEnemy largeEnemy = largeZombieSP.GetComponent<EnemySpawner>().SpawnEnemy();
                enemy.attack();
                largeEnemy.attack();

                


                zombietimer = MaxZombieTimer;
            }

            
        }

    }
}
