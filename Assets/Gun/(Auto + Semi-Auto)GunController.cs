using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class AutoAndSemiAutoGunController : MonoBehaviour
{
    [SerializeField] private CameraController camController;

    [Header("Gun Attributes")]
    [SerializeField] private int maxAmmo = 30;
    [SerializeField] private float reloadTime = 1f;
    [SerializeField] private bool automatic = false;
    [SerializeField] private Vector3 upRecoil;
    [SerializeField] private Vector3 sideRecoil;
    [SerializeField] private float recoilSmoothTime = 0.1f;
    [SerializeField] private float recoilResetSpeed = 5f;

    private bool isReloading = false;
    private bool canShoot = true;
    private int currentAmmo;
    private Vector3 accumulatedRecoil = Vector3.zero;
    private Quaternion initialRotation;
    private float recoilVelocityX, recoilVelocityY;


    [Space(20)]
    [Header("Shooting Attributes")]
    [SerializeField] private GameObject bulletSpawn;
    [SerializeField] private GameObject projectilePrefab;
    [SerializeField] private float fireRate = 15f;
    [SerializeField] private float bulletSpeed = 20f;

    private float nextTimeToFire = 0f;


    [Space(20)]
    [Header("Animations")]
    [SerializeField] private Animator animator;
    [SerializeField] private string zoomBool;
    [SerializeField] private string trigger;


    [Space(20)]
    [Header("Effects")]
    [SerializeField] private Camera playerCam;
    [SerializeField] private ParticleSystem muzzleFlash;


    [Space(20)]
    [Header("UI")]
    [SerializeField] private Text ammoText;


    [Space(20)]
    [Header("Audio")]
    [SerializeField] private AudioSource gunAudioSource;
    [SerializeField] private AudioClip shootClip;
    [SerializeField] private AudioClip reloadClip;


    [Space(20)]
    [Header("Other")]
    [SerializeField] private InputActionAsset gunControls;
    [SerializeField] private float targetZoomFOV = 40;
    [SerializeField] private float transitionDuration = 1f;
    [SerializeField] private float coneAngle;

    private InputAction shoot;
    private InputAction reload;
    private InputAction zoomInOrOut;
    private float zoomSpeed = 8f;
    private float zoomLevel = 0f;
    private Coroutine fovCoroutine;
    private float originalFOV;

    void Start()
    {
        initialRotation = transform.localRotation;

        currentAmmo = maxAmmo;
        UpdateAmmoText();

        shoot = gunControls.FindActionMap("Gun Controls").FindAction("Shoot");
        reload = gunControls.FindActionMap("Gun Controls").FindAction("Reload");
        zoomInOrOut = gunControls.FindActionMap("Gun Controls").FindAction("Zoom in/out");

        shoot.Enable();
        reload.Enable();
        zoomInOrOut.Enable();
        reload.performed += ctx => StartCoroutine(Reload());

        zoomInOrOut.performed += ctx => ChangeFOV(targetZoomFOV);
        zoomInOrOut.canceled += ctx => ChangeFOV(originalFOV);
    }
    private void Awake()
    {
        if (playerCam == null)
        {
            playerCam = Camera.main;
        }
        originalFOV = playerCam.fieldOfView;
    }

    void OnEnable()
    {
        isReloading = false;
    }

    void Update()
    {
        if (isReloading)
            return;

        if (automatic && shoot.ReadValue<float>() > 0 && Time.time >= nextTimeToFire)
        {
            nextTimeToFire = Time.time + 1f / fireRate;
            Shoot();
        }

        shoot.performed += ctx =>
        {
            if (!automatic && canShoot == true)
            {
                Shoot();
                canShoot = false;
            }
        };
        shoot.canceled += ctx => { StopRecoil(); };
        GunSight();

        float smoothX = Mathf.SmoothDampAngle(transform.localEulerAngles.x, (initialRotation * Quaternion.Euler(-accumulatedRecoil)).eulerAngles.x, ref recoilVelocityX, recoilSmoothTime);
        float smoothY = Mathf.SmoothDampAngle(transform.localEulerAngles.y, (initialRotation * Quaternion.Euler(-accumulatedRecoil)).eulerAngles.y, ref recoilVelocityY, recoilSmoothTime);
        transform.localRotation = Quaternion.Euler(smoothX, 0, smoothY);
    }

    private void GunSight()
    {
        float zoomInput = zoomInOrOut.ReadValue<float>();
        float targetZoom = zoomInput > 0.5f ? 1f : 0f;

        zoomLevel = Mathf.Lerp(zoomLevel, targetZoom, Time.deltaTime * zoomSpeed);
        //animator.SetFloat("ZoomBlend", zoomLevel);
    }
    public void ChangeFOV(float newFOV)
    {
        if (fovCoroutine != null)
        {
            StopCoroutine(fovCoroutine);
        }
        fovCoroutine = StartCoroutine(SmoothFOVChange(newFOV));
    }

    private IEnumerator SmoothFOVChange(float newFOV)
    {
        float startFOV = playerCam.fieldOfView;
        float elapsedTime = 0f;

        while (elapsedTime < transitionDuration)
        {
            elapsedTime += Time.deltaTime;
            playerCam.fieldOfView = Mathf.Lerp(startFOV, newFOV, elapsedTime / transitionDuration);
            yield return null;
        }

        playerCam.fieldOfView = newFOV;
    }
    void Shoot()
    {
        // This is important to make semi auto work
        Invoke("CanShootReset", 0.2f);

        if (currentAmmo == 0)
        {
            StartCoroutine(Reload());
        }

        camController.GunController();

        if (currentAmmo >= 1)
        {
            // Shoot animations
            AddRecoil();
            currentAmmo--;
            UpdateAmmoText();
            // Spawn and shoot the bullet
            GameObject projectile = Instantiate(projectilePrefab, bulletSpawn.transform.position, bulletSpawn.transform.rotation);
            muzzleFlash.Play();
            gunAudioSource.PlayOneShot(shootClip);
            Rigidbody rb = projectile.GetComponent<Rigidbody>();
            Vector3 bulletDirection = GetConeSpreadDirection(bulletSpawn.transform.forward, coneAngle);
            rb.velocity = bulletDirection * bulletSpeed;
        }
    }
    IEnumerator Reload()
    {
        StartCoroutine(ResetRecoil());
        isReloading = true;
        // gunAudioSource.PlayOneShot(reloadClip);
        // gun reload animation
        yield return new WaitForSeconds(reloadTime);
        currentAmmo = maxAmmo;
        isReloading = false;
        UpdateAmmoText();
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
    private void AddRecoil()
    {
        float sideAmount = Random.Range(-sideRecoil.y, sideRecoil.y);
        accumulatedRecoil = new Vector3(-upRecoil.x, sideAmount, 0f);
    }

    private void StopRecoil()
    {
        StartCoroutine(ResetRecoil());
    }

    private IEnumerator ResetRecoil()
    {
        Vector3 startRecoil = accumulatedRecoil;
        float elapsedTime = 0f;
        float duration = 1f / recoilResetSpeed;

        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            accumulatedRecoil = Vector3.Lerp(startRecoil, Vector3.zero, elapsedTime / duration);
            yield return null;
        }
        accumulatedRecoil = Vector3.zero;
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