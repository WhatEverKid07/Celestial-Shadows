using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class AutoAndSemiAutoGunController : MonoBehaviour
{
    [Header("Gun Attributes")]
    public int maxAmmo = 30;
    public float reloadTime = 1f;
    public bool automatic = false;

    private bool isReloading = false;
    private bool canShoot = true;
    private int currentAmmo;


    [Space(20)]
    [Header("Shooting Attributes")]
    public GameObject bulletSpawn;
    public GameObject projectilePrefab;
    public float fireRate = 15f;
    public float bulletSpeed = 20f;

    private float nextTimeToFire = 0f;


    [Space(20)]
    [Header("Animations")]
    public Animator animator;
    public string zoomBool;
    public string trigger;


    [Space(20)]
    [Header("Effects")]
    public Camera fpsCam;
    public ParticleSystem muzzleFlash;
    public GameObject impactEffect;
    public float impactForce = 30f;


    [Space(20)]
    [Header("UI")]
    public Text ammoText;


    [Space(20)]
    [Header("Audio")]
    public AudioSource gunAudioSource;
    public AudioClip shootClip;
    public AudioClip reloadClip;


    [Space(20)]
    [Header("Inputs")]
    public InputActionAsset gunControls;
    private InputActionMap gunControlsActionMap;
    private InputAction shoot;
    private InputAction reload;
    private InputAction zoomInOrOut;

    void Start()
    {
        currentAmmo = maxAmmo;
        UpdateAmmoText();


    }

    void OnEnable()
    {
        isReloading = false;
    }

    void Update()
    {
        if (isReloading)
            return;

        if (automatic == true && Input.GetButton("Fire1") && Time.time >= nextTimeToFire)
        {
            nextTimeToFire = Time.time + 1f / fireRate;
            Shoot();
        }

        if (automatic == false && Input.GetButtonDown("Fire1") && canShoot == true)
        {
            Shoot();
            canShoot = false;
        }
        //Reload
        if (Input.GetKeyDown(KeyCode.R))
        {
            StartCoroutine(Reload());
        }

        //Zoom in/out
        if (Input.GetMouseButtonDown(1))
        {
            animator.SetTrigger(zoomBool);
        }
        else if (Input.GetMouseButtonUp(1))
        {
            animator.SetTrigger(trigger);
        }
    }

    IEnumerator Reload()
    {
        isReloading = true;
        //gunAudioSource.PlayOneShot(reloadClip);
        //gun reload animation
        yield return new WaitForSeconds(reloadTime);
        currentAmmo = maxAmmo;
        isReloading = false;
        UpdateAmmoText();
    }

    void Shoot()
    {
        //this is important to make semi auto work
        Invoke("CanShootReset", 0.2f);

        if (currentAmmo == 0)
        {
            StartCoroutine(Reload());
            return;
        }

        muzzleFlash.Play();
        gunAudioSource.PlayOneShot(shootClip);
        //recoil and shoot animations
        currentAmmo--;
        UpdateAmmoText();

        // Spawn and shoot the bullet
        GameObject projectile = Instantiate(projectilePrefab, bulletSpawn.transform.position, bulletSpawn.transform.rotation);
        Rigidbody rb = projectile.GetComponent<Rigidbody>();
        rb.velocity = bulletSpawn.transform.forward * bulletSpeed;
    }
    void CanShootReset()
    {
        canShoot = true;
    }
    void UpdateAmmoText()
    {
        ammoText.text = currentAmmo.ToString() + " / " + maxAmmo.ToString();
    }
}