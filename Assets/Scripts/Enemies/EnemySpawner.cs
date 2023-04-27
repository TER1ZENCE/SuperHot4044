using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [SerializeField]
    private List<Transform> spawnPositions;
    [SerializeField] 
    private List<Transform> unusedSpawnPositions;

    public Transform EnemyManager;
    [SerializeField]
    private GameObject enemy;
    [SerializeField]
    private int enemyCount;
    [SerializeField]
    public int maxEnemyCount;

    private void Start()
    {
        spawnPositions = new List<Transform>(maxEnemyCount);
        unusedSpawnPositions = new List<Transform>(spawnPositions);
        GetSpawners();
        StartCoroutine(SpawnEnemies());
    }
    public void GetSpawners()
    {
        foreach (Transform child in transform)
        {
            unusedSpawnPositions.Add(child);
        }
    }

    private IEnumerator SpawnEnemies()
    {
        while (enemyCount < maxEnemyCount)
        {
            int spawnerID = UnityEngine.Random.Range(0, unusedSpawnPositions.Count);
            Transform spawnPosition = unusedSpawnPositions[spawnerID];
            unusedSpawnPositions.RemoveAt(spawnerID);

            GameObject newEnemy = Instantiate(enemy, spawnPosition.position, spawnPosition.rotation);
            newEnemy.transform.parent = EnemyManager.gameObject.transform;
            enemyCount++;
        }
        yield return null;
    }

}
