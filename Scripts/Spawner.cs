using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour
{
    public Wave[] waves;
    public Enemy enemy;

    Wave currentWave; //当前的这波敌人的信息：包括敌人总数和生成时间间隔。
    int currentWaveNumber; //当前是第几波

    int enemyRemainToSpawn; //当前这一波的敌人还剩多少个；
    int enemyRemainAlive; //当前这波敌人还有多少活着
    float nextSpawnTime; //生成下一个敌人的间隔时间

    private void Start()
    {
        NextWave();        
    }

    private void Update()
    {
        if(enemyRemainToSpawn > 0 && Time.time > nextSpawnTime)
        {
            enemyRemainToSpawn--;
            nextSpawnTime = Time.time + currentWave.timeBetweenSpawns;

            Enemy spawnEnemy = Instantiate(enemy, Vector3.zero, Quaternion.identity) as Enemy;
            spawnEnemy.OnDeath += OnEnemyDeath;
        }
    }

    void OnEnemyDeath()
    {
        enemyRemainAlive --;

        if(enemyRemainAlive <= 0)
        {
            NextWave();
        }
    }

    void NextWave()
    {
        currentWaveNumber++;

        print("Wave Number: " + currentWaveNumber);

        if(currentWaveNumber - 1 < waves.Length)
        {
            currentWave = waves[currentWaveNumber-1];

            enemyRemainToSpawn = currentWave.enemyCount;
            enemyRemainAlive = enemyRemainToSpawn;
        }
    }

    [System.Serializable]
    public class Wave
    {
        public int enemyCount; //敌人总数
        public float timeBetweenSpawns; //每个敌人的时间间隔
    }
}
