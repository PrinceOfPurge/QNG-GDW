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





    [SerializeField]
    private float MaxZombieTimer = 5;
    private float zombietimer = 0;


    [SerializeField]
    GameObject player;



    [SerializeField]
    GameObject basicZombieSpawnerPrefab;
    GameObject basicZombieSP;

    void Start()
    {
        if (player)
        {
            basicZombieSP = Instantiate(basicZombieSpawnerPrefab,transform);

        }
        

    }

    // Update is called once per frame
    void Update()
    {
        zombietimer -= Time.deltaTime;

        currentScene = SceneManager.GetActiveScene();

        if(currentScene.name == sceneName)
        {
            basicZombieSP.transform.position = player.transform.position + new Vector3(0,0,15);


            if (zombietimer <= 0)
            {
                BaseEnemy enemy = basicZombieSP.GetComponent<EnemySpawner>().SpawnEnemy();
                enemy.attack();

                zombietimer = MaxZombieTimer;
            }

            
        }

    }
}
