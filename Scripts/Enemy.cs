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
    public ParticleSystem deathEffect;
    public static event System.Action OnDeathStatic;

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

    private void Awake()
    {
        //获取组件
        pathFinder = GetComponent<NavMeshAgent>();

        if (GameObject.FindGameObjectWithTag("Player") != null)
        {
            target = GameObject.FindGameObjectWithTag("Player").transform;
            targetEntity = target.GetComponent<LivingEntity>();

            //参数赋默认值
            hasTarget = true;
            myCollisionRadius = GetComponent<CapsuleCollider>().radius;
            targetCollisionRadius = target.GetComponent<CapsuleCollider>().radius;
        }
    }
    protected override void Start()
    {
        base.Start();

        if (hasTarget)
        {
            targetEntity.OnDeath += OnTargetDeath;
            currState = State.Chasing;
            
            //开启携程，避免频繁调用更新路径的函数降低游戏效率
            StartCoroutine(UpdatePath());
        }
    }

    public override void TakeHit(float damage, Vector3 hitPoint, Vector3 hitDirection)
    {
        AudioManager.instance.PlaySound("Impact",transform.position);
        if(damage >= health)
        {
            if(OnDeathStatic != null)
            {
                OnDeathStatic();
            }
            AudioManager.instance.PlaySound("EnemyDeath", transform.position);
            Destroy(Instantiate(deathEffect.gameObject, hitPoint, Quaternion.FromToRotation(Vector3.forward, hitDirection)) as GameObject, deathEffect.startLifetime);
        }
        base.TakeHit(damage, hitPoint, hitDirection);
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
                    AudioManager.instance.PlaySound("EnemyAttack", transform.position);

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

    [System.Obsolete]
    public void SetCharacteristic(float movSpeed,int hitTokillPlayer,float enemyHealth, Color skinColor)
    {
        pathFinder.speed = movSpeed;

        if (hasTarget)
        {
            damage = Mathf.Ceil(targetEntity.startingHealth / hitTokillPlayer);
        }

        deathEffect.startColor = new Color(skinColor.r, skinColor.g, skinColor.b, 1);
        startingHealth = enemyHealth;
        skinMaterial = GetComponent<Renderer>().material;
        skinMaterial.color = skinColor;
        originColor = skinMaterial.color;
    }
}
