using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Trigger
{

    Command _onCommand;


    public Trigger(Command onCommand)
    {
        _onCommand = onCommand;
    }



    public void Fire()
    {
        _onCommand.Execute();
    }


}

