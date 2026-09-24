using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarJuice : MonoBehaviour
{
    [Header("Engine Vibration")]
    [SerializeField] private float vibrationIntensity = 0.02f;
    [SerializeField] private float vibrationSpeed = 25f;

    [Header("Sway Motion")]
    [SerializeField] private float swayAmount = 0.5f;
    [SerializeField] private float swaySpeed = 1.5f;

    private Vector3 initialPosition;

    private void Start()
    {
        initialPosition = transform.localPosition;
    }

    private void Update()
    {
        // Micro-vibrations for engine idle
        float offsetY = Mathf.Sin(Time.time * vibrationSpeed) * vibrationIntensity;
        
        // Slight horizontal drift/sway
        float offsetX = Mathf.Sin(Time.time * swaySpeed) * swayAmount * 0.05f;

        transform.localPosition = initialPosition + new Vector3(offsetX, offsetY, 0f);
    }
}
