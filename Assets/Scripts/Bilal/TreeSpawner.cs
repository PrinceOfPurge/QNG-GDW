using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class TreeSpawner : MonoBehaviour
{
    public abstract BaseTree SpawnTree(Vector3 position);
}
