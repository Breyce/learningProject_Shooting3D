using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GunController : MonoBehaviour
{
    public Transform weaponHold;
    public Gun[] allGun;
    Gun equipGun;

    private void Start()
    {
        if (allGun != null)
        {
            EquipGun(1);
        }
    }

    void EquipGun(Gun gunToEquip)
    {
        if (equipGun != null)
        {
            Destroy(equipGun.gameObject);
        }
        equipGun = Instantiate(gunToEquip, weaponHold.position, weaponHold.rotation) as Gun;

        equipGun.transform.parent = weaponHold;
    }

    public void EquipGun(int weaponIndex)
    {
        EquipGun(allGun[weaponIndex]);
    }

    public void OnTriggerHold()
    {
        if (equipGun != null)
        {
            equipGun.OnTriggerHold();
        }
    }

    public void OnTriggerRelease()
    {
        if (equipGun != null)
        {
            equipGun.OnTriggerRelease();
        }
    }

    public float GunHeight
    {
        get {
            return weaponHold.position.y;
        }
    }

    public void Aim(Vector3 aimPoint){
        if (equipGun != null)
        {
            equipGun.Aim(aimPoint);
        }
    }

    public void Reload()
    {
        if (equipGun != null)
        {
            equipGun.Reload();
        }
    }
}
