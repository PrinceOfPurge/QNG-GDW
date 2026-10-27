using System;
using System.IO;
using UnityEditor;
using UnityEngine;

public class ResourceCreator : EditorWindow
{
    string resourceName = "";
    GameObject prefab;

    [MenuItem("Tools/Resource Creator")]
    public static void ShowWindow()
    {
        GetWindow(typeof(ResourceCreator));
    }

    private void OnGUI()
    {
        GUILayout.Label("Create New Resource",  EditorStyles.boldLabel);
        resourceName = EditorGUILayout.TextField("Resource Name", resourceName);
        prefab = EditorGUILayout.ObjectField("Prefab", prefab, typeof(GameObject), true) as GameObject;

        if (GUILayout.Button("Create Resource"))
        {
            CreateNewResource();
        }
    }

    private void CreateNewResource()
    {
        if (string.IsNullOrEmpty(resourceName) || prefab == null)
        {
            Debug.LogError("Please enter a resource name and select a prefab.");
            return;
        }

        string folderPath = "Assets/Scripts/Gomagan/Prefabs";
        string localPath = Path.Combine(folderPath, resourceName +  ".prefab");
        
        GameObject instance = Instantiate(prefab);
        bool success;
        PrefabUtility.SaveAsPrefabAsset(instance, localPath, out success);
        DestroyImmediate(instance);

        if (success)
        {
            Debug.Log("Successfully created prefab at " + localPath);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }
        else
        {
            Debug.LogError("Failed to create prefab");
        }
    }
}
