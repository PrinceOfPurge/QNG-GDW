using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerShoot : MonoBehaviour
{
    public PlayerShoot _playershoot;
    InputInvoker _inputInvoker;

    [Header("References")]
    [SerializeField] private Camera playerCamera;

    [Header("Settings")]
    [SerializeField] private float shootRange = 100f;
    [SerializeField] private float damage = 25f;
    [SerializeField] private float fireRate = 0.5f;

    [Header("Ammo")]
    [SerializeField] private int currentAmmo = 6;
    [SerializeField] private int magSize = 6;
    [SerializeField] private int currentMagAmmo = 6;
    [SerializeField] private TextMeshProUGUI ammoText;

    private PlayerControls controls;
    private float nextTimeToFire;

    [SerializeField] private FMODUnity.EventReference shootSfx;

    private void Awake()
    {
        controls = new PlayerControls();
    }

    private void Start()
    {
        LockCursor();
        _inputInvoker = new InputInvoker();
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
        // 1. Always check M key to toggle UI/Cursor (Outside of shooting check)
        if (Input.GetKeyUp(KeyCode.M))
        {
            CursorToggle();
        }

        // 2. Only allow gameplay shooting/reloading when cursor is locked
        if (Cursor.lockState == CursorLockMode.Locked)
        {
            if (Input.GetMouseButtonDown(0) || controls.Player.Shoot.WasPressedThisFrame())
            {
                Command shoot = new ShootCommand(_playershoot);
                _inputInvoker.AddCommand(shoot);
            }

            if (controls.Player.Reload.WasPressedThisFrame())
            {
                Reload();
            }
        }

        // 3. UI Updates
        if (ammoText != null)
        {
            ammoText.text = currentMagAmmo.ToString();
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

    public void Shoot()
    {
        if (Time.time < nextTimeToFire)
            return;

        if (currentMagAmmo <= 0)
            return;

        Debug.Log("Shoot");
        nextTimeToFire = Time.time + fireRate;
        currentMagAmmo--;

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

    private void CursorToggle()
    {
        if (Cursor.lockState == CursorLockMode.Locked)
        {
            UnlockCursor();
        }
        else
        {
            LockCursor();
        }
    }

    private void UnlockCursor()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    private void LockCursor()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }
}