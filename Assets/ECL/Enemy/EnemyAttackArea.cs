using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAttackArea : MonoBehaviour
{
    [SerializeField] float damage;
    public Target playerTargetScript; // private

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("Triggered!");
        if (other.CompareTag("Player") || other.CompareTag("Target"))
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