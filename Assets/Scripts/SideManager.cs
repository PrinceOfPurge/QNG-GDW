using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SideManager : MonoBehaviour
{
    //States for player side
    public enum PlayerSide
    {
        Left,
        Right,
        Switching
    }

    //States for player position
    public enum PlayerPosition
    {
        Seat,
        Window,
        Door
    }

    [Header("Left Positions")]
    public Transform leftSeatPosition;
    public Transform leftWindowPosition;
    public Transform leftDoorPosition;

    [Header("Right Positions")]
    public Transform rightSeatPosition;
    public Transform rightWindowPosition;
    public Transform rightDoorPosition;

    [Header("States")]
    public PlayerSide currentSide = PlayerSide.Left;
    public PlayerPosition currentPosition = PlayerPosition.Seat;

    [Header("Settings")]
    public float switchDuration = 0.75f;
    public float positionTransitionDuration = 0.25f;

    private PlayerControls controls;
    private Coroutine movementCoroutine;

    private void Awake()
    {
        controls = new PlayerControls();
    }

    //Disable and enable control scheme
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
        //Handling controls
        if (controls.Player.SwitchSide.WasPressedThisFrame())
        {
            SwitchSide();
        }

        if(controls.Player.Window.WasPressedThisFrame())
        {
            MoveToPosition(PlayerPosition.Window);
        }

        if (controls.Player.Window.WasReleasedThisFrame())
        {
            ReturnToSeat();
        }

        if (controls.Player.Door.WasPressedThisFrame())
        {
            MoveToPosition(PlayerPosition.Door);
        }

        if (controls.Player.Door.WasReleasedThisFrame())
        {
            ReturnToSeat();
        }
    }



    //Switching from left and right seats 
    public void SwitchSide()
    {
        if (currentSide == PlayerSide.Switching)
            return;

        if (currentPosition != PlayerPosition.Seat)
            return;

        StartCoroutine(SwitchSideCoroutine());
    }

    //Coroutine for switching seats
    IEnumerator SwitchSideCoroutine()
    {
        PlayerSide previousSide = currentSide;

        currentSide = PlayerSide.Switching;

        float elapsedTime = 0f;
        Vector3 startPosition = transform.position;
        Vector3 targetPosition;

        if (previousSide == PlayerSide.Left)
        {
            targetPosition = rightSeatPosition.position;
        }
        else
        {
            targetPosition = leftSeatPosition.position;
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

    //moving from positions (door or window)
    private void MoveToPosition(PlayerPosition targetPositionType)
    {
        if (currentSide == PlayerSide.Switching)
            return;

        if (currentPosition != PlayerPosition.Seat)
            return;

        Transform targetPosition = GetPositionTransform(targetPositionType);

        if (targetPosition == null)
            return;

        currentPosition = targetPositionType;

        MoveTo(targetPosition.position);
    }

    //Moving from a position (door or window) back to seat
    private void ReturnToSeat()
    {
        if (currentSide == PlayerSide.Switching)
            return;

        if (currentPosition == PlayerPosition.Seat)
            return;

        Transform seatPosition = GetSeatTransform();

        if (seatPosition == null)
            return;

        currentPosition = PlayerPosition.Seat;

        MoveTo(seatPosition.position);
    }

    //Get the position transform to move the player to
    private Transform GetPositionTransform(PlayerPosition positionType)
    {
        if (currentSide == PlayerSide.Left)
        {
            switch (positionType)
            {
                case PlayerPosition.Window:
                    return leftWindowPosition;

                case PlayerPosition.Door:
                    return leftDoorPosition;
            }
        }

        else if (currentSide == PlayerSide.Right)
        {
            switch (positionType)
            {
                case PlayerPosition.Window:
                    return rightWindowPosition;

                case PlayerPosition.Door:
                    return rightDoorPosition;
            }
        }

        return null;
    }

    //Get the transform for the seat the player is going to
    private Transform GetSeatTransform()
    {
        if (currentSide == PlayerSide.Left)
        {
            return leftSeatPosition;
        }

        if (currentSide == PlayerSide.Right)
        {
            return rightSeatPosition;
        }

        return null;
    }

    //check if we should start the corourine for moving positions
    private void MoveTo(Vector3 targetPosition)
    {
        if (movementCoroutine != null)
        {
            StopCoroutine(movementCoroutine);
        }

        movementCoroutine = StartCoroutine(MoveToPositionCoroutine(targetPosition)
        );
    }

    //Coroutine for moving between positions
    private IEnumerator MoveToPositionCoroutine(Vector3 targetPosition)
    {
        float elapsedTime = 0f;

        Vector3 startPosition = transform.position;

        while (elapsedTime < positionTransitionDuration)
        {
            transform.position = Vector3.Lerp(startPosition,targetPosition,elapsedTime / positionTransitionDuration);

            elapsedTime += Time.deltaTime;

            yield return null;
        }

        transform.position = targetPosition;

        movementCoroutine = null;
    }
}
