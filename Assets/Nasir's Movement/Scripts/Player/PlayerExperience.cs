using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;

public class PlayerExperience : MonoBehaviour
{
    [Header("Enemies")]
    [SerializeField] private List<GameObject> enemies;
    [SerializeField] private List<Target> allTargets;

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
        enemies = GameObject.FindGameObjectsWithTag("Enemy").ToList();
        for (int i = 0; i < enemies.Count; i++)
        {
            var enemyTarget = enemies[i].GetComponent<Target>();
            if (!allTargets.Contains(enemyTarget))
            {
                allTargets.Add(enemyTarget);
            }
        }

        foreach (var target in allTargets)
        {
            Debug.Log(target.addXp);
            if (target.addXp)
            {
                AddExperience();
            }
            else
            {
                return;
            }
        }


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
        Debug.Log("Add experience.");
    }
}
