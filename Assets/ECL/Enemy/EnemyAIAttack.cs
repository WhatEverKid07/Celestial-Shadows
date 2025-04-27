using UnityEngine;
using UnityEngine.AI;
using System.Collections;
using Unity.VisualScripting;

public class EnemyAIAttack : MonoBehaviour
{
    [SerializeField] private float pauseBetweenEvents = 1f;

    [Header("Attack Mode")]
    [SerializeField] internal bool melee = false;
    [SerializeField] internal bool range = false;

    [Tooltip("For Testing")]
    internal bool isAttacking = false; // Public for testing. Make Internal

    [Header("Ranged Attack")]
    [SerializeField] private float coneAngle;

    [Space(10)]
    [SerializeField] private float semiAutoShotDelay = 0.2f;
    [SerializeField] private float bulletSpeed = 20f;
    [SerializeField] private float rangeAttackRadius = 10f;
    [Space(10)]
    [SerializeField] private GameObject bulletSpawn;
    [SerializeField] private GameObject projectilePrefab;
    [SerializeField] private ParticleSystem muzzleFlash;
    
    internal Transform player;
    private bool canShoot = true;
    private float nextTimeToFire = 0f;
    private float currentConeAngle;



    [Header("Melee Attack")]
    [SerializeField] private float meleeAttackDamage = 10f;
    [SerializeField] private float meleeAttackRange = 2f;
    [SerializeField] private Animator attackAnim;
    [SerializeField] private string nameOfAttackTrigger;
    [SerializeField] private string nameOfWalkingBool;
    [SerializeField] private EnemyAttackArea enemyAttackArea;

    private bool canAttack = true;
    internal bool walking = false;

    private void Awake()
    {
        if (melee && range || !melee && !range)
        {
            Debug.LogError("Both modes are the same value!");
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
            if (isAttacking && !walking && enabled)
            {
                StartCoroutine(Attack());
            }
        }
    }
    internal void Walk()
    {
        attackAnim.SetTrigger(nameOfWalkingBool);
    }
    private IEnumerator Attack()
    {
        if (canAttack)
        {
            canAttack = false;
            attackAnim.SetTrigger(nameOfAttackTrigger);
            Debug.Log("attack");
            yield return new WaitForSeconds(0.5f);
            enemyAttackArea.Attack();
            yield return new WaitForSeconds(1.13f);
            canAttack = true;
        }
    }

    private void Shoot()
    {
        FacePlayer();
        muzzleFlash?.Play();
        Invoke("CanShootReset", semiAutoShotDelay);
        GameObject projectile = Instantiate(projectilePrefab, bulletSpawn.transform.position, bulletSpawn.transform.rotation);
        Rigidbody rb = projectile.GetComponent<Rigidbody>();
        Vector3 bulletDirection = GetConeSpreadDirection(bulletSpawn.transform.forward, currentConeAngle);
        rb.velocity = bulletDirection * bulletSpeed;
        //muzzleFlash?.Stop();
    }
    public void FacePlayer()
    {
        Vector3 direction = player.transform.position - bulletSpawn.transform.position;
        if (direction == Vector3.zero) return;

        Quaternion targetRotation = Quaternion.LookRotation(direction);
        bulletSpawn.transform.rotation = targetRotation;
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
    private void CanShootReset(){canShoot = true;}
}