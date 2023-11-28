using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour
{
    public Wave[] waves;
    public Enemy enemy;

    Wave currentWave; //��ǰ���Ⲩ���˵���Ϣ��������������������ʱ������
    int currentWaveNumber; //��ǰ�ǵڼ���

    int enemyRemainToSpawn; //��ǰ��һ���ĵ��˻�ʣ���ٸ���
    int enemyRemainAlive; //��ǰ�Ⲩ���˻��ж��ٻ���
    float nextSpawnTime; //������һ�����˵ļ��ʱ��

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
        public int enemyCount; //��������
        public float timeBetweenSpawns; //ÿ�����˵�ʱ����
    }
}
