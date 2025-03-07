using UnityEngine;

public class EnemyCombat : MonoBehaviour
{
    public bool isMelee;
    public float attackRange = 2f;
    public float fireRate = 1f;
    private float nextAttackTime;

    public GameObject bulletPrefab;
    public Transform gunBarrel;

    public void Attack(Transform target)
    {
        if (Time.time >= nextAttackTime)
        {
            if (isMelee)
                MeleeAttack(target);
            else
                RangedAttack(target);

            nextAttackTime = Time.time + fireRate;
        }
    }

    void MeleeAttack(Transform target)
    {
        Debug.Log("Melee Attack on " + target.name);
    }

    void RangedAttack(Transform target)
    {
        GameObject bullet = Instantiate(bulletPrefab, gunBarrel.position, Quaternion.identity);
        bullet.GetComponent<Rigidbody>().velocity = (target.position - gunBarrel.position).normalized * 20f;
    }
}