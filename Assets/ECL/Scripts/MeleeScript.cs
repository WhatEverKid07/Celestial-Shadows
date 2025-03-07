using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class MeleeScript : MonoBehaviour
{
    [SerializeField] private float damageAmount;

    private void OnCollisionEnter(Collision collision)
    {
        Target targetScript = collision.gameObject.GetComponent<Target>();

        if (targetScript != null)
        {
            targetScript.TakeDamage(damageAmount);
        }
    }
}