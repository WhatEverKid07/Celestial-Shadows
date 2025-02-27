using UnityEngine;

public class Bullet : MonoBehaviour
{
    public float damage = 10f;           // Damage dealt by the projectile
    public GameObject impactEffect;      // Effect shown on collision
    public float impactForce = 30f;      // Force applied to the object hit
    public float lifeTime = 5f;          // Lifetime of the projectile (to auto-destroy)

    private void Start()
    {
        // Destroy the projectile after a certain time if it doesn't hit anything
        Destroy(gameObject, lifeTime);
    }

    private void OnCollisionEnter(Collision collision)
    {
        // Check if the object has a 'Target' component (or any other health system)
        Target target = collision.transform.GetComponent<Target>();
        if (target != null)
        {
            target.TakeDamage(damage);  // Apply damage to the target
        }

        // Apply force to the object hit if it has a Rigidbody
        
        if (collision.rigidbody != null)
        {
            collision.rigidbody.AddForce(-collision.contacts[0].normal * impactForce, ForceMode.Impulse);
        }
        

        // Instantiate the impact effect at the point of collision
        /*
        if (impactEffect != null)
        {
            GameObject impactGO = Instantiate(impactEffect, collision.contacts[0].point, Quaternion.LookRotation(collision.contacts[0].normal));
            Destroy(impactGO, 2f);  // Destroy the impact effect after 2 seconds
        }
        */

        // Destroy the projectile after impact
        Destroy(gameObject);
    }
}
