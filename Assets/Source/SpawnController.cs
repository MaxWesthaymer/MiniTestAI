using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnController : MonoBehaviour
{
    [SerializeField]private int enemyCount;
    [SerializeField]private float timeBetweenSpawn;
    [SerializeField]private Enemy enemy;
    private int enemiesRemainingToSpawn;
    private float nextSpawnTime;
    private int currentWaveNumber;
    private EntityConfig[] enemyConfigs;
    private List<Vector3> spawnPoints;

    private void Awake()
    {
        enemyConfigs = GameController.instance.entitySettings.enemyConfigs;
    }

    private void Start()
    {
        enemiesRemainingToSpawn = enemyCount;
        spawnPoints = new List<Vector3>();

        foreach (Transform it in transform)
        {
            spawnPoints.Add(it.position);
        }
    }
    private void Update()
    {
        if (enemiesRemainingToSpawn > 0 && Time.time > nextSpawnTime)
        {
            enemiesRemainingToSpawn--;
            nextSpawnTime = Time.time + timeBetweenSpawn;

            var spawnedEnemy = Instantiate(enemy, GetRandomPosition(), Quaternion.identity);

            if (enemyConfigs.Length > 0)
            {
                spawnedEnemy.SetupEntity(enemyConfigs[UnityEngine.Random.Range(0, enemyConfigs.Length)]);
            }
        }
    }
    private Vector3 GetRandomPosition()
    {
        var pos = spawnPoints.Count > 0 ? 
            spawnPoints[UnityEngine.Random.Range(0, spawnPoints.Count)] : Vector3.zero;
        return pos;
    }
}
