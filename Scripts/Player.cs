using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent (typeof(PlayerController))]
[RequireComponent (typeof(GunController))]
public class Player : LivingEntity
{
    public float moveSpeed;

    //��ȡ���
    Camera viewCamera;
    PlayerController controller;
    GunController gunController;

    protected override void Start()
    {
        //��ȡ���
        gunController = GetComponent<GunController>();
        controller = GetComponent<PlayerController>();
        viewCamera = Camera.main;
    }

    void Update()
    {
        //��ȡ��ת����
        Vector3 moveInput = new Vector3(Input.GetAxisRaw("Horizontal"), 0, Input.GetAxisRaw("Vertical"));

        //�����ɫ�ٶ�
        Vector3 moveVelocity = moveInput.normalized * moveSpeed;
        controller.Move(moveVelocity);

        //��Camera����һ�������������ߣ�
        Ray ray = viewCamera.ScreenPointToRay(Input.mousePosition);

        //�������м�⣻
        Plane groundPlane = new Plane(Vector3.up, Vector3.zero);
        float rayDistance;
        if(groundPlane.Raycast(ray, out rayDistance))
        {
            Vector3 point = ray.GetPoint(rayDistance);

            //Debug.DrawLine(ray.origin, point, Color.red);

            //������߼���⵽λ�ã�������������Ǹ�����
            controller.LookAt(point);
        }

        //����������
        if (Input.GetMouseButton(0))
        {
            gunController.Shoot();
        }
    }
}
