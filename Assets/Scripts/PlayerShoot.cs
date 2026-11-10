using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;

public class PlayerShoot : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Camera playerCamera;
    [SerializeField] private TMP_Text ammoText;
    [SerializeField] private Animator playerAnimator;

    [Header("Animation Triggers")]
    [SerializeField] private string shootTriggerName = "Shoot";
    [SerializeField] private string reloadTriggerName = "Reload";
    [SerializeField] private string equipTriggerName = "Equip";

    [Header("Settings")]
    [SerializeField] private float shootRange = 100f;
    [SerializeField] private float damage = 25f;
    [SerializeField] private float fireRate = 0.5f;
    [SerializeField] private float reloadTime = 1.5f; // Total duration of the reload animation/sequence

    [Header("Ammo")]
    [SerializeField] private int currentAmmo = 6;
    [SerializeField] private int magSize = 6;
    [SerializeField] private int currentMagAmmo = 6;

    private PlayerControls controls;

    private float nextTimeToFire;
    private bool isReloading = false;

    private void Awake()
    {
        controls = new PlayerControls();

        if (playerAnimator == null)
        {
            playerAnimator = GetComponentInChildren<Animator>();
        }

        if (ammoText == null)
        {
            Debug.LogWarning("Ammo Text reference is not assigned on PlayerShoot!");
        }

        UpdateAmmoText();
    }

    private void OnEnable()
    {
        controls.Enable();

        // Trigger Equip animation whenever player control is enabled
        if (playerAnimator != null && !string.IsNullOrEmpty(equipTriggerName))
        {
            playerAnimator.SetTrigger(equipTriggerName);
        }
    }

    private void OnDisable()
    {
        controls.Disable();
        isReloading = false;
    }

    private void Update()
    {
        // Don't accept inputs if currently reloading
        if (isReloading)
            return;

        if (controls.Player.Shoot.WasPressedThisFrame())
        {
            Shoot();
        }
        if (controls.Player.Reload.WasPressedThisFrame())
        {
            StartCoroutine(ReloadRoutine());
        }
    }

    private void UpdateAmmoText()
    {
        if (ammoText != null)
        {
            ammoText.text = currentMagAmmo + " / " + currentAmmo;
        }
    }

    private IEnumerator ReloadRoutine()
    {
        // Don't reload if magazine is full or reserve ammo is empty
        if (currentMagAmmo >= magSize || currentAmmo <= 0)
        {
            Debug.Log("Cannot reload (Mag full or no reserve ammo)");
            yield break;
        }

        isReloading = true;

        // 1. Play Reload animation on UpperBody Layer
        if (playerAnimator != null && !string.IsNullOrEmpty(reloadTriggerName))
        {
            playerAnimator.SetTrigger(reloadTriggerName);
        }

        // 2. Play reload sound effect once at the start
        if (FMODEvents.instance != null && AudioManager.instance != null)
        {
            AudioManager.instance.PlayOneShot(FMODEvents.instance.Reload, transform.position);
        }

        // Calculate how many bullets we actually need to load
        int bulletsNeeded = magSize - currentMagAmmo;
        int bulletsToReload = Mathf.Min(bulletsNeeded, currentAmmo);

        // Divide total reload animation time evenly across bullets being loaded
        float timePerBullet = reloadTime / bulletsToReload;

        // Increment bullets one by one over the duration of the reload animation
        for (int i = 0; i < bulletsToReload; i++)
        {
            yield return new WaitForSeconds(timePerBullet);

            currentMagAmmo++;
            currentAmmo--;

            UpdateAmmoText(); // Updates UI dynamically as each bullet is loaded into mag
        }

        isReloading = false;

        Debug.Log("Reload complete. Bullets in mag: " + currentMagAmmo + ". Bullets left: " + currentAmmo);
    }

    private void Shoot()
    {
        // Cooldown for shots or while reloading
        if (Time.time < nextTimeToFire || isReloading)
            return;

        // If out of magazine ammo
        if (currentMagAmmo <= 0)
        {
            // Play dry fire sound effect
            if (FMODEvents.instance != null && AudioManager.instance != null)
            {
                AudioManager.instance.PlayOneShot(FMODEvents.instance.dryFire, transform.position);
            }

            if (currentAmmo > 0)
            {
                Debug.Log("Out of magazine ammo! Auto reloading...");
                StartCoroutine(ReloadRoutine());
            }
            else
            {
                Debug.Log("Completely out of ammo!");
            }

            nextTimeToFire = Time.time + fireRate;
            return;
        }

        // 1. Play Shooting Animation
        if (playerAnimator != null && !string.IsNullOrEmpty(shootTriggerName))
        {
            playerAnimator.SetTrigger(shootTriggerName);
        }

        // 2. Play shooting sound effect
        if (FMODEvents.instance != null && AudioManager.instance != null)
        {
            AudioManager.instance.PlayOneShot(FMODEvents.instance.Shoot, transform.position);
        }

        nextTimeToFire = Time.time + fireRate;

        currentMagAmmo--;

        UpdateAmmoText();

        // Raycast shooting logic
        Ray ray = playerCamera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0f));
        RaycastHit[] hits = Physics.RaycastAll(ray, shootRange);

        System.Array.Sort(hits, (hit1, hit2) => hit1.distance.CompareTo(hit2.distance));

        foreach (RaycastHit hit in hits)
        {
            Debug.Log("Shot hit: " + hit.collider.name);

            IDamageable damageable = hit.collider.GetComponent<IDamageable>();
            if (damageable != null)
            {
                damageable.TakeDamage(damage);
            }
        }
    }
}