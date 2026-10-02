using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EndlessRoad : MonoBehaviour
{
    [Header("Scroll Settings")]
    [SerializeField] private Renderer roadRenderer;
    [SerializeField] private float scrollSpeed = 0.8f;
    [SerializeField] private string texturePropertyName = "_MainTex";

    [Header("Road Length & Tiling")]
    [Tooltip("Increases how many times the road repeats down the length of the plane.")]
    [SerializeField] private float roadTilingY = 25f;

    private Vector2 currentOffset = Vector2.zero;
    private Material roadMaterial;

    private void Start()
    {
        if (roadRenderer == null)
            roadRenderer = GetComponent<Renderer>();

        roadMaterial = roadRenderer.material;

        // Set the Y-tiling so the road extends far down the horizon smoothly
        Vector2 currentTiling = roadMaterial.GetTextureScale(texturePropertyName);
        roadMaterial.SetTextureScale(texturePropertyName, new Vector2(currentTiling.x, roadTilingY));
    }

    private void Update()
    {
        // 1. Switched from currentOffset.x to currentOffset.y to fix sideways scrolling
        // 2. Used -= so the road flows backward away from the car (forward driving motion)
        currentOffset.y -= scrollSpeed * Time.deltaTime;
    
        roadMaterial.SetTextureOffset(texturePropertyName, currentOffset);
    }
}