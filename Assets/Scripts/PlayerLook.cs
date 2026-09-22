using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerLook : MonoBehaviour
{
    [SerializeField] private Transform playerBody;

    [SerializeField] private float mouseSensitivity = 0.1f;
    [SerializeField] private float maxLookAngle = 80f;
    [SerializeField] private float maxLookHorizontal = 90f;

    private PlayerControls controls;

    private float xRotation = 0f;
    private float yRotation = 0f;

    private void Awake()
    {
        controls = new PlayerControls();
    }

    //Enable and disable the controls
    private void OnEnable()
    {
        controls.Enable();
    }
    private void OnDisable()
    {
        controls.Disable();
    }

    private void Update()
    {
        Look();
    }

    //Handle Look movement
    private void Look()
    {
        Vector2 lookInput = controls.Player.Look.ReadValue<Vector2>();

        float mouseX = lookInput.x * mouseSensitivity;
        float mouseY = lookInput.y * mouseSensitivity;

        // Vertical rotation
        xRotation -= mouseY;
        xRotation = Mathf.Clamp(
            xRotation,
            -maxLookAngle,
            maxLookAngle
        );

        // Horizontal rotation
        yRotation += mouseX;
        yRotation = Mathf.Clamp(
            yRotation,
            -maxLookHorizontal,
            maxLookHorizontal
        );

        // Apply both rotations
        transform.localRotation = Quaternion.Euler(
            xRotation,
            yRotation,
            0f
        );
    }
}
