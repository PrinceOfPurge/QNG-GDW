using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerLook : MonoBehaviour
{
    [Header("Bone Target")]
    [Tooltip("Assign the Spine or Chest bone here (e.g., mixamorig:Spine1 or Bip01 Spine)")]
    [SerializeField] private Transform upperSpineBone;

    [Header("Sensitivity & Limits")]
    [SerializeField] private float mouseSensitivity = 0.1f;
    [SerializeField] private float maxLookUpAngle = 50f;
    [SerializeField] private float maxLookDownAngle = 80f;
    [SerializeField] private float maxLookHorizontal = 90f;
    [SerializeField] private float maxSpineTwistAngle = 60f; // Prevents the spine from snapping backward

    [Header("References")]
    public SideManager sideManager;

    private PlayerControls controls;

    private float xRotation = 0f;
    private float yRotation = 0f;

    private bool canLook = true;

    private void Awake()
    {
        controls = new PlayerControls();
    }

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
        if (canLook)
            Look();
    }

    public void LockLook()
    {
        canLook = false;
    }

    private void Look()
    {
        Vector2 lookInput = controls.Player.Look.ReadValue<Vector2>();

        float mouseX = lookInput.x * mouseSensitivity;
        float mouseY = lookInput.y * mouseSensitivity;

        // 1. Vertical Looking (Pitch)
        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -maxLookUpAngle, maxLookDownAngle);

        // 2. Horizontal Looking (Yaw)
        yRotation += mouseX;

        // Clamp horizontal look limits based on side and door status
        if (sideManager != null && sideManager.currentPosition == SideManager.PlayerPosition.Door)
        {
            if (sideManager.currentSide == SideManager.PlayerSide.Right)
            {
                yRotation = Mathf.Clamp(yRotation, (180f + -maxLookHorizontal), (90f + maxLookHorizontal));
            }
            else if (sideManager.currentSide == SideManager.PlayerSide.Left)
            {
                yRotation = Mathf.Clamp(yRotation, (-90f + -maxLookHorizontal), (-180f + maxLookHorizontal));
            }
        }
        else
        {
            yRotation = Mathf.Clamp(yRotation, -maxLookHorizontal, maxLookHorizontal);
        }

        // Apply Pitch to local camera transform
        transform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
    }

    // LateUpdate runs AFTER animations play, preventing the Animator from overwriting bone rotations
    private void LateUpdate()
    {
        if (!canLook || upperSpineBone == null)
            return;

        // Clamp the spine twist so the character's upper body doesn't rotate unnatural degrees
        float clampedSpineYaw = Mathf.Clamp(yRotation, -maxSpineTwistAngle, maxSpineTwistAngle);

        // Twist the upper body spine bone horizontally to align hands and gun with crosshair
        upperSpineBone.localRotation *= Quaternion.Euler(0f, clampedSpineYaw, 0f);
    }
}