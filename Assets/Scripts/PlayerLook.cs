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

    private InputAction lookAction;

    private float xRotation = 0f;
    private float yRotation = 0f;

    private void Awake()
    {
        lookAction = new InputAction("Look", InputActionType.Value, "<Mouse>/delta");
        lookAction.Enable();
    }

    private void Update()
    {
        Look();
    }

    private void Look()
    {
        Vector2 lookInput = lookAction.ReadValue<Vector2>();

        float mouseX = lookInput.x * mouseSensitivity;
        float mouseY = lookInput.y * mouseSensitivity;

        // Vertical rotation (up and down)
        xRotation -= mouseY;
        xRotation = Mathf.Clamp(
            xRotation,
            -maxLookAngle,
            maxLookAngle
        );

        // Horizontal rotation (left and right)
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

    private void OnDestroy()
    {
        lookAction.Disable();
    }
}
