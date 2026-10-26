using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Resource : MonoBehaviour
{
    private string nam = "null";
    
    public void Init(string resource)
    {
        nam = resource;
    }
    
    public void Add()
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

    public void Use()
    {
        bool added = ResourceManager.Instance.AddResource(nam);

        if (added)
        {
            print(nam + "Added");
        }
        else
        {
            print("Max" + nam + " Reached");
        }
    }
}
