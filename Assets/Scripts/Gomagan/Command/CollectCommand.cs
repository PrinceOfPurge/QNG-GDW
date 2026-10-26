using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollectCommand : ICommand
{
    private Resource resource;
    
    public CollectCommand(Resource resource)
    {
        this.resource = resource;
    }

    public void Execute()
    {
        resource.Add();
    }
}
