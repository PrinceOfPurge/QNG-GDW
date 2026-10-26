using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LargeZombieSpawner : EnemySpawner
{

    [SerializeField]
    GameObject LargeZombie;


    public override BaseEnemy SpawnEnemy()
    {
        GameObject zom = Instantiate(LargeZombie, transform);
        return zom.GetComponent<BaseEnemy>();
    }


}
