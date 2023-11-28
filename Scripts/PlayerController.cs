using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent (typeof(Rigidbody))]
public class PlayerController : MonoBehaviour
{
    Vector3 velocity;
    Rigidbody myRigidbody;

    void Start()
    {
        //获取组件
        myRigidbody = GetComponent<Rigidbody>();
    }

    // 玩家移动函数，传入的是移动坐标
    public void Move(Vector3 moveVelocity)
    {
        velocity = moveVelocity;
    }

    
    //控制玩家面向方向
    public void LookAt(Vector3 lookPoint)
    {
        Vector3 correctLookAtPoint = new Vector3(lookPoint.x,transform.position.y,lookPoint.z);
        transform.LookAt(correctLookAtPoint);
    }

    void FixedUpdate()
    {
        myRigidbody.MovePosition(myRigidbody.position + velocity * Time.fixedDeltaTime);
    }
}
