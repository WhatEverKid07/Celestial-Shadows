using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class NewARScript : MonoBehaviour
{
    [SerializeField] private CameraMovement camMovement;
    [SerializeField] private CameraController camController;
    [SerializeField] private GunManagement gunManager;
    [SerializeField] private CameraHeadBob bob;
    [SerializeField] private CharacterMovement characterMovement;

    [Header("Gun Attributes")]
    [SerializeField] internal int maxAmmo = 30;
    [SerializeField] internal float reloadTime = 1f;
    [SerializeField] private float coneAngle;
    [SerializeField] private float targetSightZoomFOV = 40f;
    [SerializeField] private float zoomTransitionDuration = 1f;

    private bool isReloading = false;
    private bool canShoot = true;
    private int currentAmmo;


    [Space(20)]
    [Header("Recoil")]
    [SerializeField] private float recoilForce;
    [SerializeField] private AnimationCurve RecoilUp;
    [Tooltip("How long is recovery sequence?")]
    [SerializeField] private float RecoveryTime = 0.25f;
    [Tooltip("Which object is having its .localRotation driven.")]
    [SerializeField] private Transform RecoilPivot;
    [SerializeField] private float maxRecoil = 15f;

    public float fraction;
    private bool isFiring = false;
    private float recoilAmount = 0f;

    [Space(20)]
    [Header("Shooting Attributes")]
    [SerializeField] private GameObject bulletSpawn;
    [SerializeField] private GameObject projectilePrefab;
    [SerializeField] internal float fireRate = 15f;
    [SerializeField] private float bulletSpeed = 20f;

    private float nextTimeToFire = 0f;

    [Space(20)]
    [Header("Animation")]
    [SerializeField] private Animator animator;
    [SerializeField] private Animator secondAnimator;
    [SerializeField] internal float sAnimSpeed = 1f;
    [SerializeField] private float normalAnimSpeed = 1f;

    [SerializeField] private string nameOfReloadAnim;
    [SerializeField] private string nameOfSightAnim;
    [SerializeField] private string nameOfSightAnimReverse;
    [SerializeField] private string nameOfTriggerBool;
    [SerializeField] private string nameOfTriggerTrigger;

    [Space(20)]
    [Header("Effects")]
    [SerializeField] private Camera playerCam;

    [Space(20)]
    [Header("UI")]
    [SerializeField] private Text ammoText;

    /*[Space(20)]
    [Header("Audio")]
    [SerializeField] private AudioSource shootClip;
    [SerializeField] private AudioSource reloadClip;*/

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
    private bool canShootAnimation = true;

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
        {
            return;
        }

        if (reload.ReadValue<float>() > 0 && !isSighted && currentAmmo < maxAmmo)
        {
            gunManager.canSwitch = false;
            StartCoroutine(Reload());
            secondAnimator.speed = sAnimSpeed;
        }

        if (currentAmmo == 0 && !isReloading && !isSighted)
        {
            gunManager.canSwitch = false;
            StartCoroutine(Reload());
            secondAnimator.speed = sAnimSpeed;
        }

        if (!isFiring && recoilAmount > 0)
        {
            recoilAmount = Mathf.MoveTowards(recoilAmount, 0, RecoveryTime * Time.deltaTime);
            ApplyRecoil();
        }

        if (isReloading)
        {
            return;
        }
        else
        {
            secondAnimator.speed = normalAnimSpeed;
        }

        if (shoot.ReadValue<float>() > 0 && Time.time >= nextTimeToFire)
        {
            nextTimeToFire = Time.time + 1f / fireRate;
            Shoot();
            if (nameOfTriggerBool != "" && canShootAnimation)
            {
                canShootAnimation = false;
                animator.SetTrigger(nameOfTriggerTrigger);
                animator.SetBool(nameOfTriggerBool, true);
            }
        }
        else {isFiring = false;}

        shoot.canceled += ctx => {
            if (nameOfTriggerBool != "")
            { animator.SetBool(nameOfTriggerBool, false); canShootAnimation = true; }
        };
    }
    public void StartSighting(InputAction.CallbackContext zoom)
    {
        if (isReloading)
            return;
        isSighted = true;
        characterMovement.canRun = false;
        characterMovement.enableDash = false;
        characterMovement.walkSpeed /= 3;
        camMovement.fov = targetSightZoomFOV;
        currentConeAngle = 0.5f;
        currentRecoilForce /= 1.5f; 
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
        characterMovement.canRun = true;
        characterMovement.enableDash = true;
        characterMovement.walkSpeed *= 3;
        camMovement.fov = originalFOV;
        currentConeAngle = coneAngle;
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
        yield return new WaitForSeconds(0.5f);
        isSighted = false;
    }
    void Shoot()
    {
        if (currentAmmo > 0 && !isReloading)
        {
            AudioManager.instance.AssaultRifleShoot();
            camController.GunController();
            currentAmmo--;
            UpdateAmmoText();

            isFiring = true;

            recoilAmount = Mathf.Min(recoilAmount + currentRecoilForce, maxRecoil);
            ApplyRecoil();

            GameObject projectile = Instantiate(projectilePrefab, bulletSpawn.transform.position, bulletSpawn.transform.rotation);
            Rigidbody rb = projectile.GetComponent<Rigidbody>();
            rb.velocity = GetConeSpreadDirection(bulletSpawn.transform.forward, currentConeAngle) * bulletSpeed;
        }
    }
    void ApplyRecoil()
    {
        float up = -recoilAmount;
        RecoilPivot.localRotation = Quaternion.Euler(up, 0, 0);
    }
    IEnumerator Reload()
    {
        isReloading = true;
        isFiring = false;
        if (nameOfReloadAnim != "") 
        { 
            secondAnimator.Play(nameOfReloadAnim);
        }
        currentAmmo = maxAmmo;
        yield return new WaitForSeconds(reloadTime);
        UpdateAmmoText();
        isReloading = false;
        gunManager.canSwitch = true;
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
    void UpdateAmmoText() { ammoText.text = currentAmmo.ToString() + " / " + maxAmmo.ToString(); }
}