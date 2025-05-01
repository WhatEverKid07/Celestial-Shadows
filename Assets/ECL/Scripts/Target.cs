using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class Target : MonoBehaviour
{
    [SerializeField] private float maxHealth;
    [SerializeField] internal float health;
    [SerializeField] private Slider healthBar;
    [SerializeField] private GameObject shadowBody;
    [SerializeField] private GameObject shadowAttack;
    [SerializeField] private ParticleSystem deathParticles;

    internal bool addXp;

    private void Start()
    {
        addXp = false;
        health = maxHealth;
        healthBar.maxValue = maxHealth;
        healthBar.value = maxHealth;
    }
    private void FixedUpdate()
    {
        if (healthBar != null) { UpdateHealthbar();}
    }

    public void TakeDamage(float amount)
    {
        Debug.Log("Hit! " + amount);
        health -= amount;
        if (health <= 0f)
        {
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
            addXp = true;
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

    private void UpdateHealthbar()
    {
        healthBar.value = health;
    }
}