using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChangeColour : Command
{
    private CharCustomizer _receiver;
    private Color _newColor;
    private Color _oldColor;
    public ChangeColour(CharCustomizer receiver, Color newColor)
    {
        _receiver = receiver;
        _newColor = newColor;
        
    }
    public override void Execute()
    {
        _oldColor = _receiver.SkinColor;
        _receiver.SetCharSkinColor(_newColor);
    }
    public override void Undo()
    {
        _receiver.SetCharSkinColor(_oldColor);
    }
}
