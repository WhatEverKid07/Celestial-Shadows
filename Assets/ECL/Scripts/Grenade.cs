using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.ParticleSystem;

public class Grenade : MonoBehaviour
{
    [SerializeField] private float grenadeFuse = 3;
    [SerializeField] private float radius = 5f;
    [SerializeField] private float grenadeForce = 700f;
    [SerializeField] private float upwardsGrenadeForce = 700f;
    [SerializeField] private GameObject explosionEffect;
    [SerializeField] private AudioSource explosionSound;
    [SerializeField] private float maxDamage = 100f;

    [SerializeField] private GameObject[] grenadePieces;

    public void StartFuse()
    {
        StartCoroutine(Fuse(grenadeFuse));
    }
    IEnumerator Fuse(float fuseLength)
    {
        //Debug.Log("Works");
        yield return new WaitForSeconds(fuseLength);
        Explode();
        yield return new WaitForSeconds(explosionSound.clip.length);
        Destroy(gameObject);
    }
    private void Explode()
    {
        Collider[] hitColliders = new Collider[10];
        int collidersHit = Physics.OverlapSphereNonAlloc(transform.position, radius, hitColliders);

        for (int i = 0; i < collidersHit; i++)
        {
            if (hitColliders[i].TryGetComponent(out Rigidbody rb))
            {
                rb.AddExplosionForce(grenadeForce, transform.position, radius, upwardsGrenadeForce);
            }

            if (hitColliders[i].TryGetComponent(out Target enemy))
            {
                float damage = maxDamage - Vector3.Distance(transform.position, hitColliders[i].transform.position) * (maxDamage / radius);
                if (damage > 0)
                {
                    enemy.TakeDamage(damage);
                }

            }
        }

        Instantiate(explosionEffect, transform.position, transform.rotation);
        AudioManager.instance.GrenadeExplode();
        foreach(GameObject objects in grenadePieces)
        {
            objects.SetActive(false);
        }
    }
}