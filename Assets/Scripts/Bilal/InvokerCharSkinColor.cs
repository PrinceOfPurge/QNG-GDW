using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InvokerCharSkinColor 
{
    private Stack<Command> _undoStack = new Stack<Command>();
    private Stack<Command> _redoStack = new Stack<Command>();
    public void ExecuteCommand(Command command)
    {
        command.Execute();
        _undoStack.Push(command);
        _redoStack.Clear();
    }
    public void Undo()
    {
        if (_undoStack.Count == 0) return;
        Command command = _undoStack.Pop();
        command.Undo();
        _redoStack.Push(command);
    }
    public void Redo()
    {
        if (_redoStack.Count == 0) return;
        Command command = _redoStack.Pop();
        command.Execute();
        _undoStack.Push(command);
    }
}
