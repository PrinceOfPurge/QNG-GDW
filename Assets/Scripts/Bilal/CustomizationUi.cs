using System.Collections;
using System.Collections.Generic;
using UnityEngine;



    public class CustomizationUi : MonoBehaviour
    {
        
        [SerializeField] private CharCustomizer customizer;
        [SerializeField] private Color[] skinColors =
        {
            new Color(1.01f, 0.89f, 0.74f),
            new Color(0.95f, 0.77f, 0.59f),
            new Color(0.79f, 0.58f, 0.39f),
        };
        private InvokerCharSkinColor _invoker = new InvokerCharSkinColor();
    private void OnGUI()
    {
        GUILayout.BeginArea(new Rect(300, 10, 220, 200));
        GUILayout.Label("Choose Charecter Skin Colour >");
        Color oldbackground = GUI.backgroundColor;
        foreach (Color color in skinColors) {
            GUI.backgroundColor = color;
            if (GUILayout.Button("", GUILayout.Height(20)))
            {
                Command command = new ChangeColour(customizer, color);
                _invoker.ExecuteCommand(command);
            }
                }
        GUI.backgroundColor = oldbackground;

        if (GUILayout.Button("Redo")) _invoker.Redo();
        if (GUILayout.Button("Undo")) _invoker.Undo();
        GUILayout.EndArea();
    }
}


