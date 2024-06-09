using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class WaveManager : MonoBehaviour
{
    public List<WaveData> waves;
    public float checkInterval = 2f; // Time interval to check enemy count
    private int currentWaveIndex = 0;
    private int enemiesAlive = 0;
    private Transform treeTarget;
    private Transform playerTarget;

    void Start()
    {
        treeTarget = GameObject.FindObjectOfType<TreeController>().transform;
        playerTarget = GameObject.FindObjectOfType<PlayerController>().transform;
        StartCoroutine(ManageWaves());
        GameEventSystem.Instance.RegisterListener(GameEvent.ENEMY_KILLED, OnEnemyKilled);
    }

    IEnumerator ManageWaves()
    {
        while (currentWaveIndex < waves.Count)
        {
            WaveData currentWave = waves[currentWaveIndex];
            StartCoroutine(SpawnWave(currentWave));
            yield return new WaitForSeconds(currentWave.timeBetweenWaves);
            currentWaveIndex++;
        }
    }

    IEnumerator SpawnWave(WaveData wave)
    {
        foreach (var enemyData in wave.enemiesToSpawn)
        {
            for (int i = 0; i < enemyData.count; i++)
            {
                Transform spawnPoint = GetRandomSpawnPoint(enemyData.spawnRegion);
                var enemy = Instantiate(enemyData.enemyPrefab, spawnPoint.position, Quaternion.identity).GetComponent<EnemyAI>();
                enemy.treeTarget = treeTarget;
                enemy.playerTarget = playerTarget;
                enemiesAlive++;
                yield return new WaitForSeconds(0.1f); // Slight delay between spawns
            }
        }

        StartCoroutine(CheckForReinforcements(wave));
    }

    IEnumerator CheckForReinforcements(WaveData wave)
    {
        while (true)
        {
            if (enemiesAlive <= wave.reinforcementThreshold)
            {
                foreach (var enemyData in wave.enemiesToSpawn)
                {
                    for (int i = 0; i < enemyData.count; i++)
                    {
                        Transform spawnPoint = GetRandomSpawnPoint(enemyData.spawnRegion);
                        var enemy = Instantiate(enemyData.enemyPrefab, spawnPoint.position, Quaternion.identity).GetComponent<EnemyAI>();
                        enemy.treeTarget = treeTarget;
                        enemy.playerTarget = playerTarget;
                        enemiesAlive++;
                        yield return new WaitForSeconds(0.1f); // Slight delay between spawns
                    }
                }
            }
            yield return new WaitForSeconds(checkInterval);
        }
    }

    Transform GetRandomSpawnPoint(SpawnRegion region)
    {
        if (region.spawnPoints == null || region.spawnPoints.Count <= 0)
        {
            region.Initialize();
            if (region.spawnPoints == null || region.spawnPoints.Count <= 0)
                return region.regionTransform;
        }

        int randomIndex = Random.Range(0, region.spawnPoints.Count);
        return region.spawnPoints[randomIndex];
    }

    public void OnEnemyKilled(object data)
    {
        enemiesAlive--;
    }
}