using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public class ClickInputHandler : MonoBehaviour
{
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            ResourceManager.Instance.UseResource("Bullet");
        }
        
        if (Input.GetMouseButtonDown(1))
        {
            ResourceManager.Instance.UseResource("Grenade");
        }
        
        if (Input.GetKeyDown(KeyCode.F))
        {
            ResourceManager.Instance.UseResource("MedPack");
        }
        
        if (Input.GetKeyDown(KeyCode.E))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit))
            {
                if (hit.collider.CompareTag("Collectible"))
                {
                    Resource resourceComponent = hit.collider.GetComponent<Resource>();

                    if (resourceComponent != null)
                    {
                        ICommand command = new CollectCommand(resourceComponent);
                        command.Execute();
                    }
                }
            }
        }
    }
}
