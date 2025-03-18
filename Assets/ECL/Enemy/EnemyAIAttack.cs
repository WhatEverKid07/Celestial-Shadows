using UnityEngine;
using UnityEngine.AI;
using System.Collections;
using Unity.VisualScripting;

public class EnemyAIAttack : MonoBehaviour
{
    [SerializeField] private Animator animator;
    [SerializeField] private float pauseBetweenEvents = 1f;

    [Header("Attack Mode")]
    [SerializeField] private bool melee = false;
    [SerializeField] private bool range = false;

    [Tooltip("For Testing")]
    public bool isAttacking = false; // Public for testing

    [Header("Ranged Attack")]
    [SerializeField] private float coneAngle;
    [SerializeField] private Vector3 upRecoil;
    [SerializeField] private Vector3 sideRecoil;
    [SerializeField] private float recoilSmoothTime = 0.1f;
    [SerializeField] private float recoilResetSpeed = 5f;
    [SerializeField] private float recoilIncreaseMultiplier = 1.2f;
    [Space(10)]
    //[SerializeField] private int maxAmmo = 30;
    [SerializeField] private float semiAutoShotDelay = 0.2f;
    //[SerializeField] private int shootHowManyBullets = 1;
    //[SerializeField] private float fireRate = 15f;
    [SerializeField] private float bulletSpeed = 20f;
    //[SerializeField] private float reloadTime = 1f;
    [SerializeField] private float rangeAttackRadius = 10f;
    [SerializeField] private float rangeAttackDamage = 10f;
    [Space(10)]
    [SerializeField] private GameObject bulletSpawn;
    [SerializeField] private GameObject projectilePrefab;
    [SerializeField] private ParticleSystem muzzleFlash;

    private bool canShoot = true;
    private float nextTimeToFire = 0f;
    //private bool isReloading = false;
    //private int currentAmmo;
    private Quaternion accumulatedRecoilRotation = Quaternion.identity;
    private Quaternion initialRotation;
    private float currentConeAngle;


    [Header("Melee Attack")]
    [SerializeField] private float meleeAttackDamage = 10f;
    [SerializeField] private float meleeAttackRange = 2f;

    private void Awake()
    {
        if (melee && range || !melee && !range)
        {
            Debug.LogError("Both modes are the same value!");
        }
    }
    private void Start()
    {
        if (range)
        {
            //currentAmmo = maxAmmo;
            initialRotation = transform.localRotation;
        }
    }

    private void Update()
    {
        if (range)
        {
            if (isAttacking && canShoot && enabled)
            {
                Shoot();
                canShoot = false;
            }
        }
        if (melee)
        {

        }
    }

    private void Shoot()
    {
        Invoke("CanShootReset", semiAutoShotDelay);
        GameObject projectile = Instantiate(projectilePrefab, bulletSpawn.transform.position, bulletSpawn.transform.rotation);
        Rigidbody rb = projectile.GetComponent<Rigidbody>();
        Vector3 bulletDirection = GetConeSpreadDirection(bulletSpawn.transform.forward, currentConeAngle);
        rb.velocity = bulletDirection * bulletSpeed;
        muzzleFlash?.Play();
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
        return Quaternion.LookRotation(forwardDirection) * randomSpread;
    }

    private void CanShootReset()
    {
        canShoot = true;
    }

    /*
    private void HandleRecoil()
    {
        transform.localRotation = initialRotation * accumulatedRecoilRotation;
        accumulatedRecoilRotation = Quaternion.Slerp(accumulatedRecoilRotation, Quaternion.identity, recoilResetSpeed * Time.deltaTime);
    }

    private void AddRecoil()
    {
        float upAmount = Random.Range(-upRecoil.x * recoilIncreaseMultiplier, upRecoil.x * recoilIncreaseMultiplier);
        float sideAmount = Random.Range(-sideRecoil.y * recoilIncreaseMultiplier, sideRecoil.y * recoilIncreaseMultiplier);
        float tiltAmount = Random.Range(-5f, 5f);

        Quaternion recoilRotation = Quaternion.Euler(upAmount, sideAmount, tiltAmount);
        accumulatedRecoilRotation *= recoilRotation;
    }

    private IEnumerator Reload()
    {
        isReloading = true;
        yield return new WaitForSeconds(reloadTime);
        currentAmmo = maxAmmo;
        isReloading = false;
    }
    */
}
