using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class PlayerPortraitAnimator : MonoBehaviour
{
    [Header("Target UI Image")]
    [SerializeField] private Image portraitImage;

    [Header("Idle Sprites")]
    [SerializeField] private Sprite idleOpenEye;
    [SerializeField] private Sprite idleBlink;

    [Header("Hit Struggle Sprites (Played in Order)")]
    [SerializeField] private Sprite hitRed;
    [SerializeField] private Sprite hitWhite;
    [SerializeField] private Sprite hitClear;

    [Header("Optional Dead / Critical Sprite")]
    [SerializeField] private Sprite deadOrLowHealthSprite;

    [Header("Blink Timing")]
    [SerializeField] private float minBlinkInterval = 2.5f;
    [SerializeField] private float maxBlinkInterval = 5.5f;
    [SerializeField] private float blinkDuration = 0.14f;

    [Header("Hit Flash Timing")]
    [SerializeField] private float hitFrameDuration = 0.09f; // Time per Red -> White -> Clear frame

    private Coroutine hitRoutine;
    private bool isPlayingHit = false;
    private bool isDead = false;
    private bool isLowHealth = false;

    private void Awake()
    {
        if (portraitImage == null)
        {
            portraitImage = GetComponent<Image>();
        }
    }

    private void Start()
    {
        if (portraitImage != null && idleOpenEye != null)
        {
            portraitImage.sprite = idleOpenEye;
        }

        StartCoroutine(IdleBlinkLoop());
    }

    private IEnumerator IdleBlinkLoop()
    {
        while (!isDead)
        {
            // Wait a random natural interval between blinks
            float waitTime = Random.Range(minBlinkInterval, maxBlinkInterval);
            yield return new WaitForSeconds(waitTime);

            // Only blink if we aren't currently flashing a hit reaction
            if (!isPlayingHit && !isDead && portraitImage != null && idleBlink != null)
            {
                portraitImage.sprite = idleBlink;
                yield return new WaitForSeconds(blinkDuration);

                if (!isPlayingHit && !isDead)
                {
                    portraitImage.sprite = GetRestingSprite();
                }
            }
        }
    }

    /// <summary>
    /// Call this from PlayerHealth.TakeDamage() to flash Red -> White -> Clear struggle frames.
    /// </summary>
    public void PlayHitExpression()
    {
        if (isDead || portraitImage == null) return;

        if (hitRoutine != null)
        {
            StopCoroutine(hitRoutine);
        }

        hitRoutine = StartCoroutine(HitSequenceRoutine());
    }

    private IEnumerator HitSequenceRoutine()
    {
        isPlayingHit = true;

        // Frame 1: Red struggle
        if (hitRed != null)
        {
            portraitImage.sprite = hitRed;
            yield return new WaitForSeconds(hitFrameDuration);
        }

        // Frame 2: White struggle
        if (hitWhite != null)
        {
            portraitImage.sprite = hitWhite;
            yield return new WaitForSeconds(hitFrameDuration);
        }

        // Frame 3: Clear struggle
        if (hitClear != null)
        {
            portraitImage.sprite = hitClear;
            yield return new WaitForSeconds(hitFrameDuration * 1.5f); // Hold clear struggle slightly longer
        }

        isPlayingHit = false;

        if (!isDead)
        {
            portraitImage.sprite = GetRestingSprite();
        }
    }

    public void SetLowHealthState(bool lowHealth)
    {
        isLowHealth = lowHealth;

        if (!isPlayingHit && !isDead && portraitImage != null)
        {
            portraitImage.sprite = GetRestingSprite();
        }
    }

    public void SetDeadState()
    {
        isDead = true;

        if (hitRoutine != null)
        {
            StopCoroutine(hitRoutine);
        }

        if (portraitImage != null)
        {
            portraitImage.sprite = deadOrLowHealthSprite != null ? deadOrLowHealthSprite : hitClear;
        }
    }

    private Sprite GetRestingSprite()
    {
        if (isLowHealth && deadOrLowHealthSprite != null)
        {
            return deadOrLowHealthSprite;
        }

        return idleOpenEye;
    }
}