using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScoreKeeper : MonoBehaviour
{
    public int score;
    float lastEnemyKillTime;
    int streakCount;
    float streakExpiryTime = 1;

    private void Start()
    {
        Enemy.OnDeathStatic += OnEnemyKilled;
        FindObjectOfType<Player>().OnDeath += OnPlayerDeath;
    }

    void OnEnemyKilled()
    {
        if(Time.time < lastEnemyKillTime + streakExpiryTime)
        {
            streakExpiryTime++;
        }
        else
        {
            streakCount = 0;
        }

        lastEnemyKillTime = Time.time;

        score += 5 + (int)Mathf.Pow(2,streakCount);
    }

    void OnPlayerDeath()
    {
        Enemy.OnDeathStatic -= OnEnemyKilled;
        score = 0;
    }

}
