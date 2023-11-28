using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GunController : MonoBehaviour
{
    public Transform weaponHold;
    public Gun startGun;
    Gun equipGun;

    private void Start()
    {
        if(startGun != null)
        {
            EquipGun(startGun);
        }
    }

    public void EquipGun(Gun gunToEquip)
    {
        if (equipGun != null)
        {
            Destroy(equipGun.gameObject);
        }
        equipGun = Instantiate(gunToEquip,weaponHold.position,weaponHold.rotation) as Gun;

        equipGun.transform.parent = weaponHold;
    }

    public void Shoot()
    {
        if(equipGun != null)
        {
            equipGun.Shoot();
        }
    }
}
