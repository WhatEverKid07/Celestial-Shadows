using UnityEngine;

public class PlayerExperience : MonoBehaviour
{
    [Header("Scripts")]

    [Header("XP")]
    private float maxXp;
    private float previousMaxXp;
    private int xpLvl;
    private float currentXp;

    private void Start()
    {
        xpLvl = 0;
        currentXp = 0;
        maxXp = 100;
        previousMaxXp = maxXp - 100;
    }

    private void Update()
    {
        if (currentXp >= maxXp)
        {
            xpLvl++;
            maxXp += 100;
            previousMaxXp = maxXp - 100;
            currentXp = currentXp - previousMaxXp;
        }
    }

    private void AddExperience()
    {

    }
}
