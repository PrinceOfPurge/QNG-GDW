using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHealth : MonoBehaviour, IDamageable
{
    [SerializeField] private float health = 100f;
    [SerializeField] private float maxHealth = 100f;

    [SerializeField] private PlayerLook playerLook;

    private RawImage bloodSplatter;
    private GameObject deathScreen;

    private bool isDead = false;

    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        health = maxHealth;

        //find the playerHUD tag
        GameObject playerHUD = GameObject.FindWithTag("PlayerHUD");

        if (playerHUD != null)
        {
            //get the blood scatter texture
            bloodSplatter = playerHUD.GetComponentInChildren<RawImage>();

            Transform deathScreenTransform = playerHUD.transform.Find("DeathScreen");

            if (deathScreenTransform != null)
            {
                deathScreen = deathScreenTransform.gameObject;
                deathScreen.SetActive(false);
            }
            else
            {
                Debug.LogWarning("DeathScreen not found!");
            }
        }
        else
        {
            Debug.LogWarning("PlayerHUD not found!");
        }

        UpdateBloodEffect();
    }

    public void TakeDamage(float damage)
    {
        if (isDead)
            return;

        health -= damage;

        UpdateBloodEffect();

        if (health <= 0f)
        {
            Die();
        }
    }

    private void UpdateBloodEffect()
    {
        if (bloodSplatter != null)
        {
            float damagePercent = Mathf.Clamp01(1f - (health / maxHealth));
            float alpha = Mathf.Pow(damagePercent, 2f);
            Color color = bloodSplatter.color;
            color.a = alpha;
            bloodSplatter.color = color;
        }
    }

    private void Die()
    {
        if (isDead)
            return;

        isDead = true;

        Debug.Log("Player Dead!");

        if (deathScreen != null)
        {
            deathScreen.SetActive(true);
        }

        if (playerLook != null)
            playerLook.LockLook();

        //pause the game
        Time.timeScale = 0f;

        //show mouse cursor
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }
}
