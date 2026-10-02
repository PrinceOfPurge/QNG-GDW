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
        // 1. Initial State: Overview Cam active, Start Prompt visible
        overviewCam.Priority = 10;
        consoleCam.Priority = 5;

        if (startPromptCanvasGroup != null)
        {
            startPromptCanvasGroup.alpha = 1f;
            startPromptCanvasGroup.interactable = true;
            startPromptCanvasGroup.blocksRaycasts = true;
        }

        // 2. Hide Center Console UI initially
        if (consoleUICanvasGroup != null)
        {
            consoleUICanvasGroup.alpha = 0f;
            consoleUICanvasGroup.interactable = false;
            consoleUICanvasGroup.blocksRaycasts = false;
        }
    }

    private void Update()
    {
        // Listen for Spacebar press before game transition begins
        if (!hasStarted && Input.GetKeyDown(KeyCode.Space))
        {
            StartMenuSequence();
        }
    }

    public void StartMenuSequence()
    {
        hasStarted = true;
        StartCoroutine(TransitionToConsoleSequence());
    }

    private IEnumerator TransitionToConsoleSequence()
    {
        // Step A: Fade out "Press Space" text
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

        // Step B: Trigger Cinemachine camera blend
        overviewCam.Priority = 5;
        consoleCam.Priority = 10;

        // Step C: Wait for camera pan to finish before fading in console UI
        yield return new WaitForSeconds(uiFadeDelay);

        // Step D: Fade in diegetic console UI
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
}