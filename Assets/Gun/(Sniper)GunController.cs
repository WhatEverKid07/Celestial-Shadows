using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class SniperGunController : MonoBehaviour
{
    [Header("Gun Attributes")]
    public int maxAmmo = 10;
    private int currentAmmo;
    public float reloadTime = 4f;
    private bool isReloading = false;
    private bool canShoot = true;
    public float shotDelay = 0.2f;
    public bool isSighted = false;
    [SerializeField] private float coneAngle;

    private float currentConeAngle;

    [Space(20)]
    [Header("Shooting Attributes")]
    public float fireRate = 15f;
    public float bulletSpeed = 20f;
    private float nextTimeToFire = 0f;
    public GameObject bulletSpawn;
    public GameObject projectilePrefab;

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
        currentConeAngle = coneAngle;

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

        if (Input.GetButtonDown("Fire1") && canShoot == true)
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
        yield return new WaitForSeconds(reloadTime);
        currentAmmo = maxAmmo;
        isReloading = false;
        UpdateAmmoText();
    }

    void Shoot()
    {
        Invoke("CanShootReset", shotDelay);
        if (currentAmmo <= 0)
            return;

        currentAmmo--;
        UpdateAmmoText();
        muzzleFlash.Play();
        gunAudioSource.PlayOneShot(shootClip);
        // Spawn and shoot the bullet
        GameObject projectile = Instantiate(projectilePrefab, bulletSpawn.transform.position, bulletSpawn.transform.rotation);
        Rigidbody rb = projectile.GetComponent<Rigidbody>();
        Vector3 bulletDirection = GetConeSpreadDirection(bulletSpawn.transform.forward, currentConeAngle);
        rb.velocity = bulletDirection * bulletSpeed;
    }
    private Vector3 GetConeSpreadDirection(Vector3 forwardDirection, float maxAngle)
    {
        float maxAngleRad = maxAngle * Mathf.Deg2Rad;
        float randomAngle = Random.Range(0, 2 * Mathf.PI);
        float randomRadius = Mathf.Sin(maxAngleRad) * Random.Range(0f, 1f);

        Vector3 randomSpread = new Vector3(
            randomRadius * Mathf.Cos(randomAngle),
            randomRadius * Mathf.Sin(randomAngle),
            Mathf.Cos(maxAngleRad)
        );
        Quaternion rotation = Quaternion.LookRotation(forwardDirection);
        return (rotation * randomSpread).normalized;
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