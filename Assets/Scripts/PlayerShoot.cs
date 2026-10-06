using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerShoot : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Camera playerCamera;

    [Header("Settings")]
    [SerializeField] private float shootRange = 100f;
    [SerializeField] private float damage = 25f;
    [SerializeField] private float fireRate = 0.5f;

    [Header("Ammo")]
    //[SerializeField] private int maxAmmo = 6;
    [SerializeField] private int currentAmmo = 6;
    [SerializeField] private int magSize = 6;
    [SerializeField] private int currentMagAmmo = 6;

    private PlayerControls controls;

    private float nextTimeToFire;

    private void Awake()
    {
        controls = new PlayerControls();
    }

    private void OnEnable()
    {
        controls.Enable();
    }

    private void OnDisable()
    {
        controls.Disable();
    }

    private void Update()
    {
        if (controls.Player.Shoot.WasPressedThisFrame())
        {
            Shoot();
        }
        if (controls.Player.Reload.WasPressedThisFrame())
        {
            Reload();
        }
    }

    private void Reload()
    {
        int bulletsToReload;

        if (currentAmmo <= 0)
        {
            Debug.Log("Out of ammo");
            return;
        }

        bulletsToReload = magSize - currentMagAmmo;

        if (currentAmmo < bulletsToReload)
            bulletsToReload = currentAmmo;

        currentMagAmmo += bulletsToReload;
        currentAmmo -= bulletsToReload;

        Debug.Log("Bullets in mag: " + currentMagAmmo + ". Bullets left: " + currentAmmo);
    }

    private void Shoot()
    {
        // Cooldown for shots
        if (Time.time < nextTimeToFire)
            return;

        // if out of ammo
        if (currentMagAmmo <= 0)
            return;

        Debug.Log("Shoot");

        nextTimeToFire = Time.time + fireRate;

        currentMagAmmo--;

        // shoot from the center of camera
        Ray ray = playerCamera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0f));

        RaycastHit[] hits = Physics.RaycastAll(ray, shootRange);

        // Hit in order of distance
        System.Array.Sort(hits, (hit1, hit2) => hit1.distance.CompareTo(hit2.distance));

        foreach (RaycastHit hit in hits)
        {
            Debug.Log("Shot hit: " + hit.collider.name);

            // lkook for something that uses IDamageable
            IDamageable damageable = hit.collider.GetComponent<IDamageable>();

            if (damageable != null)
            {
                damageable.TakeDamage(damage);
            }
        }
    }
}
