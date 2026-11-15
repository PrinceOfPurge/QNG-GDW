using UnityEngine;
using UnityEngine.Pool;

// Spawns objects continuously. Toggle 'usePooling' in the Inspector BEFORE pressing Play
// to profile the "before" (Instantiate/Destroy) and "after" (object pool) versions.
public class ObjectSpawner : MonoBehaviour
{
    [Header("What to spawn")]
    [SerializeField] private TimedObject prefab;

    [Header("Mode")]
    [SerializeField] private bool usePooling = false;

    [Header("Spawning")]
    [SerializeField] private float spawnsPerSecond = 60f;
    [SerializeField] private float spawnRadius = 5f;

    [Header("Pool limits")]
    [SerializeField] private int defaultCapacity = 50;
    [SerializeField] private int maxPoolSize = 200; // keep this above spawnsPerSecond x lifetime

    private ObjectPool<TimedObject> _pool;
    private float _accumulator;

    private void Awake()
    {
        if (usePooling)
        {
            _pool = new ObjectPool<TimedObject>(
                CreatePooledItem,    // make a new instance when the pool is empty
                OnTakeFromPool,      // activate it when taken
                OnReturnedToPool,    // DISABLE it when returned (not destroyed)
                OnDestroyPoolObject, // only destroys extras beyond maxPoolSize
                true,                // collection check: catches double releases
                defaultCapacity,
                maxPoolSize);
        }
        if (usePooling)
        {
            InitPool();
        }
    }

    private void Update()
    {
        // Spawn at a steady rate regardless of frame rate
        _accumulator += spawnsPerSecond * Time.deltaTime;
        while (_accumulator >= 1f)
        {
            _accumulator -= 1f;
            Spawn();
        }
    }

    private void Spawn()
    {
        Vector3 position = transform.position + Random.insideUnitSphere * spawnRadius;

        if (usePooling)
        {
            // Hard cap: never more than maxPoolSize objects alive at once
            if (_pool.CountActive >= maxPoolSize) return;

            TimedObject obj = _pool.Get();
            obj.transform.position = position;
        }
        else
        {
            // BEFORE pooling: allocate a new object; TimedObject destroys itself later
            Instantiate(prefab, position, Quaternion.identity);
        }
    }

    // ---- Pool callbacks ----------------------------------------------------

    private TimedObject CreatePooledItem()
    {
        TimedObject obj = Instantiate(prefab);
        obj.OnExpired = ReturnToPool; // when its lifetime ends it recycles itself
        return obj;
    }

    private void ReturnToPool(TimedObject obj)
    {
        _pool.Release(obj);
    }

    private void OnTakeFromPool(TimedObject obj)
    {
        obj.gameObject.SetActive(true);
    }

    private void OnReturnedToPool(TimedObject obj)
    {
        obj.gameObject.SetActive(false);
    }

    private void OnDestroyPoolObject(TimedObject obj)
    {
        Destroy(obj.gameObject);
    }

    private void InitPool()
    {
        if (_pool != null) return;

        _pool = new ObjectPool<TimedObject>(
            CreatePooledItem,    // make a new instance when the pool is empty
            OnTakeFromPool,      // activate it when taken
            OnReturnedToPool,    // DISABLE it when returned (not destroyed)
            OnDestroyPoolObject, // only destroys extras beyond maxPoolSize
            true,                // collection check: catches double releases
            defaultCapacity,
            maxPoolSize);
    }
}