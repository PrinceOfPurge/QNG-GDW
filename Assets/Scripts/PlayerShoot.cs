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
    //[SerializeField] private int maxAmmo = 6;
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
        Cursor.lockState = CursorLockMode.Locked;
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
<<<<<<< Updated upstream

        if (controls.Player.Shoot.WasPressedThisFrame())
        {


=======
        if (Input.GetKeyUp(KeyCode.M)) //unlocks cursor for ui
        {
            CursorToggle(); // toggles cursor on/off
        }
        if (Cursor.lockState == CursorLockMode.Locked) //in game shooting
        {
            if (Input.GetMouseButtonDown(0))
            {
                Command shoot = new ShootCommand(_playershoot);
            }
            if (controls.Player.Reload.WasPressedThisFrame())
            {
                Reload();
            }
        }
        if (ammoText != null)
        {
            ammoText.text = currentMagAmmo.ToString();
        }
        //if (controls.Player.Shoot.WasPressedThisFrame())
        //{
        if ( Input.GetMouseButtonDown(0))
        { 
>>>>>>> Stashed changes
            Command shoot = new ShootCommand(_playershoot);
            _inputInvoker.AddCommand(shoot);
        }
        if (controls.Player.Reload.WasPressedThisFrame())
        {
            Reload();
        }
        ammoText.text = currentMagAmmo.ToString();
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
        // AudioManager.instance.PlayOneShot(shootSfx, transform.position);
    }
    private void CursorToggle() // function to toggle cursor on/off
    {
        if(Cursor.lockState == CursorLockMode.Locked)
        {
            UnlockCursor();
        }
        else
        {
            LockCursor();
        }
    }
    private void UnlockCursor() //unlocks cursor
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }
    private void LockCursor() //locks cursor
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }
}
