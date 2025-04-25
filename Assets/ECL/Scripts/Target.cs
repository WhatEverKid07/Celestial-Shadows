using UnityEngine;

public class Target : MonoBehaviour
{
    [SerializeField] private float health = 50f;
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
        Destroy(gameObject);
    }

    private void AddExperience()
    {
        Debug.Log("Add experience.");
    }
}
