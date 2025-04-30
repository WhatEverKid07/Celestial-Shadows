using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

public class Target : MonoBehaviour
{
    [SerializeField] internal float health = 50f;
    [SerializeField] private GameObject shadowBody;
    [SerializeField] private GameObject shadowAttack;
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
        if (shadowBody != null && deathParticles != null && shadowAttack != null)
        {
            deathParticles.Play();
            shadowBody.SetActive(false);
            shadowAttack.SetActive(false);
            StartCoroutine(Die2(4));
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
