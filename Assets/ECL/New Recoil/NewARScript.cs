using System.Collections;
using Unity.VisualScripting;
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
    [SerializeField] private int maxAmmo = 30;
    [SerializeField] private float reloadTime = 1f;
    [SerializeField] private float coneAngle;
    [SerializeField] private float semiAutoShotDelay = 0.2f;

    private bool isReloading = false;
    private bool canShoot = true;
    private int currentAmmo;

    private Vector3 accumulatedRecoil;
    private Quaternion initialRotation;
    private float recoilVelocityX, recoilVelocityY;
    private Vector3 currentUpRecoil;
    private Vector3 currentSideRecoil;

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
    private Quaternion targetRotation;

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
    [SerializeField] private string nameOfShootTrigger;
    [SerializeField] private string nameOfReloadTrigger;

    [Space(20)]
    [Header("Effects")]
    [SerializeField] private Camera playerCam;

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

        if (playerCam == null) { playerCam = Camera.main; }
    }
    void Start()
    {
        originalRotation = RecoilPivot.localRotation;
        targetRotation = originalRotation;
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
        if (!gameObject.activeInHierarchy) return;

        if (reload.ReadValue<float>() > 0) StartCoroutine(Reload());
        if (currentAmmo == 0 && !isReloading) StartCoroutine(Reload());

        if (isReloading) return;

        if (shoot.ReadValue<float>() > 0 && Time.time >= nextTimeToFire)
        {
            nextTimeToFire = Time.time + (1f / fireRate);
            Shoot();
        }
        HandleRecoil();
    }
    void Shoot()
    {
        if (currentAmmo <= 0) return;
        currentAmmo--;
        UpdateAmmoText();
        gunAudioSource.PlayOneShot(shootClip);
        if (animator) animator.SetTrigger(nameOfShootTrigger);

        // Fire Bullet
        GameObject projectile = Instantiate(projectilePrefab, bulletSpawn.transform.position, bulletSpawn.transform.rotation);
        Rigidbody rb = projectile.GetComponent<Rigidbody>();
        rb.velocity = bulletSpawn.transform.forward * bulletSpeed;

        // Apply Recoil
        accumulatedRecoil += Vector3.up * recoilForce;
        recoiling = 0f;
    }
    void HandleRecoil()
    {
        if (recoiling < TimeInterval)
        {
            recoiling += Time.deltaTime;
            float fraction = recoiling / TimeInterval;
            float up = -RecoilUp.Evaluate(fraction) * recoilForce;
            targetRotation = Quaternion.Euler(up, 0, 0) * originalRotation;
        }
        else if (recovering < RecoveryTime)
        {
            recovering += Time.deltaTime;
            float fraction = recovering / RecoveryTime;
            targetRotation = Quaternion.Lerp(targetRotation, originalRotation, fraction);
        }
        else
        {
            recovering = 0f;
            accumulatedRecoil = Vector3.zero;
            targetRotation = originalRotation;
        }

        RecoilPivot.localRotation = Quaternion.Lerp(RecoilPivot.localRotation, targetRotation, Time.deltaTime * 10f);
    }
    IEnumerator Reload()
    {
        isReloading = true;
        // gunAudioSource.PlayOneShot(reloadClip);
        // gun reload animation
        if (animator != null && nameOfReloadTrigger != "") { animator.SetTrigger(nameOfReloadTrigger); }
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
    void CanShootReset() { canShoot = true; }
    void UpdateAmmoText() { ammoText.text = currentAmmo.ToString() + " / " + maxAmmo.ToString(); }
}