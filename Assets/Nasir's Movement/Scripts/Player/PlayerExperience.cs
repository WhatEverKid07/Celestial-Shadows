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
    public int xpLvl { get; private set; }
    public float currentXp { get; private set; }

    private void Start()
    {
        xpLvl = 0;
        currentXp = 0;
        maxXp = 100;
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
                target.addXp = false;
                allTargets.Remove(target);
            }
        }

        if (currentXp >= maxXp)
        {
            xpLvl++;
            currentXp = 0;
            maxXp += 100;
        }
    }

    private void AddExperience()
    {
        currentXp = currentXp + 25;
    }
}
