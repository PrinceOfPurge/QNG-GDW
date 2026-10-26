using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OakTreeSpawner : TreeSpawner
{
    [SerializeField] private GameObject oakTreePrefab;
    public override BaseTree SpawnTree(Vector3 position)
    {
        Quaternion randomYaw = Quaternion.Euler(0f, Random.Range(1f, 320f), 0f);
        GameObject treeObject = Instantiate(oakTreePrefab, position, randomYaw);
        return treeObject.GetComponent<BaseTree>();
    }
}
