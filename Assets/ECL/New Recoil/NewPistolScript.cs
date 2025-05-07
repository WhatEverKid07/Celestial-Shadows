using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class NewPistolScript : MonoBehaviour
{
    [SerializeField] private CameraMovement camMovement;
    [SerializeField] private CameraController camController;
    [SerializeField] private GunManagement gunManager;
    [SerializeField] private CameraHeadBob bob;
    [SerializeField] private CharacterMovement characterMovement;

    [Header("Gun Attributes")]
    [SerializeField] private int maxAmmo = 30;
    [SerializeField] internal float reloadTime;
    [SerializeField] private float coneAngle;
    [SerializeField] private float semiAutoShotDelay = 0.2f;

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
    [SerializeField] internal float fireRate = 15f;
    [SerializeField] private float bulletSpeed = 20f;

    private float nextTimeToFire = 0f;

    [Space(20)]
    [Header("Animation")]
    [SerializeField] private Animator animator;
    [SerializeField] internal float animSpeed = 1f;
    [SerializeField] private float normalAnimSpeed = 1f;

    [SerializeField] private string nameOfShootTrigger;
    [SerializeField] private string nameOfReloadTrigger;

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
    private bool canShootAnimation = true;

    private Vector3 currentRecoil = Vector3.zero;
    private Quaternion accumulatedRecoilRotation = Quaternion.identity;

    private void Awake()
    {
        shoot = gunControls.FindActionMap("Gun Controls").FindAction("Shoot");
        reload = gunControls.FindActionMap("Gun Controls").FindAction("Reload");

        if (playerCam == null)
        {
            playerCam = Camera.main;
        }
    }
    void Start()
    {
        originalRotation = RecoilPivot.localRotation;
        currentConeAngle = coneAngle;

        currentAmmo = maxAmmo;
        UpdateAmmoText();

        shoot.Enable();
        reload.Enable();
    }
    void OnEnable()
    {
        shoot.Enable();
        reload.Enable();
        isReloading = false;
        ammoText.gameObject.SetActive(true);
        UpdateAmmoText();
    }
    private void OnDisable()
    {
        shoot.Disable();
        reload.Disable();
        if (ammoText != null)
            ammoText.gameObject.SetActive(false);
    }
    void Update()
    {
        if (!gameObject.activeInHierarchy)
        { 
            return; 
        }

        if (reload.ReadValue<float>() > 0)
        {
            gunManager.canSwitch = false;
            StartCoroutine(Reload());
            animator.speed = animSpeed;
        }
        if (currentAmmo == 0 && !isReloading)
        {
            gunManager.canSwitch = false;
            StartCoroutine(Reload());
            animator.speed = animSpeed;
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
        { 
            return; 
        }
        else
        {
            animator.speed = normalAnimSpeed;
        }

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
            AudioManager.instance.PistolShoot();
            if (animator != null && nameOfShootTrigger != "") { animator.SetTrigger(nameOfShootTrigger); }
            camController.GunController();
            currentAmmo--;
            UpdateAmmoText();
            recoiling = Time.deltaTime;
            // Spawn and shoot the bullet
            for (var i = 0; i < 1; i++)
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
        up *= recoilForce;
        up = -up;
        RecoilPivot.localRotation = Quaternion.Euler(up, 0, 0);
    }
    IEnumerator Reload()
    {
        isReloading = true;
        // reloadClip.Play();
        // gun reload animation
        if (animator != null && nameOfReloadTrigger != "") { animator.SetTrigger(nameOfReloadTrigger); }
        yield return new WaitForSeconds(reloadTime);
        currentAmmo = maxAmmo;
        UpdateAmmoText();
        isReloading = false;
        yield return new WaitForSeconds(0.4f);
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
    void CanShootReset(){canShoot = true;}
    void UpdateAmmoText(){ammoText.text = currentAmmo.ToString() + " / " + maxAmmo.ToString();}
}