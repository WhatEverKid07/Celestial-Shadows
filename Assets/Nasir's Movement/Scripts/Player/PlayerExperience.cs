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
    [Header("Scripts")]
    [SerializeField] private PlayerStats playerStatScript;
    [SerializeField] private NewARScript arScript;
    [SerializeField] private NewPistolScript pistolScript;
    [SerializeField] private NewShotgunScript shotgunScript;

    [Header("Weapons")]
    private int arAmmo;
    private int pistolAmmo;
    private int shotgunAmmo;

    internal bool increaseArDmg;
    internal bool increasePistolDmg;
    internal bool increaseShotDmg;

    [Header("ItemDrops")]
    [SerializeField] private Items[] items;
    private Dictionary<string, Items> itemDictionary;
    private GameObject itemDropPrefab;

    [Header("Enemies")]
    [SerializeField] private List<GameObject> enemies;
    [SerializeField] private List<Target> allTargets;

    [Header("XP")]
    private float maxXp;
    public int xpLvl { get; private set; }
    public float currentXp { get; private set; }

    private void Awake()
    {
        Debug.Log($"Items array length: {items.Length}");
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

        arAmmo = arScript.maxAmmo;
        pistolAmmo = pistolScript.maxAmmo;
        shotgunAmmo = shotgunScript.maxAmmo;
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
            if (target.addXp)
            {
                AddExperience();
                target.addXp = false;
                allTargets.Remove(target);
            }
        }

        Items randomItem = GetRandomItem();

        if (currentXp >= maxXp)
        {
            xpLvl++;
            currentXp = 0;
            maxXp += 50;

            //mag size
            //

            if (items != null)
            {
                SummonItem(randomItem.ItemDrops);
            }

            if (arScript.isActiveAndEnabled)
            {
                int ammo = arAmmo + 5;
                arAmmo = ammo;
                arScript.maxAmmo = arAmmo;

                increaseArDmg = true;
                increasePistolDmg = false;
                increaseShotDmg = false;

                Invoke(nameof(UpdateBool), .001f);
            }
            else if (pistolScript.isActiveAndEnabled)
            {
                int ammo = pistolAmmo + 5;
                pistolScript.maxAmmo = ammo;

                increasePistolDmg = true;
                increaseArDmg = false;
                increaseShotDmg = false;
            }
            else if (shotgunScript.isActiveAndEnabled)
            {
                int ammo = shotgunAmmo + 2;
                shotgunScript.maxAmmo = ammo;

                increaseShotDmg = true;
                increaseArDmg = false;
                increasePistolDmg = false;
            }
            else
            {
                Debug.Log("Not a weapon.");
            }
        }
    }

    private void UpdateBool()
    {
        increaseArDmg = false;
        increasePistolDmg = false;
        increaseShotDmg = false;
    }

    private void AddExperience()
    {
        currentXp = currentXp + 25;
    }

    private Items GetRandomItem()
    {
        if (items == null || items.Length == 0)
        {
            Debug.LogWarning("No items available to select from.");
            return null;
        }

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
        return items[^1];
    }
    private void SummonItem(GameObject item)
    {
        Vector3 spawn = new Vector3(gameObject.transform.position.x + 1f, gameObject.transform.position.y - .5f, gameObject.transform.position.z + 1f);
        itemDropPrefab = Instantiate(item, spawn, item.transform.rotation);
    }
}
