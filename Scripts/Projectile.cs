using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    // Projectile的属性
    float speed = 10f;
    float damage = 1f;
    float lifetime = 3f;
    float skinWidth = .1f;

    //获取组件
    public LayerMask collisionMask;

    private void Start()
    {
        Destroy(gameObject, lifetime); //生命周期结束就销毁游戏实体

        /*
            解决的问题：子弹因为穿模在游戏实体内部打出，此时射线无法检测到，那么会浪费一颗子弹。
            解决方法：在子弹生成时，检测半径为0.1f的一个球体范围内是否与Enemy层有重叠，有，则说明子弹打到敌人了。
         */
        Collider[] initialCollision = Physics.OverlapSphere(transform.position, .1f, collisionMask);
        if(initialCollision.Length > 0)
        {
            OnHitObject(initialCollision[0], transform.position);
        }
    }

    public void SetSpeed(float newSpeed)
    {
        speed = newSpeed;
    }

    void Update()
    {
        float moveDistance = speed * Time.deltaTime;
        CheckCollision(moveDistance);
        transform.Translate(Vector3.forward * moveDistance);
    }

    //用于检测当前碰撞情况
    void CheckCollision(float moveDistance)
    {
        Ray ray = new Ray(transform.position, transform.forward);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, moveDistance + skinWidth, collisionMask, QueryTriggerInteraction.Collide))
        {
            OnHitObject(hit.collider,hit.point);
        }
    }

    void OnHitObject(Collider c, Vector3 hitPoint)
    {
        //print(hit.collider.gameObject.name);

        //检查碰撞物体身上是否有实现Idamageable接口
        IDamageable damageableObject = c.GetComponent<IDamageable>();
        if (damageableObject != null) //如果有实现
        {
            damageableObject.TakeHit(damage, hitPoint, transform.forward);
        }

        GameObject.Destroy(gameObject);
    }
}
