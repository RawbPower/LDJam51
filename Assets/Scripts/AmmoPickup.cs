using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AmmoPickup : Pickup
{
    public int ammo;
    public override void Activate(GameObject otherGameObject)
    {
        WeaponManager weaponManager = otherGameObject.GetComponent<WeaponManager>();
        if (weaponManager != null)
        {
            if (weaponManager.GetEquipedWeapon() is GunWeapon)
            {
                GunWeapon gun = weaponManager.GetEquipedWeapon() as GunWeapon;
                gun.IncreaseAmmo(ammo);
            }
        }

        base.Activate(otherGameObject);
    }
}
