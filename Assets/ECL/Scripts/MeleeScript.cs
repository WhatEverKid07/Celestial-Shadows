using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class MeleeScript : MonoBehaviour
{
    [SerializeField] private float damageAmount;
    [SerializeField] private LayerMask ignoredLayers; // Layers to ignore

    private void OnCollisionEnter(Collision collision)
    {
        if (!gameObject.activeInHierarchy)
            return;
        if (((1 << collision.gameObject.layer) & ignoredLayers) != 0)
        {
            return; // Ignore collision
        }
        Target targetScript = collision.gameObject.GetComponent<Target>();

        if (targetScript != null)
        {
            targetScript.TakeDamage(damageAmount);
            AudioManager.instance.KnifeHit();
        }
    }
}