using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class WaveManager : MonoBehaviour
{
    public List<WaveData> waves;
    public float checkInterval = 2f;
    private int currentWaveIndex = 0;
    private int enemiesAlive = 0;
    private Transform treeTarget;
    private Transform playerTarget;
    private bool waveInProgress = false;

    void Start()
    {
        treeTarget = GameObject.FindObjectOfType<TreeController>().transform;
        playerTarget = GameObject.FindObjectOfType<PlayerController>().transform;
        GameEventSystem.Instance.RegisterListener(GameEvent.ENEMY_KILLED, OnEnemyKilled);
    }

    public void StartWave()
    {
        if (currentWaveIndex < waves.Count)
        {
            StartCoroutine(ManageWave(waves[currentWaveIndex]));
        }
    }

    IEnumerator ManageWave(WaveData wave)
    {
        waveInProgress = true;
        foreach (var stage in wave.stages)
        {
            yield return StartCoroutine(SpawnWaveStage(stage));
            yield return new WaitForSeconds(stage.timeBetweenStages);
        }
        waveInProgress = false;
        StartCoroutine(CheckWaveCompletion());
    }

    IEnumerator SpawnWaveStage(WaveStage stage)
    {
        foreach (var enemyData in stage.enemiesToSpawn)
        {
            for (int i = 0; i < enemyData.count; i++)
            {
                Transform spawnPoint = GetRandomSpawnPoint(enemyData.spawnRegion);
                var enemy = Instantiate(enemyData.enemyPrefab, spawnPoint.position, Quaternion.identity).GetComponent<EnemyAI>();
                enemy.treeTarget = treeTarget;
                enemy.playerTarget = playerTarget;
                enemiesAlive++;
                yield return new WaitForSeconds(0.1f);
            }
        }

        StartCoroutine(CheckForReinforcements(stage));
    }

    IEnumerator CheckForReinforcements(WaveStage stage)
    {
        while (waveInProgress)
        {
            if (enemiesAlive <= stage.reinforcementThreshold)
            {
                foreach (var enemyData in stage.enemiesToSpawn)
                {
                    for (int i = 0; i < enemyData.count; i++)
                    {
                        Transform spawnPoint = GetRandomSpawnPoint(enemyData.spawnRegion);
                        var enemy = Instantiate(enemyData.enemyPrefab, spawnPoint.position, Quaternion.identity).GetComponent<EnemyAI>();
                        enemy.treeTarget = treeTarget;
                        enemy.playerTarget = playerTarget;
                        enemiesAlive++;
                        yield return new WaitForSeconds(0.1f);
                    }
                }
            }
            yield return new WaitForSeconds(checkInterval);
        }
    }

    IEnumerator CheckWaveCompletion()
    {
        while (true)
        {
            if (enemiesAlive <= 0 && !waveInProgress)
            {
                GameEventSystem.Instance.TriggerEvent(GameEvent.WAVE_COMPLETED, null);
                yield break;
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
