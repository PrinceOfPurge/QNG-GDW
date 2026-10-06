using UnityEngine;
using Cinemachine;
using TMPro;
using System.Collections;
using FMODUnity;

public class IntroSequenceController : MonoBehaviour
{
    [System.Serializable]
    public struct DialogueLine
    {
        public string speakerName;
        [TextArea(2, 4)] public string lineText;
        public float displayDuration;
        public float delayBeforeNext;
        
        [Tooltip("Select the scene-specific FMOD event directly from your FMOD project tree")]
        public EventReference audioEvent; 
    }

    [Header("Cinemachine Cameras")]
    [SerializeField] private CinemachineVirtualCamera consoleCam;      // Priority 30 at launch (Main Menu view)
    [SerializeField] private CinemachineVirtualCamera roadOverviewCam; // Priority 10 at launch (Straight road view, LookAt = None)
    [SerializeField] private CinemachineVirtualCamera signGlanceCam;   // Priority 10 at launch (LookAt assigned to Sign Transform)

    [Header("Sign & Environment Setup")]
    [SerializeField] private PassingSignController signController;
    [SerializeField] private PassingTreeSpawner treeSpawner;
    [SerializeField] private float glanceDuration = 3.5f;

    [Header("UI Subtitles Setup")]
    [SerializeField] private TextMeshProUGUI subtitleTextUI;
    [SerializeField] private CanvasGroup subtitleCanvasGroup;
    [SerializeField] private float textFadeSpeed = 4.0f;

    [Header("Walkie-Talkie Dialogue Sequence")]
    [SerializeField] private DialogueLine[] dialogueLines;

    private bool sequenceStarted = false;

    private void Start()
    {
        // Force initial menu state: Camera locked on Console
        if (consoleCam != null) consoleCam.Priority = 30;
        if (roadOverviewCam != null) roadOverviewCam.Priority = 10;
        if (signGlanceCam != null) signGlanceCam.Priority = 10;

        // Hide subtitles initially
        if (subtitleCanvasGroup != null)
        {
            subtitleCanvasGroup.alpha = 0f;
        }

        if (dialogueLines == null || dialogueLines.Length == 0)
        {
            SetupDefaultScript();
        }
    }

    // Called strictly when the 3D Play button is clicked
    public void BeginIntroFromButton()
    {
        if (sequenceStarted) return;
        sequenceStarted = true;

        StartCoroutine(PlayIntroSequence());
    }

    private IEnumerator PlayIntroSequence()
    {
        // STEP 1: Transition from Console to Road Overview View
        if (consoleCam != null) consoleCam.Priority = 5;
        if (roadOverviewCam != null) roadOverviewCam.Priority = 20;

        yield return new WaitForSeconds(1.2f); // Wait for camera blend up to windshield to finish

        // STEP 2: Trigger sign & tree movement, switch camera focus to passing sign
        if (treeSpawner != null) treeSpawner.StartSpawningTrees();
        if (signController != null) signController.TriggerSignPass();

        if (signGlanceCam != null) signGlanceCam.Priority = 25; // Take control to track/zoom on sign

        // Hold camera focus on the sign as it approaches and passes
        yield return new WaitForSeconds(glanceDuration);

        // STEP 3: Transition camera back to Road Overview View
        if (signGlanceCam != null) signGlanceCam.Priority = 5;
        if (roadOverviewCam != null) roadOverviewCam.Priority = 30;

        yield return new WaitForSeconds(1.5f); // Pause briefly on road view before dialogue kicks in

        // STEP 4: Walkie-Talkie Dialogue Subtitle & Audio Sequence
        foreach (DialogueLine line in dialogueLines)
        {
            // Trigger dialogue audio if an event is assigned
            PlayDialogueAudio(line.audioEvent);

            string formattedText = string.IsNullOrEmpty(line.speakerName) 
                ? line.lineText 
                : $"<b>{line.speakerName}:</b> {line.lineText}";

            subtitleTextUI.text = formattedText;

            yield return StartCoroutine(FadeSubtitles(1f));
            yield return new WaitForSeconds(line.displayDuration);
            yield return StartCoroutine(FadeSubtitles(0f));
            yield return new WaitForSeconds(line.delayBeforeNext);
        }
    }

    private void PlayDialogueAudio(EventReference eventRef)
    {
        // Play one-shot dialogue audio if assigned in the Inspector
        if (!eventRef.IsNull)
        {
            RuntimeManager.PlayOneShot(eventRef);
        }
    }

    private IEnumerator FadeSubtitles(float targetAlpha)
    {
        if (subtitleCanvasGroup == null) yield break;

        while (!Mathf.Approximately(subtitleCanvasGroup.alpha, targetAlpha))
        {
            subtitleCanvasGroup.alpha = Mathf.MoveTowards(subtitleCanvasGroup.alpha, targetAlpha, Time.deltaTime * textFadeSpeed);
            yield return null;
        }
    }

    private void SetupDefaultScript()
    {
        dialogueLines = new DialogueLine[]
        {
            new DialogueLine { speakerName = "Officer Jackson", lineText = "Unit 2-Alpha to Control, or any unit near the square.", displayDuration = 3.0f, delayBeforeNext = 0.5f },
            new DialogueLine { speakerName = "Bailey Morgan", lineText = "This is 3-Bravo, go ahead 2-Alpha.", displayDuration = 2.0f, delayBeforeNext = 0.5f },
            new DialogueLine { speakerName = "Officer Jackson", lineText = "3-Bravo, we have a code 10-54 out here. Heavy localized fog, citizens unaccounted for. Requesting immediate backup at the city square.", displayDuration = 6.0f, delayBeforeNext = 0.5f },
            new DialogueLine { speakerName = "Bailey Morgan", lineText = "Copy that... did you say fog? Give me a break, Alpha.", displayDuration = 3.0f, delayBeforeNext = 0.5f },
            new DialogueLine { speakerName = "Officer Jackson", lineText = "It's real. We need boots on the groun—", displayDuration = 3.0f, delayBeforeNext = 0.2f },
            new DialogueLine { speakerName = "Officer Jackson", lineText = "[Radio Static]SHOTS FIRED! SHOTS FIRED!", displayDuration = 2.5f, delayBeforeNext = 0.3f },
            new DialogueLine { speakerName = "Bailey Morgan", lineText = "2-Alpha, status! 2-Alpha, come in!", displayDuration = 2.5f, delayBeforeNext = 0.3f },
            new DialogueLine { speakerName = "Officer Jackson", lineText = "*Growls* [Radio Static]... HELP!!!", displayDuration = 2.5f, delayBeforeNext = 1.0f }
        };
    }
}