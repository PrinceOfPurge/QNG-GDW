using UnityEngine;
using Cinemachine;
using System.Collections;

public class MainMenuCameraController : MonoBehaviour
{
    [Header("Cinemachine Cameras")]
    [SerializeField] private CinemachineVirtualCamera overviewCam;
    [SerializeField] private CinemachineVirtualCamera consoleCam;

    [Header("Start Prompt UI (Screen Space)")]
    [SerializeField] private CanvasGroup startPromptCanvasGroup;
    [SerializeField] private float promptFadeSpeed = 3.0f;

    [Header("Center Console Menu UI (World Space)")]
    [SerializeField] private CanvasGroup consoleUICanvasGroup;
    [SerializeField] private float uiFadeDelay = 1.0f;
    [SerializeField] private float uiFadeSpeed = 2.0f;

    private bool hasStarted = false;

    private void Start()
    {
        // Initial State: Start at Overview (Road view), Press Space to go to Console
        if (overviewCam != null) overviewCam.Priority = 10;
        if (consoleCam != null) consoleCam.Priority = 5;

        if (startPromptCanvasGroup != null)
        {
            startPromptCanvasGroup.alpha = 1f;
            startPromptCanvasGroup.interactable = true;
            startPromptCanvasGroup.blocksRaycasts = true;
        }

        if (consoleUICanvasGroup != null)
        {
            consoleUICanvasGroup.alpha = 0f;
            consoleUICanvasGroup.interactable = false;
            consoleUICanvasGroup.blocksRaycasts = false;
        }
    }

    private void Update()
    {
        if (!hasStarted && Input.GetKeyDown(KeyCode.Space))
        {
            StartMenuSequence();
        }
    }

    public void StartMenuSequence()
    {
        if (hasStarted) return;
        hasStarted = true;
        StartCoroutine(TransitionToConsoleSequence());
    }

    private IEnumerator TransitionToConsoleSequence()
    {
        if (startPromptCanvasGroup != null)
        {
            startPromptCanvasGroup.interactable = false;
            startPromptCanvasGroup.blocksRaycasts = false;

            while (startPromptCanvasGroup.alpha > 0f)
            {
                startPromptCanvasGroup.alpha -= Time.deltaTime * promptFadeSpeed;
                yield return null;
            }
        }

        // Blend camera down to console
        if (overviewCam != null) overviewCam.Priority = 5;
        if (consoleCam != null) consoleCam.Priority = 10;

        yield return new WaitForSeconds(uiFadeDelay);

        if (consoleUICanvasGroup != null)
        {
            while (consoleUICanvasGroup.alpha < 1f)
            {
                consoleUICanvasGroup.alpha += Time.deltaTime * uiFadeSpeed;
                yield return null;
            }

            consoleUICanvasGroup.interactable = true;
            consoleUICanvasGroup.blocksRaycasts = true;
        }
    }

    // Call this when the 3D button is pressed to clean up the console UI
    public void HideConsoleUI()
    {
        if (consoleUICanvasGroup != null)
        {
            consoleUICanvasGroup.alpha = 0f;
            consoleUICanvasGroup.interactable = false;
            consoleUICanvasGroup.blocksRaycasts = false;
        }
    }
}