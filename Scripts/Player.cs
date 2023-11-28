using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent (typeof(PlayerController))]
[RequireComponent (typeof(GunController))]
public class Player : LivingEntity
{
    public float moveSpeed;

    //获取组件
    Camera viewCamera;
    PlayerController controller;
    GunController gunController;

    protected override void Start()
    {
        //获取组件
        gunController = GetComponent<GunController>();
        controller = GetComponent<PlayerController>();
        viewCamera = Camera.main;
    }

    void Update()
    {
        //获取旋转方向
        Vector3 moveInput = new Vector3(Input.GetAxisRaw("Horizontal"), 0, Input.GetAxisRaw("Vertical"));

        //给予角色速度
        Vector3 moveVelocity = moveInput.normalized * moveSpeed;
        controller.Move(moveVelocity);

        //从Camera发射一道经过鼠标的射线；
        Ray ray = viewCamera.ScreenPointToRay(Input.mousePosition);

        //与地面进行检测；
        Plane groundPlane = new Plane(Vector3.up, Vector3.zero);
        float rayDistance;
        if(groundPlane.Raycast(ray, out rayDistance))
        {
            Vector3 point = ray.GetPoint(rayDistance);

            //Debug.DrawLine(ray.origin, point, Color.red);

            //如果射线检测检测到位置，就让玩家面向那个方向。
            controller.LookAt(point);
        }

        //武器输入检测
        if (Input.GetMouseButton(0))
        {
            gunController.Shoot();
        }
    }
}
