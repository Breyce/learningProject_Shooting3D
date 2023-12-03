using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gun : MonoBehaviour
{
    public enum FireMode
    {
        Auto, Burst, Single
    }
    public FireMode fireMode;
    
    public Transform[] projectSpawn;
    public Projectile projectile;
    public float msBetweenShoots = 1000;
    public float muzzleVelocity = 35;

    //feature for Burst fire
    public int burstCount;

    float nextShotTime;

    public Transform shell;
    public Transform shellEjectionPoint;
    public MuzzleFlash muzzleFlash;

    bool triggerReleaseSinceLastShoot;
    int shootRemainInBurst;

    private void Start()
    {
        muzzleFlash = GetComponent<MuzzleFlash>();
        shootRemainInBurst = burstCount;
    }

    void Shoot()
    {
        if(Time.time > nextShotTime)//Éä»÷¼ä¸ô
        {
            if(fireMode == FireMode.Burst)
            {
                if(shootRemainInBurst == 0)
                {
                    return;
                }
                shootRemainInBurst--;
            }else if(fireMode == FireMode.Single)
            {
                if (!triggerReleaseSinceLastShoot)
                {
                    return;
                }
            }
            nextShotTime = Time.time + msBetweenShoots / 1000;

            for(int i = 0;i<projectSpawn.Length;i++)
            {
                Projectile newProjectile = Instantiate(projectile, projectSpawn[i].position, projectSpawn[i].rotation) as Projectile;
                newProjectile.SetSpeed(muzzleVelocity);
            }

            Instantiate(shell, shellEjectionPoint.position, shellEjectionPoint.rotation);
            muzzleFlash.Activate();
        }

    }

    public void OnTriggerHold()
    {
        Shoot();
        triggerReleaseSinceLastShoot = false;
    }

    public void OnTriggerRelease()
    {
        triggerReleaseSinceLastShoot = true;
        shootRemainInBurst = burstCount;
    }
}
