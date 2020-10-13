using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnController : MonoBehaviour
{
    [SerializeField]private int enemyCount;
    [SerializeField]private float timeBetweenSpawn;
    [SerializeField]private LivingEntity enemy;
    [SerializeField]private LivingEntity gunEnemy;
    private int _enemiesRemainingToSpawn;
    private float _nextSpawnTime;
    private int _currentWaveNumber;
    private EntityConfig[] _enemyConfigs;
    private List<Vector3> _spawnPoints;

    private void Awake()
    {
        _enemyConfigs = GameController.instance.entitySettings.enemyConfigs;
    }

    private void Start()
    {
        _enemiesRemainingToSpawn = enemyCount;
        _spawnPoints = new List<Vector3>();

        foreach (Transform it in transform)
        {
            _spawnPoints.Add(it.position);
        }
        GameController.OnEndGame += (x) => { _enemiesRemainingToSpawn = 0;};
    }
    private void Update()
    {
        if (_enemiesRemainingToSpawn > 0 && Time.time > _nextSpawnTime)
        {
            _enemiesRemainingToSpawn--;
            _nextSpawnTime = Time.time + timeBetweenSpawn;

            var spawnedEnemy = Instantiate(GetEnemy(), GetRandomPosition(), Quaternion.identity);

            if (_enemyConfigs.Length > 0)
            {
                spawnedEnemy.SetupEntity(_enemyConfigs[Random.Range(0, _enemyConfigs.Length)]);
            }
        }
    }

    private LivingEntity GetEnemy()
    {
        var chance = Random.value;

        if (chance > 0.8f)
        {
            return gunEnemy;
        }
        else
        {
            return enemy;
        }
    }
    private Vector3 GetRandomPosition()
    {
        var pos = _spawnPoints.Count > 0 ? 
            _spawnPoints[Random.Range(0, _spawnPoints.Count)] : Vector3.zero;
        return pos;
    }
}
