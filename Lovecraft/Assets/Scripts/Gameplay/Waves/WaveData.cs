using UnityEngine;
using System.Collections.Generic;

[System.Serializable]
public class WaveData
{
    public List<EnemySpawnData> enemiesToSpawn;
    public float timeBetweenWaves;
    public int reinforcementThreshold;
}

[System.Serializable]
public class EnemySpawnData
{
    public GameObject enemyPrefab;
    public int count;
    public SpawnRegion spawnRegion;
}

[System.Serializable]
public class SpawnRegion
{
    public Transform regionTransform;
    [HideInInspector]
    public List<Transform> spawnPoints = new List<Transform>();

    public void Initialize()
    {
        if (regionTransform != null)
        {
            spawnPoints.Clear();
            foreach (Transform child in regionTransform)
            {
                spawnPoints.Add(child);
            }
        }
    }
}