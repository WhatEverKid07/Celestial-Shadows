using UnityEngine;
using UnityEngine.AI;
using System.Collections;
using Unity.VisualScripting;

public class EnemyAIAttack : MonoBehaviour
{
    public Animator animator;
    public float rangeAttackRadius = 10f;
    public float rangeAttackDamage = 10f;
    public float meleeAttackDamage = 10f;
    public float pauseBetweenEvents = 1f;

    private bool isAttacking = false;

    void Start()
    {

    }
    private void Update()
    {

    }
    IEnumerator StartMeleeAttack()
    {
        yield return new WaitForSeconds(pauseBetweenEvents);
        // Animation starts. Will be using blend tree

    }
    private void StartRangeAttack()
    {

    }
    private void StopAttack()
    {

    }
}