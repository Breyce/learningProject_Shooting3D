using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(PlayerController))]
[RequireComponent(typeof(GunController))]
public class Player : LivingEntity
{
    public float moveSpeed;

    public Crosshairs crosshairs;

    //��ȡ���
    Camera viewCamera;
    PlayerController controller;
    GunController gunController;

    private void Awake()
    {
        //��ȡ���
        gunController = GetComponent<GunController>();
        controller = GetComponent<PlayerController>();
        viewCamera = Camera.main;
        FindObjectOfType<Spawner>().OnNewWave += OnNewWave;
    }

    protected override void Start()
    {
        base.Start();
    }

    public void OnNewWave(int waveNumber)
    {
        health = startingHealth;
        gunController.EquipGun(waveNumber - 1);
    }

    void Update()
    {
        if (transform.position.y < -10)
        {
            TakeDamage(health);
        }
        //��ȡ��ת����
        Vector3 moveInput = new Vector3(Input.GetAxisRaw("Horizontal"), 0, Input.GetAxisRaw("Vertical"));

        //�����ɫ�ٶ�
        Vector3 moveVelocity = moveInput.normalized * moveSpeed;
        controller.Move(moveVelocity);

        //��Camera����һ�������������ߣ�
        Ray ray = viewCamera.ScreenPointToRay(Input.mousePosition);

        //�������м�⣻
        Plane groundPlane = new Plane(Vector3.up, Vector3.up * gunController.GunHeight);
        float rayDistance;
        if(groundPlane.Raycast(ray, out rayDistance))
        {
            Vector3 point = ray.GetPoint(rayDistance);

            //Debug.DrawLine(ray.origin, point, Color.red);

            //������߼���⵽λ�ã�������������Ǹ�����
            controller.LookAt(point);
            crosshairs.transform.position = point;
            crosshairs.DetectedTarget(ray);

            if((new Vector2(point.x, point.z) - new Vector2(transform.position.x, transform.position.z)).sqrMagnitude > 2.5)
            {
                gunController.Aim(point);
            }
        }

        //����������
        if (Input.GetMouseButton(0))
        {
            gunController.OnTriggerHold();
        }
        if (Input.GetMouseButtonUp(0))
        {
            gunController.OnTriggerRelease();
        }
        if(Input.GetKeyDown(KeyCode.R)) 
        {
            gunController.Reload();
        }
    }

    public override void Die()
    {
        base.Die();
        AudioManager.instance.PlaySound("PlayerDeath", transform.position);
    }
}
