using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class WeaponManager : MonoBehaviour
{
    public Action AmmoChanged;
    public Action WeaponChanged;
    public Weapon[] weapons;

    private int equipedWeaponIndex;

    private void Start()
    {
        equipedWeaponIndex = 0;
        WeaponChanged?.Invoke();
    }

    public void CycleWeapon(bool cycleBack = false)
    {
        weapons[equipedWeaponIndex].Unequip();
        equipedWeaponIndex = cycleBack ? equipedWeaponIndex - 1 : equipedWeaponIndex + 1;
        equipedWeaponIndex = equipedWeaponIndex % weapons.Length;
        while (equipedWeaponIndex < 0)
        {
            equipedWeaponIndex += weapons.Length;
        }
        weapons[equipedWeaponIndex].Equip(this);
        WeaponChanged?.Invoke();
    }

    public Weapon GetEquipedWeapon()
    {
        return weapons[equipedWeaponIndex];
    }
}
