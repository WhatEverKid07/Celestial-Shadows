using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class Target : MonoBehaviour
{
    [SerializeField] private PlayerDeathManager playerDeath;
    [SerializeField] private float maxHealth;
    [SerializeField] internal float health;
    [SerializeField] private Slider healthBar;
    [SerializeField] private GameObject shadowBody;
    [SerializeField] private GameObject shadowAttack;
    [SerializeField] private ParticleSystem deathParticles;

    [Space(20)]
    [SerializeField] private AudioSource deathSound;
    [SerializeField] private AudioSource hitSound;

    private Coroutine xpGain;
    internal bool canLoseDamage;
    internal bool addXp;

    private void Start()
    {
        canLoseDamage = true;
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
        if (canLoseDamage)
        {
            Debug.Log("Hit! " + amount);
            health -= amount;
            if (hitSound != null) { hitSound.Play(); }
            if (health <= 0f)
            {
                Die();
            }
        }
    }

    private void Die()
    {
        if (shadowBody != null && deathParticles != null && shadowAttack != null && gameObject.GetComponent<Collider>() != null)
        {
            deathParticles.Play();
            shadowBody.SetActive(false);
            shadowAttack.SetActive(false);
            gameObject.GetComponent<Collider>().enabled = false;
            xpGain = StartCoroutine(CancelXPGain());
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
        if (deathSound != null) { deathSound.Play(); }
        if (playerDeath != null) { playerDeath.ScreenEnable(); }
        Destroy(gameObject);
    }

    private IEnumerator CancelXPGain()
    {
        addXp = true;

        yield return new WaitForSeconds(.001f);

        addXp = false;
    }

    private void UpdateHealthbar()
    {
        healthBar.value = health;
    }
}