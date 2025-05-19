using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class WaveManager : MonoBehaviour
{
    [SerializeField] private string enemyTag;
    [SerializeField] private TMP_Text waveText;
    [SerializeField] private TMP_Text enemyCounter;
    [SerializeField] private EnemySpawner[] enemySpawners;
    [SerializeField] private List<GameObject> enemies = new List<GameObject>();

    private bool endOfWave = false;
    private int waveCount = 1;

    private void Start()
    {
        waveText.text = "Wave: " + waveCount;
    }
    private void GetEnemies()
    {
        enemies.Clear();
        GameObject[] activeEnemies = GameObject.FindGameObjectsWithTag(enemyTag);
        enemies.AddRange(activeEnemies);
        // update enemy counter
        enemyCounter.text = "Enemies left: " + activeEnemies.Length;
        if (enemies.Count == 0)
        {
            endOfWave = true;
            AudioManager.instance.ToggleVolume();
            StartCoroutine(SummonNewWave());
        }
    }

    private void FixedUpdate()
    {
        if (!endOfWave)
        {
            GetEnemies();
        }
    }

    private IEnumerator SummonNewWave()
    {
        foreach (EnemySpawner script in enemySpawners)
        {
            yield return new WaitForSeconds(2);
            script.Summon(waveCount / 10f);
        }

        waveCount++;
        waveText.text = "Wave: " + waveCount;

        yield return new WaitForSeconds(2);
        GetEnemies();
        yield return new WaitForSeconds(2);
        AudioManager.instance.ToggleVolume();
        endOfWave = false;
    }
}