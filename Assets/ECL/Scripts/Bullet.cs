using System.Collections;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    [SerializeField] internal float damage;

    [SerializeField] internal float arDamage = 50f;
    [SerializeField] internal float shotDamage = 40f;
    [SerializeField] internal float pistolDamage = 40f;

    [SerializeField] private ParticleSystem impactEffect;
    [SerializeField] private float lifeTime = 5f;
    [SerializeField] private LayerMask ignoredLayers;

    private void Start()
    {
        Destroy(gameObject, lifeTime);
    }

    private void OnCollisionEnter(Collision collision)
    {
        // Check if the collided object's layer is in the ignored layers list
        if (((1 << collision.gameObject.layer) & ignoredLayers) != 0)
        {
            return; // Ignore collision
        }

        if (!collision.gameObject.CompareTag("Bullet"))
        {
            Target target = collision.transform.GetComponent<Target>();
            if (target != null)
            {
                target.TakeDamage(damage);
            }

            if (impactEffect != null)
            {
                MeshRenderer renderer = impactEffect.GetComponentInParent<MeshRenderer>();
                renderer.enabled = false;
                Rigidbody rb = GetComponent<Rigidbody>();
                rb.isKinematic = true;
                Collider collider = GetComponent<Collider>();
                collider.enabled = false;
                
                /*GameObject impactGO = Instantiate(
                    impactEffect,
                    collision.contacts[0].point,
                    Quaternion.LookRotation(collision.contacts[0].normal)
                );*/
                //Destroy(impactGO, 2f);

                impactEffect.Play();
                StartCoroutine(DestroyAfterDelay(5f));
                return;
            }

            Destroy(gameObject);
        }
    }
    private IEnumerator DestroyAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        Destroy(gameObject);
    }
}
