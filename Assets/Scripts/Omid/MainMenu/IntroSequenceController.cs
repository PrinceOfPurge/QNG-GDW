using UnityEngine;
using Cinemachine;
using TMPro;
using System.Collections;

public class IntroSequenceController : MonoBehaviour
{
    [System.Serializable]
    public struct DialogueLine
    {
        public string speakerName;
        [TextArea(2, 4)] public string lineText;
        public float displayDuration;
        public float delayBeforeNext;
        // Public string parameter to easily link FMOD event paths later
        public string fmodEventPath; 
    }

    [Header("Cinemachine Setup")]
    [SerializeField] private CinemachineVirtualCamera overviewCam;
    [SerializeField] private Transform signTransform;

    [Header("Sign Motion Settings")]
    [SerializeField] private PassingSignController signController;
    [SerializeField] private float glanceDuration = 2.5f; // Time camera tracks sign before returning to road

    [Header("UI Subtitles Setup")]
    [SerializeField] private TextMeshProUGUI subtitleTextUI;
    [SerializeField] private CanvasGroup subtitleCanvasGroup;
    [SerializeField] private float textFadeSpeed = 4.0f;

    [Header("Walkie-Talkie Dialogue Sequence")]
    [SerializeField] private DialogueLine[] dialogueLines;

    private void Start()
    {
        // Hide subtitles initially
        if (subtitleCanvasGroup != null)
        {
            subtitleCanvasGroup.alpha = 0f;
        }

        // Default dialogue setup matching your script
        if (dialogueLines == null || dialogueLines.Length == 0)
        {
            SetupDefaultScript();
        }

        // Start the intro flow
        StartCoroutine(PlayIntroSequence());
    }

    private IEnumerator PlayIntroSequence()
    {
        // --- STEP 1: GLANCE AT SIGN & LOOK BACK AT ROAD ---
        if (signTransform != null && overviewCam != null)
        {
            // Direct camera to look at passing sign
            overviewCam.LookAt = signTransform;
            
            if (signController != null)
            {
                signController.TriggerSignPass();
            }

            // Wait while sign approaches and is read
            yield return new WaitForSeconds(glanceDuration);

            // Clear LookAt so camera smoothly resets rotation to look straight down the road
            overviewCam.LookAt = null;
        }

        yield return new WaitForSeconds(1.5f); // Brief pause before radio kicks in

        // --- STEP 2: WALKIE-TALKIE SUBTITLE SEQUENCE ---
        foreach (DialogueLine line in dialogueLines)
        {
            // Format speaker text (e.g., "Cop on Walkie: Officer Morgan, come in.")
            string formattedText = string.IsNullOrEmpty(line.speakerName) 
                ? line.lineText 
                : $"<b>{line.speakerName}:</b> {line.lineText}";

            subtitleTextUI.text = formattedText;

            // TODO: Play FMOD audio event here when ready
            // if (!string.IsNullOrEmpty(line.fmodEventPath)) { FMODUnity.RuntimeManager.PlayOneShot(line.fmodEventPath); }

            // Fade Subtitle In
            yield return StartCoroutine(FadeSubtitles(1f));

            // Hold subtitle on screen
            yield return new WaitForSeconds(line.displayDuration);

            // Fade Subtitle Out
            yield return StartCoroutine(FadeSubtitles(0f));

            // Pause between lines
            yield return new WaitForSeconds(line.delayBeforeNext);
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
            new DialogueLine { speakerName = "Officer Jackson", lineText = "Officer Morgan, come in.", displayDuration = 2.0f, delayBeforeNext = 0.5f },
            new DialogueLine { speakerName = "Bailey Morgan", lineText = "Copy.", displayDuration = 1.2f, delayBeforeNext = 0.5f },
            new DialogueLine { speakerName = "Officer Jackson", lineText = "There’s reporting of a strange fog taking over the city and some citizens have gone missing, we need back up in the city square.", displayDuration = 5.0f, delayBeforeNext = 0.5f },
            new DialogueLine { speakerName = "Bailey Morgan", lineText = "A fog? Give me a break.", displayDuration = 2.2f, delayBeforeNext = 0.5f },
            new DialogueLine { speakerName = "Officer Jackson", lineText = "I know but things are hectic here, we need you out here… HOLY MOTHER OF GOD SHOTS FIRED!", displayDuration = 4.5f, delayBeforeNext = 0.2f },
            new DialogueLine { speakerName = "Bailey Morgan", lineText = "Cop 1, come in! Cop 1, come in!", displayDuration = 2.5f, delayBeforeNext = 0.3f },
            new DialogueLine { speakerName = "Officer Jackson", lineText = "*Monster sounds* HELP!!!", displayDuration = 2.5f, delayBeforeNext = 1.0f }
        };
    }
}