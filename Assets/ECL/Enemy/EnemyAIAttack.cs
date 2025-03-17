using UnityEngine;
using UnityEngine.AI;
using System.Collections;
using Unity.VisualScripting;

public class EnemyAIAttack : MonoBehaviour
{
    public Animator animator;
    public float rangeAttackRadius = 10f;
    public float rangeAttackDamage = 10f;
    public float pauseBetweenEvents = 1f;

    [Header("Ranged Attack")]
    [SerializeField] private float coneAngle;
    [SerializeField] private Vector3 upRecoil;
    [SerializeField] private Vector3 sideRecoil;
    [SerializeField] private float recoilSmoothTime = 0.1f;
    [SerializeField] private float recoilResetSpeed = 5f;
    [SerializeField] private float recoilIncreaseMultiplier = 1.2f;
    [SerializeField] private int maxAmmo = 30;
    [SerializeField] private int shootHowManyBullets = 1;
    [SerializeField] private float reloadTime = 1f;
    [SerializeField] private GameObject bulletSpawn;
    [SerializeField] private GameObject projectilePrefab;
    [SerializeField] private float fireRate = 15f;
    [SerializeField] private float bulletSpeed = 20f;
    [SerializeField] private ParticleSystem muzzleFlash;

    private float nextTimeToFire = 0f;
    private bool isReloading = false;
    private int currentAmmo;
    private Vector3 accumulatedRecoil;
    private Quaternion initialRotation;
    private float recoilVelocityX, recoilVelocityY;
    private Vector3 currentUpRecoil;
    private Vector3 currentSideRecoil;
    private Vector3 currentRecoil = Vector3.zero;
    private Quaternion accumulatedRecoilRotation = Quaternion.identity;
    private float currentConeAngle;



    [Header("Melee Attack")]
    public float meleeAttackDamage = 10f;


    public bool isAttacking = false;

    void Start()
    {

    }
    private void Update()
    {
        if (currentAmmo == 0)
        {
            StartCoroutine(Reload());
            //StopRecoil();
        }
        if (isReloading)
            return;
        if (isAttacking && Time.time >= nextTimeToFire && enabled)
        {
            nextTimeToFire = Time.time + 1f / fireRate;
            Shoot(shootHowManyBullets);
        }
    }
    IEnumerator StartMeleeAttack()
    {
        yield return new WaitForSeconds(pauseBetweenEvents);
        // Animation starts. Will be using blend tree

    }
    private void StartRangeAttack()
    {

    }

    void Shoot(int numberOfBullets)
    {
        if (currentAmmo >= 1)
        {
            if (muzzleFlash != null)
                muzzleFlash.Play();
            //gunAudioSource.PlayOneShot(shootClip); MUST DO

            //animator.SetTrigger(nameOfShootTrigger); MUST DO
            AddRecoil();
            currentAmmo--;
            // Spawn and shoot the bullet
            for (var i = 0; i < numberOfBullets; i++)
            {
                Debug.Log("shoot");
                GameObject projectile = Instantiate(projectilePrefab, bulletSpawn.transform.position, bulletSpawn.transform.rotation);
                Rigidbody rb = projectile.GetComponent<Rigidbody>();
                Vector3 bulletDirection = GetConeSpreadDirection(bulletSpawn.transform.forward, currentConeAngle);
                rb.velocity = bulletDirection * bulletSpeed;
            }
        }
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

    private void HandleRecoil()
    {
        transform.localRotation = initialRotation * accumulatedRecoilRotation;
        accumulatedRecoilRotation = Quaternion.Slerp(accumulatedRecoilRotation, Quaternion.identity, recoilResetSpeed * Time.deltaTime);
    }

    private void AddRecoil()
    {
        float upAmount = Random.Range(-currentUpRecoil.x * recoilIncreaseMultiplier, currentUpRecoil.x * recoilIncreaseMultiplier);
        float sideAmount = Random.Range(-currentSideRecoil.y * recoilIncreaseMultiplier, currentSideRecoil.y * recoilIncreaseMultiplier);
        float tiltAmount = Random.Range(-5f, 5f);

        Quaternion recoilRotation = Quaternion.Euler(upAmount, sideAmount, tiltAmount);

        accumulatedRecoilRotation *= recoilRotation;
    }
    private void StopRecoil()
    {
        if (gameObject.activeInHierarchy)
        {
            StartCoroutine(ResetRecoil());
        }
    }

    private IEnumerator ResetRecoil()
    {
        currentConeAngle = coneAngle;
        currentUpRecoil = upRecoil;
        currentSideRecoil = sideRecoil;

        Vector3 startRecoil = accumulatedRecoil;
        float elapsedTime = 0f;
        float duration = 1f / (recoilResetSpeed * 0.8f);

        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            accumulatedRecoil = Vector3.Lerp(startRecoil, Vector3.zero, elapsedTime / duration);
            yield return null;
        }
        accumulatedRecoil = Vector3.zero;
    }

    IEnumerator Reload()
    {
        //StartCoroutine(ResetRecoil());
        isReloading = true;
        // gunAudioSource.PlayOneShot(reloadClip);
        // gun reload animation
        yield return new WaitForSeconds(reloadTime);
        currentAmmo = maxAmmo;
        isReloading = false;
    }

    private void StopAttack()
    {

    }
}