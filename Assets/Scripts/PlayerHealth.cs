using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHealth : MonoBehaviour, IDamageable
{
    [Header("References")]
    [SerializeField] private PlayerLook playerLook;
    [SerializeField] private Image healthBarFill;                   // Drag your filled health bar Image here
    [SerializeField] private RawImage bloodSplatter;                // Drag your blood splatter RawImage here
    [SerializeField] private GameObject deathScreen;                // Drag your DeathScreen GameObject here
    [SerializeField] private PlayerPortraitAnimator portraitAnimator; // Drag your PlayerPortraitAnimator here

    [Header("Health Settings")]
    [SerializeField] private float maxHealth = 100f;
    [SerializeField] private float health = 100f;

    [Header("Low Health Settings")]
    [SerializeField] private float lowHealthThreshold = 30f; // Health level where the blood screen starts appearing

    private bool isDead = false;

    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        health = maxHealth;

        if (deathScreen != null)
        {
            deathScreen.SetActive(false);
        }

        UpdateHealthUI();
    }

    public void TakeDamage(float damage)
    {
        if (isDead)
            return;

        health -= damage;
        health = Mathf.Clamp(health, 0f, maxHealth);

        // Trigger the Red -> White -> Clear struggle expression animation
        if (portraitAnimator != null)
        {
            portraitAnimator.PlayHitExpression();
        }

        UpdateHealthUI();

        if (health <= 0f)
        {
            Die();
        }
    }

    private void UpdateHealthUI()
    {
        // 1. Update the Health Fill Bar
        if (healthBarFill != null)
        {
            healthBarFill.fillAmount = health / maxHealth;
        }

        // 2. Update Low Health Portrait State
        bool isLow = health <= lowHealthThreshold;
        if (portraitAnimator != null)
        {
            portraitAnimator.SetLowHealthState(isLow);
        }

        // 3. Update the Blood Effect (Only active when health is super low)
        if (bloodSplatter != null)
        {
            Color color = bloodSplatter.color;

            if (isLow)
            {
                // Calculate how close to 0 health the player is below the threshold
                float dangerPercent = 1f - (health / lowHealthThreshold);
                color.a = Mathf.Pow(dangerPercent, 1.5f); // Scales up intensity as health gets lower
            }
            else
            {
                // Completely hide the blood screen when health is above the low threshold
                color.a = 0f;
            }

            bloodSplatter.color = color;
        }
    }

    private void Die()
    {
        if (isDead)
            return;

        isDead = true;

        Debug.Log("Player Dead!");

        if (portraitAnimator != null)
        {
            portraitAnimator.SetDeadState();
        }

        if (deathScreen != null)
        {
            deathScreen.SetActive(true);
        }

        if (playerLook != null)
            playerLook.LockLook();

        // Pause the game
        Time.timeScale = 0f;

        // Show mouse cursor
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }
}