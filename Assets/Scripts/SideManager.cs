using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SideManager : MonoBehaviour
{
    public enum PlayerSide
    {
        Left,
        Right,
        Switching
    }

    public enum PlayerPosition
    {
        Seat,
        Window,
        Door
    }

    [Header("States")]
    public PlayerSide currentSide = PlayerSide.Left;
    public PlayerPosition currentPosition = PlayerPosition.Seat;

    [Header("Tutorial UI")]
    [SerializeField] private GameObject tutorialPrompt;

    [Header("Settings")]
    public float switchDuration = 0.75f;
    public float positionTransitionDuration = 0.25f;

    [Header("Truck Door Objects & Settings")]
    [SerializeField] private Transform leftDoorTransform;
    [SerializeField] private Transform rightDoorTransform;
    [SerializeField] private float doorOpenAngle = 55f; // Rotation angle when door opens
    [SerializeField] private float doorRotationSpeed = 4f;

    private Quaternion leftDoorClosedRot;
    private Quaternion rightDoorClosedRot;
    private Coroutine leftDoorCoroutine;
    private Coroutine rightDoorCoroutine;

    [Header("Animation Setup")]
    public Animator playerAnimator;
    public string slideLeftToRightTrigger = "SlideLeftToRight";
    public string slideRightToLeftTrigger = "SlideRightToLeft";

    [Header("Door Animation Triggers")]
    public string openLeftDoorTrigger = "OpenLeftDoor";
    public string closeLeftDoorTrigger = "CloseLeftDoor";
    public string openRightDoorTrigger = "OpenRightDoor";
    public string closeRightDoorTrigger = "CloseRightDoor";

    // Truck Movement Positions
    private Transform leftSeatPosition;
    private Transform leftWindowPosition;
    private Transform leftDoorPosition;
    private Transform rightSeatPosition;
    private Transform rightWindowPosition;
    private Transform rightDoorPosition;

    private PlayerControls controls;
    private Coroutine movementCoroutine;

    private GameObject truck;

    // Track input presses for tutorial completion
    private bool pressedSpace = false;
    private bool pressedQ = false;
    private bool pressedE = false;
    private bool tutorialCompleted = false;

    private void Awake()
    {
        controls = new PlayerControls();

        if (playerAnimator == null)
        {
            playerAnimator = GetComponentInChildren<Animator>();
        }
    }

    private void OnEnable()
    {
        controls.Enable();
    }

    private void OnDisable()
    {
        controls.Disable();
    }

    private void Start()
    {
        truck = GameObject.FindGameObjectWithTag("Truck");
        leftSeatPosition = truck.transform.Find("SeatPosLeft");
        leftDoorPosition = truck.transform.Find("DoorPosLeft");
        leftWindowPosition = truck.transform.Find("WindowPosLeft");
        rightSeatPosition = truck.transform.Find("SeatPosRight");
        rightDoorPosition = truck.transform.Find("DoorPosRight");
        rightWindowPosition = truck.transform.Find("WindowPosRight");

        // Cache initial door rotations
        if (leftDoorTransform != null) leftDoorClosedRot = leftDoorTransform.localRotation;
        if (rightDoorTransform != null) rightDoorClosedRot = rightDoorTransform.localRotation;

        if (tutorialPrompt != null)
        {
            tutorialPrompt.SetActive(true);
        }
    }

    private void Update()
    {
        // Handling controls
        if (controls.Player.SwitchSide.WasPressedThisFrame())
        {
            pressedSpace = true;
            CheckTutorialCompletion();
            SwitchSide();
        }

        // Q Key (Leftward Leaning Input)
        if (controls.Player.Window.WasPressedThisFrame())
        {
            pressedQ = true;
            CheckTutorialCompletion();

            if (currentSide == PlayerSide.Left)
                MoveToPosition(PlayerPosition.Window);
            else
                MoveToPosition(PlayerPosition.Door);
        }

        if (controls.Player.Window.WasReleasedThisFrame())
        {
            ReturnToSeat();
        }

        // E Key (Rightward Leaning Input)
        if (controls.Player.Door.WasPressedThisFrame())
        {
            pressedE = true;
            CheckTutorialCompletion();

            if (currentSide == PlayerSide.Left)
                MoveToPosition(PlayerPosition.Door);
            else
                MoveToPosition(PlayerPosition.Window);
        }

        if (controls.Player.Door.WasReleasedThisFrame())
        {
            ReturnToSeat();
        }
    }

    private void CheckTutorialCompletion()
    {
        if (tutorialCompleted)
            return;

        if (pressedSpace && pressedQ && pressedE)
        {
            tutorialCompleted = true;

            if (tutorialPrompt != null)
            {
                tutorialPrompt.SetActive(false);
            }

            Debug.Log("Tutorial completed! Disabling prompt.");
        }
    }

    public void SwitchSide()
    {
        if (currentSide == PlayerSide.Switching)
            return;

        if (currentPosition != PlayerPosition.Seat)
            return;

        StartCoroutine(SwitchSideCoroutine());
    }

    IEnumerator SwitchSideCoroutine()
    {
        PlayerSide previousSide = currentSide;
        currentSide = PlayerSide.Switching;

        if (FMODEvents.instance != null && AudioManager.instance != null)
        {
            AudioManager.instance.PlayOneShot(FMODEvents.instance.SeatSwitch, transform.position);
        }

        float elapsedTime = 0f;
        Vector3 startPosition = transform.position;
        Vector3 targetPosition;

        if (previousSide == PlayerSide.Left)
        {
            targetPosition = rightSeatPosition.position;
            if (playerAnimator != null) playerAnimator.SetTrigger(slideLeftToRightTrigger);
        }
        else
        {
            targetPosition = leftSeatPosition.position;
            if (playerAnimator != null) playerAnimator.SetTrigger(slideRightToLeftTrigger);
        }

        while (elapsedTime < switchDuration)
        {
            transform.position = Vector3.Lerp(startPosition, targetPosition, elapsedTime / switchDuration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        transform.position = targetPosition;

        currentSide = (previousSide == PlayerSide.Left) ? PlayerSide.Right : PlayerSide.Left;
        currentPosition = PlayerPosition.Seat;
    }

    private void MoveToPosition(PlayerPosition targetPositionType)
    {
        if (currentSide == PlayerSide.Switching || currentPosition != PlayerPosition.Seat)
            return;

        Transform targetPosition = GetPositionTransform(targetPositionType);
        if (targetPosition == null)
            return;

        // Trigger character animation & rotate truck door based on which side the player is on
        if (currentSide == PlayerSide.Left)
        {
            if (playerAnimator != null) playerAnimator.SetTrigger(openLeftDoorTrigger);
            // Positive Y rotation (+1f) for Left Door
            SetDoorState(leftDoorTransform, leftDoorClosedRot, true, 1f, ref leftDoorCoroutine);
        }
        else if (currentSide == PlayerSide.Right)
        {
            if (playerAnimator != null) playerAnimator.SetTrigger(openRightDoorTrigger);
            // Inverted sign (-1f) for Right Door so it opens outward
            SetDoorState(rightDoorTransform, rightDoorClosedRot, true, -1f, ref rightDoorCoroutine);
        }

        currentPosition = targetPositionType;
        MoveTo(targetPosition.position);
    }

    private void ReturnToSeat()
    {
        if (currentSide == PlayerSide.Switching || currentPosition == PlayerPosition.Seat)
            return;

        Transform seatPosition = GetSeatTransform();
        if (seatPosition == null)
            return;

        // Return character animation to idle & close the physical truck door
        if (currentSide == PlayerSide.Left)
        {
            if (playerAnimator != null) playerAnimator.SetTrigger(closeLeftDoorTrigger);
            SetDoorState(leftDoorTransform, leftDoorClosedRot, false, 1f, ref leftDoorCoroutine);
        }
        else if (currentSide == PlayerSide.Right)
        {
            if (playerAnimator != null) playerAnimator.SetTrigger(closeRightDoorTrigger);
            SetDoorState(rightDoorTransform, rightDoorClosedRot, false, -1f, ref rightDoorCoroutine);
        }

        currentPosition = PlayerPosition.Seat;
        MoveTo(seatPosition.position);
    }

    // Smoothly rotates the physical truck doors on the LOCAL Y-AXIS open or closed
    private void SetDoorState(Transform doorTransform, Quaternion closedRot, bool open, float directionSign, ref Coroutine doorCoroutine)
    {
        if (doorTransform == null) return;

        if (doorCoroutine != null)
            StopCoroutine(doorCoroutine);

        // Explicitly force rotation around the local Y-axis ONLY
        Quaternion targetRot = open ? closedRot * Quaternion.Euler(0f, doorOpenAngle * directionSign, 0f) : closedRot;
        doorCoroutine = StartCoroutine(RotateDoorCoroutine(doorTransform, targetRot));
    }

    private IEnumerator RotateDoorCoroutine(Transform doorTransform, Quaternion targetRotation)
    {
        while (Quaternion.Angle(doorTransform.localRotation, targetRotation) > 0.5f)
        {
            doorTransform.localRotation = Quaternion.Slerp(doorTransform.localRotation, targetRotation, Time.deltaTime * doorRotationSpeed);
            yield return null;
        }
        doorTransform.localRotation = targetRotation;
    }

    private Transform GetPositionTransform(PlayerPosition positionType)
    {
        if (currentSide == PlayerSide.Left)
        {
            switch (positionType)
            {
                case PlayerPosition.Window: return leftWindowPosition;
                case PlayerPosition.Door: return leftDoorPosition;
            }
        }
        else if (currentSide == PlayerSide.Right)
        {
            switch (positionType)
            {
                case PlayerPosition.Window: return rightWindowPosition;
                case PlayerPosition.Door: return rightDoorPosition;
            }
        }
        return null;
    }

    private Transform GetSeatTransform()
    {
        return (currentSide == PlayerSide.Left) ? leftSeatPosition : rightSeatPosition;
    }

    private void MoveTo(Vector3 targetPosition)
    {
        if (movementCoroutine != null)
        {
            StopCoroutine(movementCoroutine);
        }

        movementCoroutine = StartCoroutine(MoveToPositionCoroutine(targetPosition));
    }

    private IEnumerator MoveToPositionCoroutine(Vector3 targetPosition)
    {
        float elapsedTime = 0f;
        Vector3 startPosition = transform.position;

        while (elapsedTime < positionTransitionDuration)
        {
            transform.position = Vector3.Lerp(startPosition, targetPosition, elapsedTime / positionTransitionDuration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        transform.position = targetPosition;
        movementCoroutine = null;
    }
}