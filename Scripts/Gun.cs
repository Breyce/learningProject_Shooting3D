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
    public float reloadTime = 2f;

    [Header("Reload Feature")]
    public int projectilesPerMag;

    [Header("Recoil Feature")]
    public Vector2 kickMinMax = new Vector2(.05f,.2f);
    public Vector2 recoilAngleMinMax = new Vector2(3,5);
    public float recoilMoveSettleTime = .1f;
    public float recoilRotationSettleTime = .1f;

    //feature for Burst fire

    float nextShotTime;

    [Header("Effect of Gun")]
    public Transform shell;
    public Transform shellEjectionPoint;
    public MuzzleFlash muzzleFlash;
    public int burstCount;
    public AudioClip shootAudio;
    public AudioClip reloadSound;

    bool isReloading;
    bool triggerReleaseSinceLastShoot;
    int shootRemainInBurst;
    int projectilesRemainInMag;
    float recoilAngleSmoothDampVelocity;
    float recoilAngle;
    
    
    Vector3 recoilSmoothDampVelocity;
    GameUI gameUI;


    private void Start()
    {
        muzzleFlash = GetComponent<MuzzleFlash>();
        gameUI = FindObjectOfType<Canvas>().GetComponent<GameUI>();
        shootRemainInBurst = burstCount;

        //参数初始化
        kickMinMax = new Vector2(.05f, .2f);
        recoilAngleMinMax = new Vector2(3, 5);
        recoilMoveSettleTime = .1f;
        recoilRotationSettleTime = .1f;
        projectilesRemainInMag = projectilesPerMag;
        gameUI.bulletLeft = projectilesPerMag;
    }

    private void LateUpdate()
    {
        transform.localPosition = Vector3.SmoothDamp(transform.localPosition, Vector3.zero,ref recoilSmoothDampVelocity, recoilMoveSettleTime);
        recoilAngle = Mathf.SmoothDamp(recoilAngle, 0, ref recoilAngleSmoothDampVelocity, recoilRotationSettleTime);
        transform.localEulerAngles = transform.localEulerAngles + Vector3.right * -recoilAngle;

        if(!isReloading && projectilesRemainInMag <= 0) 
        {
            Reload();
        }

    }
    void Shoot()
    {
        if (!isReloading && Time.time > nextShotTime && projectilesRemainInMag > 0)//射击间隔
        {
            if (fireMode == FireMode.Burst)
            {
                if (shootRemainInBurst == 0)
                {
                    return;
                }
                shootRemainInBurst--;
            }
            else if (fireMode == FireMode.Single)
            {
                if (!triggerReleaseSinceLastShoot)
                {
                    return;
                }
            }
            nextShotTime = Time.time + msBetweenShoots / 1000;

            for (int i = 0; i < projectSpawn.Length; i++)
            {
                if (projectilesRemainInMag == 0)
                {
                    return;
                }
                projectilesRemainInMag--;
                Projectile newProjectile = Instantiate(projectile, projectSpawn[i].position, projectSpawn[i].rotation) as Projectile;
                newProjectile.SetSpeed(muzzleVelocity);
            }

            Instantiate(shell, shellEjectionPoint.position, shellEjectionPoint.rotation);
            muzzleFlash.Activate();

            transform.localPosition -= Vector3.forward * Random.Range(kickMinMax.x, kickMinMax.y);
            recoilAngle += Random.Range(recoilAngleMinMax.x, recoilAngleMinMax.y);
            recoilAngle = Mathf.Clamp(recoilAngle, 0, 30);

            AudioManager.instance.PlaySound(shootAudio, transform.position);

            //子弹数量修改
            gameUI.bulletLeft = projectilesRemainInMag;
        }
    }
            
    public void Reload()
    {
        if(!isReloading && projectilesRemainInMag != projectilesPerMag)
        {
            StartCoroutine(AnimateReload());
            AudioManager.instance.PlaySound(reloadSound, transform.position);
        }
    }

    IEnumerator AnimateReload()
    {
        isReloading = true;

        yield return new WaitForSeconds(.2f);

        float percent = 0;
        float reloadSpeed = 1f / reloadTime;
        Vector3 initialRot = transform.localEulerAngles;
        float maxReloadAngle = 30;

        while (percent < 1)
        {
            percent += Time.deltaTime * reloadSpeed;

            //显示换弹时间
            gameUI.reloadTime = reloadTime - percent * reloadTime;

            float interpolatioon = (-Mathf.Pow(percent,2) + percent) * 4;
            float reloadAngle = Mathf.Lerp(0, maxReloadAngle, interpolatioon);

            transform.localEulerAngles = initialRot + Vector3.left * reloadAngle;

            yield return null;
        }

        isReloading = false;
        projectilesRemainInMag = projectilesPerMag;

        //子弹数量修改
        gameUI.bulletLeft = projectilesRemainInMag;
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

    public void Aim(Vector3 aimPoint)
    {
        if (!isReloading)
        {
            transform.LookAt(aimPoint);
        }
    }

}
