using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[Serializable]
public class Items
{
    [SerializeField] private GameObject itemDrop;
    [SerializeField][Range(0, 100)] private int percentChance;

    public GameObject ItemDrops => itemDrop;
    public int PercentChance => percentChance;
}
public class PlayerExperience : MonoBehaviour
{
    [Header("ItemDrops")]
    [SerializeField] private Items[] items;
    private Dictionary<string, Items> itemDictionary;

    [Header("Enemies")]
    [SerializeField] private List<GameObject> enemies;
    [SerializeField] private List<Target> allTargets;

    [Header("XP")]
    private float maxXp;
    public int xpLvl { get; private set; }
    public float currentXp { get; private set; }

    private void Awake()
    {
        itemDictionary = new Dictionary<string, Items>();

        for (int i = 0; i < items.Length; i++)
        {
            string key = $"Item{i + 1}"; 
            itemDictionary[key] = items[i];
        }

        foreach (var entry in itemDictionary)
        {
            Debug.Log($"{entry.Key}: {entry.Value.ItemDrops.name}, {entry.Value.PercentChance}%");
        }
    }

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

            Items randomItem = GetRandomItem();
            SummonItem(randomItem.ItemDrops);
        }
    }

    private void AddExperience()
    {
        currentXp = currentXp + 25;
    }

    private Items GetRandomItem()
    {
        int randomValue = UnityEngine.Random.Range(0, 100);
        int cumulative = 0;

        foreach (var item in items)
        {
            cumulative += item.PercentChance;
            if (randomValue < cumulative)
            {
                return item;
            }
        }
        return items[items.Length - 1];
    }
    private void SummonItem(GameObject item)
    {
        Instantiate(item, gameObject.transform.position, gameObject.transform.rotation);
    }
}
