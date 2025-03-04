using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grenade : MonoBehaviour
{
    [SerializeField] private float grenadeFuse = 3;
    [SerializeField] private float radius = 5f;
    [SerializeField] private float grenadeForce = 700f;
    [SerializeField] private GameObject explosionEffect;
    public void StartFuse()
    {
        StartCoroutine(Fuse(grenadeFuse));
    }
    IEnumerator Fuse(float fuseLength)
    {
        Debug.Log("Works");
        yield return new WaitForSeconds(fuseLength);
        Explode();
    }

    void Explode()
    {
        Instantiate(explosionEffect, transform.position, transform.rotation);
        /*
        Collider[] colliders = Physics.OverlapSphere(transform.position, radius);

        foreach (Collider nearbyObject in colliders)
        {
            Rigidbody rb = nearbyObject.GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.AddExplosionForce(grenadeForce, transform.position, radius);
            }
        }
        */
        Destroy(gameObject);
    }
}