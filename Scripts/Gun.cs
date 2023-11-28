using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gun : MonoBehaviour
{
    public Transform muzzle;
    public Projectile projectile;
    public float msBetweenShoots = 1000;
    public float muzzleVelocity = 35;

    float nextShotTime;

    public void Shoot()
    {
        if(Time.time > nextShotTime)//Éä»÷¼ä¸ô
        {
            nextShotTime = Time.time + msBetweenShoots / 1000;

            Projectile newProjectile = Instantiate(projectile, muzzle.position, muzzle.rotation) as Projectile;
            newProjectile.SetSpeed(muzzleVelocity);
        }

    }

}
