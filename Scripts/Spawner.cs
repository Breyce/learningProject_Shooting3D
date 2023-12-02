using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour
{
    public Wave[] waves;
    public Enemy enemy;

    LivingEntity playerEntity;
    Transform playerT;

    Wave currentWave; //当前的这波敌人的信息：包括敌人总数和生成时间间隔。
    int currentWaveNumber; //当前是第几波

    int enemyRemainToSpawn; //当前这一波的敌人还剩多少个；
    int enemyRemainAlive; //当前这波敌人还有多少活着
    float nextSpawnTime; //生成下一个敌人的间隔时间

    MapGenerator map;

    float timeBetweenCampingChecks = 2;
    float campThresholdDistance = 1.5f;
    float nextCampCheckTime;
    Vector3 campPositionOld;
    bool isCamping;

    bool isDisabled;

    public event System.Action<int> OnNewWave;

    private void Start()
    {
        playerEntity = FindObjectOfType<Player>();
        playerT = playerEntity.transform;

        nextCampCheckTime = timeBetweenCampingChecks + Time.time;
        campPositionOld = playerT.position;
        playerEntity.OnDeath += OnPlayerDeath;

        map = FindObjectOfType<MapGenerator>();
        NextWave();        
    }

    private void Update()
    {
        if (!isDisabled)
        {
            if(Time.time > nextCampCheckTime)
            {
                nextCampCheckTime = timeBetweenCampingChecks + Time.time;

                isCamping = (Vector3.Distance(playerT.position, campPositionOld) < campThresholdDistance);

                campPositionOld = playerT.position;
            }
            if(enemyRemainToSpawn > 0 && Time.time > nextSpawnTime)
            {
                enemyRemainToSpawn--;
                nextSpawnTime = Time.time + currentWave.timeBetweenSpawns;

                StartCoroutine(SpawnEnemy());
            }
        }
    }
    IEnumerator SpawnEnemy()
    {
        float spawnDelay = 1;
        float timeFlashSpeed = 4;

        Transform randomTile = map.GetRandomOpenTile();
        if (isCamping)
        {
            randomTile = map.GetTileFromPosition(playerT.position);
        }
        Material tileMat = randomTile.GetComponent<Renderer>().material;

        Color initialColor = tileMat.color;
        Color flashColor = Color.red;
        float spawnTimer = 0;
        
        while(spawnTimer < spawnDelay)
        {
            tileMat.color = Color.Lerp(initialColor ,flashColor, Mathf.PingPong(spawnTimer * timeFlashSpeed, 1));
            Debug.Log("颜色变化了" + tileMat.color);

            spawnTimer += Time.deltaTime;
            yield return null;
        }

        Enemy spawnEnemy = Instantiate(enemy, randomTile.position + Vector3.up, Quaternion.identity) as Enemy;
        spawnEnemy.OnDeath += OnEnemyDeath;
    }

    void OnPlayerDeath()
    {
        isDisabled = true;
    }

    void ResetPlayerPosition()
    {
        playerT.position = map.GetTileFromPosition(Vector3.zero).position + Vector3.up * 3;
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

        //print("Wave Number: " + currentWaveNumber);

        if(currentWaveNumber - 1 < waves.Length)
        {
            currentWave = waves[currentWaveNumber-1];

            enemyRemainToSpawn = currentWave.enemyCount;
            enemyRemainAlive = enemyRemainToSpawn;

            if(OnNewWave != null)
            {
                OnNewWave(currentWaveNumber);
            }

            ResetPlayerPosition();
        }
    }

    [System.Serializable]
    public class Wave
    {
        public int enemyCount; //敌人总数
        public float timeBetweenSpawns; //每个敌人的时间间隔
    }
}
