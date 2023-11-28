using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class Enemy : LivingEntity
{
    NavMeshAgent pathFinder;
    Transform target;

    protected override void Start()
    {
        base.Start();

        //获取组件
        pathFinder = GetComponent<NavMeshAgent>();
        target = GameObject.FindGameObjectWithTag("Player").transform;

        //开启携程，避免频繁调用更新路径的函数降低游戏效率
        StartCoroutine(UpdatePath());
    }

    void Update()
    {

    }

    // 为避免敌人太多频繁调用，在这里对追踪设定进行定时更新。
    IEnumerator UpdatePath()
    {
        float refreshRate = 0.25f;
        while (target != null && !dead)
        {
            Vector3 targetPosition = new Vector3(target.position.x, 0, target.position.z);
            pathFinder.SetDestination(targetPosition);
            yield return new WaitForSeconds(refreshRate);
        }
    }
}
