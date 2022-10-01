using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HUD : MonoBehaviour
{
    public Image weaponUI;
    public Text healthText;
    public Health health;
    public Text ammoText;
    public WeaponManager weaponManager;
    public Text expText;
    public Experience experience;

    private void Start()
    {
        health.HealthChanged += OnHealthChanged;
        weaponManager.AmmoChanged += OnAmmoChanged;
        weaponManager.WeaponChanged += OnWeaponChanged;
        experience.ExperienceChanged += OnExperienceChanged;
    }

    void OnHealthChanged()
    {
        healthText.text = health.GetHealth().ToString();
    }

    void OnAmmoChanged()
    {
        ammoText.text = weaponManager.GetEquipedWeapon().GetAmmo() >= 0 ? weaponManager.GetEquipedWeapon().GetAmmo().ToString() : "-";
    }

    void OnWeaponChanged()
    {
        weaponUI.sprite = weaponManager.GetEquipedWeapon().uiIcon;
        OnAmmoChanged();
    }

    void OnExperienceChanged()
    {
        expText.text = experience.GetExperience().ToString();
    }
}
