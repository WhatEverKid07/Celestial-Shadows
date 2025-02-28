using UnityEngine;

public class Bullet : MonoBehaviour
{
    public float damage = 10f;
    public GameObject impactEffect;
    public float impactForce = 30f;
    public float lifeTime = 5f;

    private void Start()
    {
        Destroy(gameObject, lifeTime);
    }

    private void OnCollisionEnter(Collision collision)
    {
        Target target = collision.transform.GetComponent<Target>();
        if (target != null)
        {
            target.TakeDamage(damage);
        }
        
        if (collision.rigidbody != null)
        {
            collision.rigidbody.AddForce(-collision.contacts[0].normal * impactForce, ForceMode.Impulse);
        }
        if (impactEffect != null)
        {
            GameObject impactGO = Instantiate(impactEffect, collision.contacts[0].point, Quaternion.LookRotation(collision.contacts[0].normal));
            Destroy(impactGO, 2f);
        }
        Destroy(gameObject);
    }
}
