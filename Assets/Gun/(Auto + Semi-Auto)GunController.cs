using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class AutoAndSemiAutoGunController : MonoBehaviour
{
    [Header("Gun Attributes")]
    public int maxAmmo = 30;
    private int currentAmmo;
    public float reloadTime = 1f;
    private bool isReloading = false;
    public bool automatic = false;
    private bool canShoot = true;

    [Space(20)]
    [Header("Shooting Attributes")]
    //public float damage = 10f;
    //public float range = 100f;
    public float fireRate = 15f;
    private float nextTimeToFire = 0f;
    public GameObject bulletSpawn;
    public GameObject projectilePrefab; // Projectile prefab to fire
    public float bulletSpeed = 20f; // Speed of the projectile

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

        if (currentAmmo <= 0)
        {
            StartCoroutine(Reload());
            return;
        }

        if (automatic == true && Input.GetButton("Fire1") && Time.time >= nextTimeToFire)
        {
            nextTimeToFire = Time.time + 1f / fireRate;
            Shoot();
            //muzzleFlash.Play();
        }

        if (automatic == false && Input.GetButtonDown("Fire1") && canShoot == true)
        {
            Shoot();
            canShoot = false;
            //muzzleFlash.Play();

        }

        if (Input.GetKeyDown(KeyCode.R))
        {
            StartCoroutine(Reload());
        }

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
        yield return new WaitForSeconds(reloadTime - 0.25f);
        yield return new WaitForSeconds(0.25f);
        currentAmmo = maxAmmo;
        isReloading = false;
        UpdateAmmoText();
    }

    void Shoot()
    {
        Invoke("CanShootReset", 0.2f);
        if (currentAmmo <= 0)
            return;

        currentAmmo--;
        UpdateAmmoText();

        
        
        muzzleFlash.Play();

        gunAudioSource.PlayOneShot(shootClip);
        

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
