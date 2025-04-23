using Fusion;
using UnityEngine;
using System.Collections;

public class enemySpawner : NetworkBehaviour
{
    [Networked] private TickTimer delay { get; set; }
    public GameObject enemyPrefab, elitePrefab;
    public float spawnRadius = 10f;
    public float spawnInterval = 5f;
    public int maxEnemies = 5;
    public Transform circleCentrePoint;
    public bool enemySpawnerActivator = false;
    

    public override void Spawned()
    {
        if (Object.HasStateAuthority)
        {
            delay = TickTimer.CreateFromSeconds(Runner, spawnInterval);
        }
    }

    public override void FixedUpdateNetwork()
    {
        if (Object.HasStateAuthority && delay.Expired(Runner) && enemySpawnerActivator)
        {
            delay = TickTimer.CreateFromSeconds(Runner, spawnInterval);
            spawnEnemy();
        }
    }

    void spawnEnemy()
    {
        int currentEnemyNum = EnemyCount();
        if (currentEnemyNum < maxEnemies)
        {
            Vector2 randomCirclePoint = Random.insideUnitCircle.normalized * spawnRadius;
            Vector3 spawnPos = circleCentrePoint.position + new Vector3(randomCirclePoint.x, 0, randomCirclePoint.y);

            Runner.Spawn(enemyPrefab, spawnPos, Quaternion.identity);
        }
    }

    int EnemyCount()
    {
        int count = 0;
        foreach (var networkObject in Runner.GetAllNetworkObjects())
        {
            if (networkObject.TryGetComponent<EnemyScript>(out _))
            {
                count++;
            }
        }
        return count;
    }

    public void summonElite()
    {
        int currentEnemyNum = 0;
        if (currentEnemyNum < 4)
        {
            Vector2 randomCirclePoint = Random.insideUnitCircle.normalized * spawnRadius;
            Vector3 spawnPos = circleCentrePoint.position + new Vector3(randomCirclePoint.x, 0, randomCirclePoint.y);

            Runner.Spawn(elitePrefab, spawnPos, Quaternion.identity);
        }
    }
}

