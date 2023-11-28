using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    float speed = 10f;
    float damage = 1f;

    //��ȡ���
    public LayerMask collisionMask;

    public void SetSpeed(float newSpeed)
    {
        speed = newSpeed;
    }

    void Update()
    {
        float moveDistance = speed * Time.deltaTime;
        CheeckCollision(moveDistance);
        transform.Translate(Vector3.forward * moveDistance);
    }

    void CheeckCollision(float moveDistance)
    {
        Ray ray = new Ray(transform.position, transform.forward);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, moveDistance, collisionMask, QueryTriggerInteraction.Collide))
        {
            OnHitObject(hit);
        }
    }

    void OnHitObject(RaycastHit hit)
    {
        print(hit.collider.gameObject.name);

        //�����ײ���������Ƿ���ʵ��Idamageable�ӿ�
        IDamageable damageableObject = hit.collider.GetComponent<IDamageable>();
        if (damageableObject != null) //�����ʵ��
        {
            damageableObject.TakeHit(damage, hit);
        }

        GameObject.Destroy(gameObject);
    }
}
