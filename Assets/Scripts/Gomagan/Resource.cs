using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Resource : MonoBehaviour
{
    [SerializeField] private string nam;
    
    public void Init()
    {
        float randomX = UnityEngine.Random.Range(-2f, 2f);
        float randomZ = UnityEngine.Random.Range(-2f, 2f);
        transform.position = new Vector3(randomX, .5f, randomZ);
    }
    
    public void Add()
    {
        bool added = ResourceManager.Instance.AddResource(nam);

        if (added)
        {
            print(nam + "Added");
            ResourceManager.Instance.currentResourceCount--;
            Destroy(gameObject);
        }
        else
        {
            print("Max" + nam + " Reached");
        }
    }

    public void Use()
    {
        bool used = ResourceManager.Instance.UseResource(nam);
        if (used)
        {
            print(nam + "Used");
        }
        else
        {
            print("No " + nam +" Left");
        }
    }
}
