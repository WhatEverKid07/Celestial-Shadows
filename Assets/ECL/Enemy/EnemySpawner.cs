using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class Enemies
{
    [SerializeField] private GameObject enemy;
    [SerializeField] [Range(0, 100)] private int percentChance;

    public GameObject Enemy => enemy;
    public int PercentChance => percentChance;
}
public class EnemySpawner : MonoBehaviour
{
    public EnemyAI enemyAIScript;

    [SerializeField] private Enemies[] enemies;
    private Dictionary<string, Enemies> enemyDictionary;

    [SerializeField] private AudioSource spawn;

    private void Awake()
    {
        enemyDictionary = new Dictionary<string, Enemies>();

        for (int i = 0; i < enemies.Length; i++)
        {
            string key = $"Enemy{i + 1}"; // "Enemy1", "Enemy2", etc.
            enemyDictionary[key] = enemies[i];
        }

        foreach (var entry in enemyDictionary)
        {
            Debug.Log($"{entry.Key}: {entry.Value.Enemy.name}, {entry.Value.PercentChance}%");
        }
        Summon(0);
    }

    private void Start()
    {

    }

    public void Summon(float extraSpeedLink)
    {
        Enemies randomEnemy = GetRandomEnemy();
        spawn.Play();
        Debug.Log("Randomly selected enemy: " + randomEnemy.Enemy.name + " | Chance: " + randomEnemy.PercentChance + "%");
        SummonEnemy(randomEnemy.Enemy, extraSpeedLink);

    }

    private Enemies GetRandomEnemy()
    {
        int randomValue = UnityEngine.Random.Range(0, 100);
        int cumulative = 0;

        foreach (var enemy in enemies)
        {
            cumulative += enemy.PercentChance;
            if (randomValue < cumulative)
            {
                return enemy;
            }
        }
        return enemies[enemies.Length - 1];
    }

    private void SummonEnemy(GameObject enemy, float extraSpeed)
    {
        GameObject enemyPrefab = Instantiate(enemy, gameObject.transform.position, gameObject.transform.rotation);
        enemyAIScript = enemyPrefab.gameObject.GetComponent<EnemyAI>();
        enemyAIScript.speed = 4 + extraSpeed;
    }
}   