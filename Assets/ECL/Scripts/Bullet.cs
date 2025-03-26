using UnityEngine;

public class Bullet : MonoBehaviour
{
    [SerializeField] private float damage = 10f;
    [SerializeField] private GameObject impactEffect;
    [SerializeField] private float lifeTime = 5f;
    [SerializeField] private LayerMask ignoredLayers; // Layers to ignore

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
                GameObject impactGO = Instantiate(
                    impactEffect,
                    collision.contacts[0].point,
                    Quaternion.LookRotation(collision.contacts[0].normal)
                );
                Destroy(impactGO, 2f);
            }

            Destroy(gameObject);
        }
    }
}
