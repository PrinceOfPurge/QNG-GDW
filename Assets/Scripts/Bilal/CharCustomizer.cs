using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharCustomizer : MonoBehaviour
{
    [SerializeField] private Renderer charSkinRenderer;
    public Color SkinColor {  get; private set; }
    private void Awake()
    {
        SkinColor = charSkinRenderer.material.color;
    }
    public void SetCharSkinColor(Color color)
    {
        SkinColor = color;
        charSkinRenderer.material.color = color;
    }
}
