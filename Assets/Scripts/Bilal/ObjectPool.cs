using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectPool : MonoBehaviour
{
    [SerializeField] public bool isEnabled = false;
    [SerializeField] public GameObject[] objects;
    [SerializeField] public Transform SpawnPoint;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void EnemySpawner()
    {
        Instantiate(objects[0]);
    }
}
