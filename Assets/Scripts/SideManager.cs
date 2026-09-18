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

    public Vector3 leftPosition = new Vector3(-1, 0, 0);
    public Vector3 rightPosition = new Vector3(1, 0, 0);

    public PlayerSide currentSide = PlayerSide.Left;
    public float switchDuration = 0.75f;

    public void SwitchSide()
    {
        if (currentSide == PlayerSide.Switching)
            return;
        StartCoroutine(SwitchSideCoroutine());
    }

    IEnumerator SwitchSideCoroutine()
    {
        PlayerSide previousSide = currentSide;

        currentSide = PlayerSide.Switching;

        float elapsedTime = 0f;
        Vector3 startPosition = transform.position;
        Vector3 targetPosition = (previousSide == PlayerSide.Left) ? rightPosition : leftPosition;
        
        while (elapsedTime < switchDuration)
        {
            transform.position = Vector3.Lerp(startPosition, targetPosition, elapsedTime / switchDuration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        transform.position = targetPosition;
        
        currentSide = (previousSide == PlayerSide.Left) ? PlayerSide.Right : PlayerSide.Left;
    }


    public void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            SwitchSide();
        }
    }
}
