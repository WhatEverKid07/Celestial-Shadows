using System.Collections;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    [Header("PlayerScripts")]
    [SerializeField] private CharacterMovement characterMoveScript;
    [SerializeField] private PlayerExperience playerXpScript;
    [SerializeField] private NewARScript arScript;
    [SerializeField] private NewPistolScript pistolScript;
    [SerializeField] private NewShotgunScript shotgunScript;
    [SerializeField] private KnifeAnimation knifeScript;

    [SerializeField] private float damage;

    [SerializeField] private float arDamage = 50f;
    [SerializeField] private float shotDamage = 20f;
    [SerializeField] private float pistolDamage = 40f;

    [SerializeField] private ParticleSystem impactEffect;
    [SerializeField] private float lifeTime = 5f;
    [SerializeField] private LayerMask ignoredLayers;

    private void Start()
    {
        Destroy(gameObject, lifeTime);
    }

    private void Update()
    {
        //UpdateCurrentGun();
    }

    /*
    private void UpdateCurrentGun()
    {
        if (arScript.isActiveAndEnabled)
        {
            damage = arDamage;
        }
        if (pistolScript.isActiveAndEnabled)
        {
            damage = pistolDamage;
        }
        else if (shotgunScript.isActiveAndEnabled)
        {
            damage = shotDamage;
        }
        else
        {
            Debug.LogWarning("Not a gun!");
        }
    }
    */
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
