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

        //��ȡ���
        pathFinder = GetComponent<NavMeshAgent>();
        target = GameObject.FindGameObjectWithTag("Player").transform;

        //����Я�̣�����Ƶ�����ø���·���ĺ���������ϷЧ��
        StartCoroutine(UpdatePath());
    }

    void Update()
    {

    }

    // Ϊ�������̫��Ƶ�����ã��������׷���趨���ж�ʱ���¡�
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
