using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputInvoker 
{

    Stack<Command> commandList;



    public InputInvoker()
    {
        commandList = new Stack<Command>();
    }


    public void AddCommand(Command newCommand)
    {
        newCommand.Execute();
        commandList.Push(newCommand);
    }
}
