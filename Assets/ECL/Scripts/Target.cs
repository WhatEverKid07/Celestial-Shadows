using System.Collections;
using UnityEngine;

public class Target : MonoBehaviour
{
    [SerializeField] private float health = 50f;
    [SerializeField] private GameObject shadowBody;
    [SerializeField] private ParticleSystem deathParticles;
    public bool addXp {get; private set;}

    private void Start()
    {
        addXp = false;
    }

    public void TakeDamage(float amount)
    {
        Debug.Log("Hit! " + amount);
        health -= amount;
        if (health <= 0f)
        {
            Invoke(nameof(AddExperience), .01f);
            Die();
        }
    }

    private void Die()
    {
        if (shadowBody != null && deathParticles != null)
        {
            deathParticles.Play();
            shadowBody.SetActive(false);
            StartCoroutine(Die2(deathParticles.main.duration));
        }
        else
        {
            StartCoroutine(Die2(0));
        }
    }
    
    private IEnumerator Die2(float waitTime)
    {
        yield return new WaitForSeconds(waitTime);
        Destroy(gameObject);
    }

    private void AddExperience()
    {
        Debug.Log("Add experience.");
    }
}
