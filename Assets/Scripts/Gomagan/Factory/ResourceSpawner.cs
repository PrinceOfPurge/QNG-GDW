using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResourceSpawner : MonoBehaviour
{
    [SerializeField] private GameObject bulletPrefab;
    [SerializeField] private GameObject grenadePrefab;
    [SerializeField] private GameObject medpackPrefab;

    private int MAXRESOURCE = 5;
    private int MAXTIMER = 3;
    private int currentResourceCount = 0;
    private float timer = 0;

    private void Start()
    {
        for (int i = 0; i < MAXRESOURCE; i++)
        {
            SpawnRandomResource();
        }
    }

    private void Update()
    {
        if (currentResourceCount >= MAXRESOURCE) return;
        
        timer += Time.deltaTime;
        if (timer >= MAXTIMER)
        {
            timer = 0;
            SpawnRandomResource();
        }
    }
    
    private void SpawnRandomResource()
    {
        string[] types = { "Bullet", "Grenade", "MedPack" };
        int randomIndex = UnityEngine.Random.Range(0, types.Length);
        string chosenType = types[randomIndex];

        GameObject spawnedResource = Instantiate(GetResourcePrefab(chosenType), Vector3.zero, Quaternion.identity);
        var resourceComponent = spawnedResource.GetComponent<Resource>();
        resourceComponent.Init();
        currentResourceCount++;
    }
    
    private GameObject GetResourcePrefab(string resourceType)
    {
        GameObject resourceToSpawn;

        switch (resourceType)
        {
            case "Bullet":
                resourceToSpawn = bulletPrefab;
                break;
            case "Grenade":
                resourceToSpawn = grenadePrefab;
                break;
            case "MedPack":
                resourceToSpawn = medpackPrefab;
                break;
            default:
                resourceToSpawn = bulletPrefab;
                break;
        }

        return resourceToSpawn;
    }
}
