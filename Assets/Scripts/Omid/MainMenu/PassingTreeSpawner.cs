using UnityEngine;
using System.Collections;

public class PassingTreeSpawner : MonoBehaviour
{
    [Header("Tree Prefabs & Spawning")]
    [SerializeField] private GameObject[] treePrefabs;
    [SerializeField] private float spawnInterval = 0.5f;

    [Header("Road Sides")]
    [SerializeField] private bool spawnBothSides = true;
    [SerializeField] private bool spawnSimultaneously = true;

    [Header("Spawn Position Settings")]
    [Tooltip("Independent X position for left side trees (must be negative to move left)")]
    [SerializeField] private float leftTreeXPosition = -18f; 

    [Tooltip("Independent X position for right side trees")]
    [SerializeField] private float rightTreeXPosition = 12f; 

    [Tooltip("Push back (e.g., 90 to 120) so trees spawn off-camera")]
    [SerializeField] private float spawnZ = 90f; 
    [SerializeField] private float destroyZ = -20f;
    [SerializeField] private float yHeightOffset = 0f;

    [Header("Seamless Spawning")]
    [SerializeField] private float scaleInDuration = 0.4f;

    [Header("Tree Speed & Transform Settings")]
    [SerializeField] private float treeSpeed = 25f; 
    [SerializeField] private float minTreeScale = 0.9f;
    [SerializeField] private float maxTreeScale = 1.2f;
    [SerializeField] private Vector3 spawnRotation = new Vector3(0f, 0f, 0f);

    private bool isSpawning = false;

    public void StartSpawningTrees()
    {
        if (isSpawning) return;
        isSpawning = true;
        StartCoroutine(SpawnRoutine());
    }

    public void StopSpawningTrees()
    {
        isSpawning = false;
    }

    private IEnumerator SpawnRoutine()
    {
        while (isSpawning)
        {
            if (treePrefabs != null && treePrefabs.Length > 0)
            {
                if (spawnBothSides)
                {
                    if (spawnSimultaneously)
                    {
                        SpawnSingleTree(leftTreeXPosition);  // Left side
                        SpawnSingleTree(rightTreeXPosition); // Right side
                    }
                    else
                    {
                        float xSide = Random.value > 0.5f ? rightTreeXPosition : leftTreeXPosition;
                        SpawnSingleTree(xSide);
                    }
                }
                else
                {
                    SpawnSingleTree(rightTreeXPosition);
                }
            }

            yield return new WaitForSeconds(spawnInterval);
        }
    }

    private void SpawnSingleTree(float xPos)
    {
        GameObject randomTree = treePrefabs[Random.Range(0, treePrefabs.Length)];

        float zJitter = Random.Range(-3f, 3f);
        Vector3 spawnPos = new Vector3(xPos, yHeightOffset, spawnZ + zJitter);
        Quaternion correctedRotation = Quaternion.Euler(spawnRotation);

        GameObject tree = Instantiate(randomTree, spawnPos, correctedRotation, transform);

        tree.transform.localScale = Vector3.zero;
        tree.SetActive(true);

        float targetScale = Random.Range(minTreeScale, maxTreeScale);
        StartCoroutine(MoveAndScaleTree(tree, targetScale));
    }

    private IEnumerator MoveAndScaleTree(GameObject tree, float targetScale)
    {
        float elapsed = 0f;
        Vector3 targetVectorScale = Vector3.one * targetScale;

        while (tree != null && tree.transform.position.z > destroyZ)
        {
            tree.transform.Translate(-Vector3.forward * treeSpeed * Time.deltaTime, Space.World);

            if (elapsed < scaleInDuration)
            {
                elapsed += Time.deltaTime;
                float t = Mathf.Clamp01(elapsed / scaleInDuration);
                tree.transform.localScale = Vector3.Lerp(Vector3.zero, targetVectorScale, t);
            }
            else
            {
                tree.transform.localScale = targetVectorScale;
            }

            yield return null;
        }

        if (tree != null)
        {
            Destroy(tree);
        }
    }
}