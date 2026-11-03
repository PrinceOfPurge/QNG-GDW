using UnityEngine;
using System.Collections;

public class ConsoleButton3D : MonoBehaviour
{
    [Header("3D Model Setup")]
    [SerializeField] private Transform buttonCapTransform;
    [SerializeField] private Vector3 pressedOffsetLocal = new Vector3(0f, 0f, -0.02f); // Local Z-axis depress
    [SerializeField] private float pressSpeed = 10f;

    [Header("Sequence Trigger")]
    [SerializeField] private IntroSequenceController introController;

    private Vector3 initialLocalPos;
    private bool isPressed = false;

    private void Start()
    {
        if (buttonCapTransform == null) buttonCapTransform = transform;
        initialLocalPos = buttonCapTransform.localPosition;
    }

    private void OnMouseDown()
    {
        if (isPressed) return;
        isPressed = true;
        StartCoroutine(AnimateButtonPress());
    }

    private IEnumerator AnimateButtonPress()
    {
        Vector3 targetPos = initialLocalPos + pressedOffsetLocal;

        // Push button back along its local Z axis
        while (Vector3.Distance(buttonCapTransform.localPosition, targetPos) > 0.0005f)
        {
            buttonCapTransform.localPosition = Vector3.MoveTowards(
                buttonCapTransform.localPosition, 
                targetPos, 
                Time.deltaTime * pressSpeed
            );
            yield return null;
        }

        // Trigger intro sequence flow
        if (introController != null)
        {
            introController.BeginIntroFromButton();
        }
    }
}