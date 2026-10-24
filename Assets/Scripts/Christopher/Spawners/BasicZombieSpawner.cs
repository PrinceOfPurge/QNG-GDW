using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasicZombieSpawner : EnemySpawner
{

    [SerializeField]
    GameObject basicZombie;


    public override BaseEnemy SpawnEnemy()
    {
        GameObject zom = Instantiate(basicZombie, transform);
        return zom.GetComponent<BaseEnemy>();
    }





}
