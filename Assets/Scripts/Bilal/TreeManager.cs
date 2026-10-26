
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TreeManager : MonoBehaviour
{
    [SerializeField] private TreeSpawner[] tSpawners;
    private int startingNumOfTrees = 15;
    private float spawnRadiusOfTrees = 25;
    private void Start()
    {
        for(int i = 0; i <startingNumOfTrees; i++)
        {
            SpawnArandomTree();
        }
    }
    private void Update()
    {
        if (Input.GetKey(KeyCode.T))
        {
            SpawnArandomTree();
        }
    }
    private void SpawnArandomTree()
    {
        TreeSpawner spawner = tSpawners[Random.Range(0, tSpawners.Length)];
        BaseTree tree = spawner.SpawnTree(GetRandomPosition());
        if (tree != null) { tree.Grow(); }
    }
    private Vector3 GetRandomPosition()
    {
        Vector2 offset = Random.insideUnitCircle * spawnRadiusOfTrees;
        Vector3 position = transform.position + new Vector3(offset.x, 0f, offset.y);
        return position;
    }
}
