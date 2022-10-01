using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon : MonoBehaviour
{
    public Sprite uiIcon;
    public bool automatic;

    protected WeaponManager parentWeaponManager;
    protected float cooldown;
    protected bool hasAmmo;

    protected virtual void Start()
    {
        cooldown = 0.0f;

        if (transform.parent != null)
        {
            parentWeaponManager = transform.parent.gameObject.GetComponent<WeaponManager>();
        }

        if (parentWeaponManager != null)
        {
            parentWeaponManager.AmmoChanged?.Invoke();
        }
    }

    protected virtual void Update()
    {
        if (cooldown > 0.0f)
        {
            cooldown -= Time.deltaTime;
        }
        else
        {
            cooldown = 0.0f;
        }
    }


    public virtual void Attack(Vector2 aimDir)
    {
    }

    public virtual int GetAmmo()
    {
        return -1;
    }

    public bool GetHasAmmo()
    {
        return hasAmmo;
    }

    public void Equip(WeaponManager weaponManager)
    {
        parentWeaponManager = weaponManager;
    }

    public void Unequip()
    {
        parentWeaponManager = null;
    }
}
