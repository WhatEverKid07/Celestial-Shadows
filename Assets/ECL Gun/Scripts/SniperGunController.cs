using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class SniperGunController : MonoBehaviour
{
    [SerializeField] private CameraController camController;
    [SerializeField] private GunManagement gunManager;

    [Header("Gun Attributes")]
    [SerializeField] private int maxAmmo = 10;
    [SerializeField] private float reloadTime = 4f;
    [SerializeField] private float shotDelay = 0.2f;
    [SerializeField] private float coneAngle;
    [SerializeField] private float coneAngleWhenSighted;

    private float currentConeAngle;
    private int currentAmmo;
    private bool isReloading = false;
    private bool canShoot = true;


    [Space(20)]
    [Header("Shooting Attributes")]
    [SerializeField] private GameObject bulletSpawn;
    [SerializeField] private GameObject projectilePrefab;
    [SerializeField] private float bulletSpeed = 20f;


    [Space(20)]
    [Header("Animations")]
    [SerializeField] private Animator animator;
    [SerializeField] private string zoomBool;
    [SerializeField] private string trigger;


    [Space(20)]
    [Header("Effects")]
    [SerializeField] private Camera fPSCam;
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

    private InputAction shoot;
    private InputAction reload;
    private InputAction zoomInOrOut;
    private float zoomSpeed = 8f;
    private float zoomLevel = 0f;
    private Coroutine fovCoroutine;
    private float originalFOV;

    void Start()
    {
        currentConeAngle = coneAngle;
        currentAmmo = maxAmmo;
        UpdateAmmoText();

        shoot = gunControls.FindActionMap("Gun Controls").FindAction("Shoot");
        reload = gunControls.FindActionMap("Gun Controls").FindAction("Reload");
        zoomInOrOut = gunControls.FindActionMap("Gun Controls").FindAction("Zoom in/out");

        shoot.Enable();
        reload.Enable();
        zoomInOrOut.Enable();


        zoomInOrOut.performed += ctx => ChangeFOV(targetZoomFOV);
        zoomInOrOut.canceled += ctx => ChangeFOV(originalFOV);
        zoomInOrOut.performed += ctx => gunManager.canSwitch = false;
        zoomInOrOut.canceled += ctx => gunManager.canSwitch = true;
        zoomInOrOut.canceled += ctx => currentConeAngle = coneAngle;
    }

    private void Awake()
    {
        if (fPSCam == null)
        {
            fPSCam = Camera.main;
        }
        originalFOV = fPSCam.fieldOfView;
    }

    void OnEnable()
    {
        isReloading = false;
        ammoText.gameObject.SetActive(true);
        UpdateAmmoText();
    }
    private void OnDisable()
    {
        if (ammoText != null)
            ammoText.gameObject.SetActive(false);
    }

    void Update()
    {
        if (!gameObject.activeInHierarchy)
            return;

        if (gameObject.activeInHierarchy && reload.ReadValue<float>() > 0)
        {
            StartCoroutine(Reload());
        }

        if (currentAmmo == 0)
        {
            StartCoroutine(Reload());
            return;
        }

        if (isReloading)
            return;

        shoot.performed += ctx =>
        {
            if (canShoot == true && gameObject.activeInHierarchy)
            {
                Shoot();
                canShoot = false;
            }
        };
        GunSight();
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
        if (gameObject.activeInHierarchy)
        {
            fovCoroutine = StartCoroutine(SmoothFOVChange(newFOV));
        }
    }

    private IEnumerator SmoothFOVChange(float newFOV)
    {
        currentConeAngle = coneAngleWhenSighted;
        float startFOV = fPSCam.fieldOfView;
        float elapsedTime = 0f;

        while (elapsedTime < transitionDuration)
        {
            elapsedTime += Time.deltaTime;
            fPSCam.fieldOfView = Mathf.Lerp(startFOV, newFOV, elapsedTime / transitionDuration);
            yield return null;
        }
        fPSCam.fieldOfView = newFOV;
    }

    void Shoot()
    {
        Invoke("CanShootReset", shotDelay);

        if (currentAmmo >= 1)
        {
            camController.GunController();
            currentAmmo--;
            UpdateAmmoText();
            muzzleFlash.Play();
            gunAudioSource.PlayOneShot(shootClip);
            GameObject projectile = Instantiate(projectilePrefab, bulletSpawn.transform.position, bulletSpawn.transform.rotation);
            Rigidbody rb = projectile.GetComponent<Rigidbody>();
            Vector3 bulletDirection = GetConeSpreadDirection(bulletSpawn.transform.forward, currentConeAngle);
            rb.velocity = bulletDirection * bulletSpeed;
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