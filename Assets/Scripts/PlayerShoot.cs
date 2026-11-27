using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;
using UnityEngine.UI;

public class PlayerShoot : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Camera playerCamera;
    [SerializeField] private TMP_Text magAmmoText;      // Displays current bullets in magazine
    [SerializeField] private TMP_Text reserveAmmoText;  // Displays remaining reserve ammo
    [SerializeField] private Animator playerAnimator;

    [Header("Crosshair & Reload Prompt")]
    [SerializeField] private Image crosshairImage;
    [SerializeField] private TMP_Text reloadPromptText; // Drag a center-screen TMP_Text (e.g. "RELOAD") here
    [SerializeField] private string reloadMessage = "RELOAD";
    [SerializeField] private string outOfAmmoMessage = "NO AMMO";
    [SerializeField] private Color defaultCrosshairColor = Color.white;
    [SerializeField] private Color enemyCrosshairColor = Color.red;

    [Header("Bullet UI Icons")]
    [Tooltip("Drag your 6 bullet UI Image objects here in order (Bullet 1 to Bullet 6)")]
    [SerializeField] private Image[] bulletIcons;
    [SerializeField] private bool dimEmptyBulletsInsteadOfHiding = false;
    [SerializeField] private Color emptyBulletColor = new Color(1f, 1f, 1f, 0.2f);

    [Header("Animation Triggers")]
    [SerializeField] private string shootTriggerName = "Shoot";
    [SerializeField] private string reloadTriggerName = "Reload";
    [SerializeField] private string equipTriggerName = "Equip";

    [Header("Settings")]
    [SerializeField] private float shootRange = 100f;
    [SerializeField] private float damage = 25f;
    [SerializeField] private float headshotMultiplier = 2.5f;
    [SerializeField] private float fireRate = 0.5f;
    [SerializeField] private float reloadTime = 1.5f; // Total duration of the reload animation/sequence

    [Header("Ammo")]
    [SerializeField] private int currentAmmo = 6;
    [SerializeField] private int magSize = 6;
    [SerializeField] private int currentMagAmmo = 6;

    [Header("Muzzle Flash & Blood VFX")]
    [SerializeField] private ParticleSystem muzzleFlash; // muzzle flash for the gun must be placed manually
    [SerializeField] private GameObject bodyBloodVfxPrefab;
    [SerializeField] private GameObject headshotChunkVfxPrefab;
    [SerializeField] private float vfxLifetime = 3f;

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

        UpdateAmmoUI();
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
        UpdateCrosshairUI();

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

    private void UpdateCrosshairUI()
    {
        if (playerCamera == null)
            return;

        // Show RELOAD / NO AMMO text instead of crosshair when magazine is empty or reloading
        bool showReloadText = currentMagAmmo <= 0 || isReloading;

        if (reloadPromptText != null)
        {
            reloadPromptText.gameObject.SetActive(showReloadText);
            if (showReloadText)
            {
                reloadPromptText.text = (currentAmmo <= 0 && !isReloading) ? outOfAmmoMessage : reloadMessage;
            }
        }

        if (crosshairImage != null)
        {
            crosshairImage.enabled = !showReloadText;

            if (showReloadText)
                return;
        }
        else
        {
            return;
        }

        // Raycast to tint crosshair red when aiming at a living zombie
        Ray ray = playerCamera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0f));
        RaycastHit[] hits = Physics.RaycastAll(ray, shootRange);
        System.Array.Sort(hits, (hit1, hit2) => hit1.distance.CompareTo(hit2.distance));

        bool aimingAtZombie = false;

        foreach (RaycastHit hit in hits)
        {
            // Ignore the player's own colliders
            if (hit.collider.CompareTag("Player"))
                continue;

            // Check if aiming at a living zombie (EnemyMovement gets disabled when Zombie dies)
            Zombie zombie = hit.collider.GetComponentInParent<Zombie>();
            if (zombie != null)
            {
                EnemyMovement movement = zombie.GetComponent<EnemyMovement>();
                if (movement == null || movement.enabled)
                {
                    aimingAtZombie = true;
                }
                break;
            }

            // Stop raycast if blocked by the truck, window frame, or a wall
            if (!hit.collider.isTrigger)
            {
                break;
            }
        }

        crosshairImage.color = aimingAtZombie ? enemyCrosshairColor : defaultCrosshairColor;
    }

    private void UpdateAmmoUI()
    {
        // 1. Update separate magazine and reserve textboxes
        if (magAmmoText != null)
        {
            magAmmoText.text = currentMagAmmo.ToString();
        }

        if (reserveAmmoText != null)
        {
            reserveAmmoText.text = currentAmmo.ToString();
        }

        // 2. Sync visual bullet UI icons with currentMagAmmo
        if (bulletIcons != null)
        {
            for (int i = 0; i < bulletIcons.Length; i++)
            {
                if (bulletIcons[i] == null) continue;

                bool hasBullet = i < currentMagAmmo;

                if (dimEmptyBulletsInsteadOfHiding)
                {
                    bulletIcons[i].enabled = true;
                    bulletIcons[i].color = hasBullet ? Color.white : emptyBulletColor;
                }
                else
                {
                    bulletIcons[i].enabled = hasBullet;
                }
            }
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

            UpdateAmmoUI(); // Updates textboxes AND pops each bullet icon back in one by one
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

        UpdateAmmoUI();

        // Raycast shooting logic
        Ray ray = playerCamera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0f));
        RaycastHit[] hits = Physics.RaycastAll(ray, shootRange);

        System.Array.Sort(hits, (hit1, hit2) => hit1.distance.CompareTo(hit2.distance));

        foreach (RaycastHit hit in hits)
        {
            // Ignore the player's own colliders
            if (hit.collider.CompareTag("Player"))
                continue;

            Debug.Log("Shot hit: " + hit.collider.name);

            // Check the hit bone AND all parent objects up to the root
            Zombie zombie = hit.collider.GetComponentInParent<Zombie>();
            if (zombie != null)
            {
                bool isHeadshot = hit.collider.CompareTag("Head") ||
                                  hit.collider.name.ToLower().Contains("head");

                float finalDamage = isHeadshot ? damage * headshotMultiplier : damage;
                float knockbackForce = isHeadshot ? 28f : 15f;

                // Play 3D headshot or body shot impact audio at hit.point
                if (FMODEvents.instance != null && AudioManager.instance != null)
                {
                    if (isHeadshot)
                    {
                        AudioManager.instance.PlayOneShot(FMODEvents.instance.Headshot, hit.point);
                    }
                    else
                    {
                        AudioManager.instance.PlayOneShot(FMODEvents.instance.Bodyshot, hit.point);
                    }
                }

                // Spawn headshot chunk/volumetric VFX or standard body blood VFX
                if (isHeadshot && headshotChunkVfxPrefab != null)
                {
                    GameObject vfx = Instantiate(
                        headshotChunkVfxPrefab,
                        hit.point,
                        Quaternion.LookRotation(ray.direction)
                    );
                    Destroy(vfx, vfxLifetime);
                }
                else if (bodyBloodVfxPrefab != null)
                {
                    GameObject vfx = Instantiate(
                        bodyBloodVfxPrefab,
                        hit.point,
                        Quaternion.LookRotation(hit.normal)
                    );
                    Destroy(vfx, vfxLifetime);
                }

                zombie.TakeDamageWithForce(finalDamage, ray.direction * knockbackForce, hit.point);
                break; // Stop raycast so bullet doesn't hit multiple bones behind it
            }

            IDamageable damageable = hit.collider.GetComponentInParent<IDamageable>();
            if (damageable != null)
            {
                damageable.TakeDamage(damage);
                break;
            }

            // Stop raycast if it hits a solid wall/barricade/truck
            if (!hit.collider.isTrigger)
            {
                break;
            }
        }

        if (muzzleFlash != null)
        {
            muzzleFlash.Play(); // Plays the muzzle flash
        }
    }
}