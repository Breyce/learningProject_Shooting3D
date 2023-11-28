using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class Enemy : LivingEntity
{
    public enum State
    {
        Idle,
        Chasing,
        Attack
    }
    State currState;

    NavMeshAgent pathFinder;
    Transform target;
    Material skinMaterial;
    LivingEntity targetEntity;
    Color originColor;

    float damage = 1f;
    float attackDistanceThreshold = .5f;
    float timeBetweenAttacks = 1f;
    float nextAttackTime;

    float myCollisionRadius;
    float targetCollisionRadius;

    bool hasTarget;

    protected override void Start()
    {
        base.Start();

        //获取组件
        pathFinder = GetComponent<NavMeshAgent>();
        skinMaterial = GetComponent<Renderer>().material;
        originColor = skinMaterial.color;

        if (GameObject.FindGameObjectWithTag("Player") != null)
        {
            target = GameObject.FindGameObjectWithTag("Player").transform;
            targetEntity = target.GetComponent<LivingEntity>();
            targetEntity.OnDeath += OnTargetDeath;

            //参数赋默认值
            hasTarget = true;
            currState = State.Chasing;
            myCollisionRadius = GetComponent<CapsuleCollider>().radius;
            targetCollisionRadius = target.GetComponent<CapsuleCollider>().radius;

            //开启携程，避免频繁调用更新路径的函数降低游戏效率
            StartCoroutine(UpdatePath());
        }
    }

    void Update()
    {
        if (hasTarget)
        {            
            if (Time.time > nextAttackTime && !dead)
            {
                float sqrDstToTarget = (target.position - transform.position).sqrMagnitude;

                if(sqrDstToTarget < Mathf.Pow(attackDistanceThreshold + myCollisionRadius + targetCollisionRadius, 2))
                {
                    nextAttackTime = Time.time + timeBetweenAttacks;
                    StartCoroutine(Attack());
                }
            }
        }

    }

    void OnTargetDeath()
    {
        hasTarget = false;
        currState = State.Idle;
    }

    //敌人攻击的携程
    IEnumerator Attack()
    {
        currState = State.Attack;// 改变当前敌人的移动状态
        pathFinder.enabled = false;// 停止敌人的

        Vector3 originPosition = transform.position;
        Vector3 dirToTarget = (target.position - transform.position).normalized;
        Vector3 attackPosition = target.position - dirToTarget * (myCollisionRadius);

        float percent = 0;
        float attackSpeed = 3;
        skinMaterial.color = Color.yellow;
        bool hasApplyTheDamage = false;

        while(percent <= 1)
        {
            if(percent > .5f && !hasApplyTheDamage)
            {
                hasApplyTheDamage = true;
                targetEntity.TakeDamage(damage);
            }

            //移动的差值，让它比较平缓
            percent += Time.deltaTime * attackSpeed;
            float interpolation = (-Mathf.Pow(percent,2) + percent) * 4;
            transform.position = Vector3.Lerp(originPosition, attackPosition, interpolation);

            yield return null;
        }

        skinMaterial.color = originColor;
        pathFinder.enabled = true;
        currState = State.Chasing;
    }

    // 为避免敌人太多频繁调用，在这里对追踪设定进行定时更新。
    IEnumerator UpdatePath()
    {
        float refreshRate = 0.25f;
        while (hasTarget)
        {
            if(currState == State.Chasing)
            {
                Vector3 dirToTarget = (target.position - transform.position).normalized;
                Vector3 targetPosition = target.position - dirToTarget * (myCollisionRadius + targetCollisionRadius + attackDistanceThreshold / 2);
                if (!dead)
                {
                    pathFinder.SetDestination(targetPosition);
                }
            }
            yield return new WaitForSeconds(refreshRate);
        }
    }
}
