using System.Collections.Generic;
using UnityEngine;

// =====================================================================
// FACTORY METHOD PATTERN IN ONE FILE
// Products and creators are plain C# classes. Only TreeManager is a
// MonoBehaviour, so it is the only thing you attach in Unity.
// Don't add "using System;" here, it makes Random/Object ambiguous.
// =====================================================================

// ---------- ABSTRACT PRODUCT ----------
public abstract class BaseTree
{
    public GameObject TreeObject { get; private set; }

    protected BaseTree(GameObject treeObject)
    {
        TreeObject = treeObject;
    }

    public abstract string NameOfTree { get; }
    public abstract void Grow();
}

// ---------- CONCRETE PRODUCTS ----------
public class OakTree : BaseTree
{
    public OakTree(GameObject treeObject) : base(treeObject) { }

    public override string NameOfTree => "Oak Tree";

    public override void Grow()
    {
        float size = Random.Range(1.0f, 2.0f);
        TreeObject.transform.localScale = Vector3.one * size;
    }
}

public class PineTree : BaseTree
{
    public PineTree(GameObject treeObject) : base(treeObject) { }

    public override string NameOfTree => "Pine Tree";

    public override void Grow()
    {
        float size = Random.Range(2.0f, 3.0f);
        TreeObject.transform.localScale = Vector3.one * size;
    }
}

// ---------- ABSTRACT CREATOR (declares the factory method) ----------
public abstract class TreeSpawner
{
    public abstract BaseTree SpawnTree(Vector3 position);
}

// ---------- CONCRETE CREATORS (each decides which tree to build) ----------
public class OakTreeSpawner : TreeSpawner
{
    private readonly GameObject _oakTreePrefab;

    public OakTreeSpawner(GameObject oakTreePrefab)
    {
        _oakTreePrefab = oakTreePrefab;
    }

    public override BaseTree SpawnTree(Vector3 position)
    {
        Quaternion randomYaw = Quaternion.Euler(0f, Random.Range(0f, 360f), 0f);
        GameObject treeObject = Object.Instantiate(_oakTreePrefab, position, randomYaw);
        return new OakTree(treeObject);
    }
}

public class PineTreeSpawner : TreeSpawner
{
    private readonly GameObject _pineTreePrefab;

    public PineTreeSpawner(GameObject pineTreePrefab)
    {
        _pineTreePrefab = pineTreePrefab;
    }

    public override BaseTree SpawnTree(Vector3 position)
    {
        Quaternion randomYaw = Quaternion.Euler(0f, Random.Range(0f, 360f), 0f);
        GameObject treeObject = Object.Instantiate(_pineTreePrefab, position, randomYaw);
        return new PineTree(treeObject);
    }
}

// ---------- CLIENT (the only MonoBehaviour) ----------
public class TreeManager : MonoBehaviour
{
    [SerializeField] private GameObject oakTreePrefab;
    [SerializeField] private GameObject pineTreePrefab;

    private int startingNumOfTrees = 15;
    private float spawnRadiusOfTrees = 25f;

    private List<TreeSpawner> tSpawners;

    private void Awake()
    {
        // The client only ever holds TreeSpawner references
        tSpawners = new List<TreeSpawner>
        {
            new OakTreeSpawner(oakTreePrefab),
            new PineTreeSpawner(pineTreePrefab)
        };
    }

    private void Start()
    {
        if (oakTreePrefab == null || pineTreePrefab == null)
        {
            Debug.LogWarning("TreeManager: assign both tree prefabs in the Inspector.");
            return;
        }

        for (int i = 0; i < startingNumOfTrees; i++)
        {
            SpawnArandomTree();
        }
    }

    private void Update()
    {
        // GetKeyDown = one tree per key press (GetKey would spawn one every frame)
        if (Input.GetKeyDown(KeyCode.T))
        {
            SpawnArandomTree();
        }
    }

    private void SpawnArandomTree()
    {
        TreeSpawner spawner = tSpawners[Random.Range(0, tSpawners.Count)];
        BaseTree tree = spawner.SpawnTree(GetRandomPosition());
        if (tree != null) { tree.Grow(); }
    }

    private Vector3 GetRandomPosition()
    {
        Vector2 offset = Random.insideUnitCircle * spawnRadiusOfTrees;
        return transform.position + new Vector3(offset.x, 0f, offset.y);
    }
}