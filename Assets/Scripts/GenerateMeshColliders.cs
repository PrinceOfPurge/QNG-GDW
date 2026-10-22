using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class GenerateMeshColliders : MonoBehaviour
{
    [ContextMenu("Add Mesh Colliders To Child Meshes")]
    private void AddMeshCollidersToChildMeshes()
    {
        MeshFilter[] meshFilters =
            GetComponentsInChildren<MeshFilter>(true);

        int added = 0;
        int skipped = 0;

        foreach (MeshFilter meshFilter in meshFilters)
        {
            // Only use objects that have a real mesh
            if (meshFilter.sharedMesh == null ||
                meshFilter.GetComponent<Collider>() != null)
            {
                skipped++;
                continue;
            }

            MeshCollider meshCollider;

#if UNITY_EDITOR
            meshCollider = Undo.AddComponent<MeshCollider>(
                meshFilter.gameObject);
#else
            meshCollider = meshFilter.gameObject
                .AddComponent<MeshCollider>();
#endif

            // Makes the collider follow the actual mesh
            meshCollider.sharedMesh = meshFilter.sharedMesh;

            added++;
        }

        //Debug.Log(
           // $"Mesh collider setup complete. Added: {added}. " +
           // $"Skipped: {skipped}.",
           // this);
    }
}

