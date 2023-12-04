using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(PlayerController))]
[RequireComponent(typeof(GunController))]
public class Player : LivingEntity
{
    public float moveSpeed;

    public Crosshairs crosshairs;

    //获取组件
    Camera viewCamera;
    PlayerController controller;
    GunController gunController;

    private void Awake()
    {
        //获取组件
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
        //获取旋转方向
        Vector3 moveInput = new Vector3(Input.GetAxisRaw("Horizontal"), 0, Input.GetAxisRaw("Vertical"));

        //给予角色速度
        Vector3 moveVelocity = moveInput.normalized * moveSpeed;
        controller.Move(moveVelocity);

        //从Camera发射一道经过鼠标的射线；
        Ray ray = viewCamera.ScreenPointToRay(Input.mousePosition);

        //与地面进行检测；
        Plane groundPlane = new Plane(Vector3.up, Vector3.up * gunController.GunHeight);
        float rayDistance;
        if(groundPlane.Raycast(ray, out rayDistance))
        {
            Vector3 point = ray.GetPoint(rayDistance);

            //Debug.DrawLine(ray.origin, point, Color.red);

            //如果射线检测检测到位置，就让玩家面向那个方向。
            controller.LookAt(point);
            crosshairs.transform.position = point;
            crosshairs.DetectedTarget(ray);

            if((new Vector2(point.x, point.z) - new Vector2(transform.position.x, transform.position.z)).sqrMagnitude > 2.5)
            {
                gunController.Aim(point);
            }
        }

        //武器输入检测
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
