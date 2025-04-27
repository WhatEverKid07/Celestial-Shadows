using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAttackArea : MonoBehaviour
{
    [SerializeField] float damage;
    private Target playerTargetScript;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (other.TryGetComponent<Target>(out Target enemy))
            {
                playerTargetScript = enemy;
            }
        }
    }
    internal void Attack()
    {
        playerTargetScript.TakeDamage(damage);
    }
}