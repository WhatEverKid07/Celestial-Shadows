using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class NewShotgunScript : MonoBehaviour
{
    [SerializeField] private CameraMovement camMovement;
    [SerializeField] private CameraController camController;
    [SerializeField] private GunManagement gunManager;
    [SerializeField] private CameraHeadBob bob;
    [SerializeField] private CharacterMovement characterMovement;

    [Header("Gun Attributes")]
    [SerializeField] private int maxAmmo = 30;
    [SerializeField] private float reloadTime = 1f;
    [SerializeField] private float coneAngle;
    [SerializeField] private float semiAutoShotDelay = 0.2f;
    [SerializeField] private float targetSightZoomFOV = 40f;
    [SerializeField] private float zoomTransitionDuration = 1f;

    private bool isReloading = false;
    private bool canShoot = true;
    private int currentAmmo;


    [Space(20)]
    [Header("Recoil")]
    [SerializeField] private float recoilForce;
    [SerializeField] private AnimationCurve RecoilUp;
    [Tooltip("How long is entire recoil sequence?")]
    [SerializeField] private float TimeInterval = 0.25f;
    [Tooltip("How long is recovery sequence?")]
    [SerializeField] private float RecoveryTime = 0.25f;
    [Tooltip("Which object is having its .localRotation driven.")]
    [SerializeField] private Transform RecoilPivot;

    private float recoiling;
    private float recovering;
    private Quaternion originalRotation;

    [Space(20)]
    [Header("Shooting Attributes")]
    [SerializeField] private GameObject bulletSpawn;
    [SerializeField] private GameObject projectilePrefab;
    [SerializeField] private float fireRate = 15f;
    [SerializeField] private float bulletSpeed = 20f;

    private float nextTimeToFire = 0f;

    [Space(20)]
    [Header("Animation")]
    [SerializeField] private Animator animator;
    [SerializeField] private Animator secondAnimator;
    [SerializeField] private string nameOfReloadAnim;
    [SerializeField] private string nameOfShootTrigger;
    [SerializeField] private string nameOfSightAnim;
    [SerializeField] private string nameOfSightAnimReverse;

    [Space(20)]
    [Header("Effects")]
    [SerializeField] private Camera playerCam;

    [Space(20)]
    [Header("UI")]
    [SerializeField] private Text ammoText;
    //[SerializeField] private GameObject crosshair;

    [Space(20)]
    [Header("Audio")]
    [SerializeField] private AudioSource shootClip;
    [SerializeField] private AudioSource reloadClip;

    [Space(20)]
    [Header("Other")]
    [SerializeField] private InputActionAsset gunControls;

    private InputAction shoot;
    private InputAction reload;
    private InputAction zoomInOrOut;
    private float currentConeAngle;
    private float currentRecoilForce;
    private float originalFOV;
    private Coroutine fovCoroutine;
    private bool isSighted = false;

    private void Awake()
    {
        shoot = gunControls.FindActionMap("Gun Controls").FindAction("Shoot");
        reload = gunControls.FindActionMap("Gun Controls").FindAction("Reload");
        zoomInOrOut = gunControls.FindActionMap("Gun Controls").FindAction("Zoom in/out");
        originalFOV = playerCam ? playerCam.fieldOfView : Camera.main.fieldOfView;

        if (playerCam == null)
        {
            playerCam = Camera.main;
        }
    }
    void Start()
    {
        originalRotation = RecoilPivot.localRotation;
        currentConeAngle = coneAngle;
        currentRecoilForce = recoilForce;
        currentAmmo = maxAmmo;
        UpdateAmmoText();

        shoot.Enable();
        reload.Enable();
    }
    void OnEnable()
    {
        shoot.Enable();
        reload.Enable();
        zoomInOrOut.Enable();
        zoomInOrOut.performed += StartSighting;
        zoomInOrOut.canceled += StopSighting;
        isReloading = false;
        ammoText.gameObject.SetActive(true);
        UpdateAmmoText();
    }
    private void OnDisable()
    {
        shoot.Disable();
        reload.Disable();
        zoomInOrOut.Disable();
        zoomInOrOut.performed -= StartSighting;
        zoomInOrOut.canceled -= StopSighting;
        if (ammoText != null)
            ammoText.gameObject.SetActive(false);
    }
    void Update()
    {
        if (!gameObject.activeInHierarchy)
            return;

        if (reload.ReadValue<float>() > 0 && !isSighted && currentAmmo < maxAmmo)
        {
            StartCoroutine(Reload());
        }
        if (currentAmmo == 0 && !isReloading && !isSighted)
        {
            StartCoroutine(Reload());
        }
        if (recoiling > 0)
        {
            float fraction = recoiling / TimeInterval;
            recoiling += Time.deltaTime;
            if (recoiling > TimeInterval)
            {
                recoiling = 0;
                fraction = 1;
                recovering = Time.deltaTime; // Start recovery phase
            }

            DriveRecoil(fraction);
        }
        else if (recovering > 0)
        {
            float fraction = recovering / RecoveryTime;
            recovering += Time.deltaTime;
            if (recovering > RecoveryTime)
            {
                recovering = 0;
                fraction = 1;
            }

            // Smoothly return to original rotation
            RecoilPivot.localRotation = Quaternion.Lerp(RecoilPivot.localRotation, originalRotation, fraction);
        }
        if (isReloading)
            return;

        shoot.performed += ctx =>
        {
            if (canShoot && gameObject.activeInHierarchy)
            {
                Shoot();
                canShoot = false;
            }
        };
    }

    void Shoot()
    {
        // This is important to make semi auto work
        Invoke("CanShootReset", semiAutoShotDelay);
        if (currentAmmo >= 1 && !isReloading && recoiling == 0 && recovering == 0)
        {
            shootClip.Play();
            if (animator != null && nameOfShootTrigger != "") { animator.SetTrigger(nameOfShootTrigger); }
            camController.GunController();
            currentAmmo--;
            UpdateAmmoText();
            recoiling = Time.deltaTime;
            // Spawn and shoot the bullet
            for (var i = 0; i < 6; i++)
            {
                //Debug.Log("shoot");
                GameObject projectile = Instantiate(projectilePrefab, bulletSpawn.transform.position, bulletSpawn.transform.rotation);
                Rigidbody rb = projectile.GetComponent<Rigidbody>();
                Vector3 bulletDirection = GetConeSpreadDirection(bulletSpawn.transform.forward, currentConeAngle);
                rb.velocity = bulletDirection * bulletSpeed;
            }
        }
    }
    void DriveRecoil(float fraction)
    {
        float up = RecoilUp.Evaluate(fraction);
        if (fraction == 0)
        {
            up = 0;
        }
        up *= currentRecoilForce;
        up = -up;
        RecoilPivot.localRotation = Quaternion.Euler(up, 0, 0);
    }
    IEnumerator Reload()
    {
        isReloading = true;
        //yield return new WaitForSeconds(0.5f);
        // reloadClip.Play();
        // gun reload animation
        secondAnimator.Play(nameOfReloadAnim);
        currentAmmo = maxAmmo;
        yield return new WaitForSeconds(reloadTime);
        UpdateAmmoText();
        isReloading = false;
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

    public void StartSighting(InputAction.CallbackContext zoom)
    {
        if (isReloading)
            return;
        isSighted = true;
        //crosshair.SetActive(false);
        characterMovement.canRun = false;
        characterMovement.enableDash = false;
        characterMovement.walkSpeed /= 3;
        camMovement.fov = targetSightZoomFOV;
        currentRecoilForce /= 2f;
        ChangeFOV(targetSightZoomFOV);
        gunManager.canSwitch = false;
        bob.bobForce = 0.0009f;
        bob.bobSpeed = 2f;
        if (secondAnimator != null)
        {
            secondAnimator.Play(nameOfSightAnim);
        }
    }
    public void StopSighting(InputAction.CallbackContext zoom)
    {
        if (isReloading || !isSighted)
            return;
        //crosshair.SetActive(true);
        characterMovement.canRun = true;
        characterMovement.enableDash = true;
        characterMovement.walkSpeed *= 3;
        camMovement.fov = originalFOV;
        currentRecoilForce = recoilForce;
        ChangeFOV(originalFOV);
        gunManager.canSwitch = true;
        bob.bobForce = bob.originalBobForce;
        bob.bobSpeed = bob.originalBobSpeed;
        if (secondAnimator != null)
        {
            secondAnimator.Play(nameOfSightAnimReverse);
        }
        StartCoroutine(SightedBool());
    }
    private IEnumerator SightedBool()
    {
        yield return new WaitForSeconds(0.3f);
        isSighted = false;
    }
    private void ChangeFOV(float newFOV)
    {
        if (fovCoroutine != null)
            StopCoroutine(fovCoroutine);
        fovCoroutine = StartCoroutine(SmoothFOVChange(newFOV));
    }
    private IEnumerator SmoothFOVChange(float newFOV)
    {
        float startFOV = playerCam.fieldOfView;
        float elapsedTime = 0f;
        while (elapsedTime < zoomTransitionDuration)
        {
            elapsedTime += Time.deltaTime;
            playerCam.fieldOfView = Mathf.Lerp(startFOV, newFOV, elapsedTime / zoomTransitionDuration);
            yield return null;
        }
        playerCam.fieldOfView = newFOV;
    }

    void CanShootReset() { canShoot = true; }
    void UpdateAmmoText() { ammoText.text = currentAmmo.ToString() + " / " + maxAmmo.ToString(); }
}