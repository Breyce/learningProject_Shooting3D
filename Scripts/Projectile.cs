using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    // Projectile������
    float speed = 10f;
    float damage = 1f;
    float lifetime = 3f;
    float skinWidth = .1f;

    //��ȡ���
    public LayerMask collisionMask;

    private void Start()
    {
        Destroy(gameObject, lifetime); //�������ڽ�����������Ϸʵ��

        /*
            ��������⣺�ӵ���Ϊ��ģ����Ϸʵ���ڲ��������ʱ�����޷���⵽����ô���˷�һ���ӵ���
            ������������ӵ�����ʱ�����뾶Ϊ0.1f��һ�����巶Χ���Ƿ���Enemy�����ص����У���˵���ӵ��򵽵����ˡ�
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

    //���ڼ�⵱ǰ��ײ���
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

        //�����ײ���������Ƿ���ʵ��Idamageable�ӿ�
        IDamageable damageableObject = c.GetComponent<IDamageable>();
        if (damageableObject != null) //�����ʵ��
        {
            damageableObject.TakeHit(damage, hitPoint, transform.forward);
        }

        GameObject.Destroy(gameObject);
    }
}
