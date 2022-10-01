using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GunWeapon : Weapon
{
    public enum ShotType { Single, Triple, Burst };

    public GameObject bulletPrefab;
    public GameObject flashPrefab;
    public ShotType shotType;
    public float fireRate;
    public int maxAmmo;
    public int startAmmo = -1;

    private int currentAmmo;

    // Start is called before the first frame update
    protected override void Start()
    {
        hasAmmo = true;
        currentAmmo = maxAmmo;
        if (startAmmo >= 0)
        {
            currentAmmo = startAmmo;
        }

        base.Start();

    }

    public override void Attack(Vector2 aimDir)
    {
        Shoot(aimDir);
    }

    public void Shoot(Vector2 aimDir)
    {
        if (base.cooldown <= 0.0f && currentAmmo > 0)
        {
            switch (shotType)
            {
                case ShotType.Single:
                    StartCoroutine(SingleShot(aimDir));
                    break;
                case ShotType.Triple:
                    StartCoroutine(TripleShot(aimDir));
                    break;
                case ShotType.Burst:
                    StartCoroutine(BurstShot(aimDir));
                    break;
            }

            if (fireRate > 0.0f)
            {
                base.cooldown = 1.0f / fireRate;
            }
        }
    }

    IEnumerator SingleShot(Vector2 aimDir)
    {
        float angle = Mathf.Atan2(aimDir.y, aimDir.x) * Mathf.Rad2Deg - 90.0f;        // -90 degree offset due to original direction being 90 degrees from x axis
        Vector2 bulletPosition = new Vector2(transform.position.x, transform.position.y) + 1.2f * aimDir;
        Quaternion bulletRotation = Quaternion.Euler(0.0f, 0.0f, angle);
        Vector2 flashPosition = new Vector2(transform.position.x, transform.position.y) + 0.5f * aimDir;
        Instantiate(flashPrefab, flashPosition, Quaternion.identity);
        yield return new WaitForSeconds(0.15f);
        GameObject bulletObject = Instantiate(bulletPrefab, bulletPosition, bulletRotation);
        Bullet bullet = bulletObject.GetComponent<Bullet>();
        bullet.SetInitialDirection(aimDir);
        ReduceAmmo(1);
    }

    IEnumerator TripleShot(Vector2 aimDir)
    {
        float angle = Mathf.Atan2(aimDir.y, aimDir.x) * Mathf.Rad2Deg - 90.0f;        // -90 degree offset due to original direction being 90 degrees from x axis
        float[] angles = { angle - 20.0f, angle, angle + 20.0f };
        Vector2 bulletPosition = new Vector2(transform.position.x, transform.position.y) + 1.2f * aimDir;
        Vector2 flashPosition = new Vector2(transform.position.x, transform.position.y) + 0.5f * aimDir;
        Instantiate(flashPrefab, flashPosition, Quaternion.identity);
        yield return new WaitForSeconds(0.15f);
        foreach (float bulletAngle in angles)
        {
            if (currentAmmo <= 0)
            {
                Vector2 rotatedAimDir = Quaternion.Euler(0.0f, 0.0f, bulletAngle - angle) * aimDir;
                Quaternion bulletRotation = Quaternion.Euler(0.0f, 0.0f, bulletAngle);
                GameObject bulletObject = Instantiate(bulletPrefab, bulletPosition, bulletRotation);
                Bullet bullet = bulletObject.GetComponent<Bullet>();
                bullet.SetInitialDirection(rotatedAimDir);
                ReduceAmmo(1);
            }
        }
    }

    IEnumerator BurstShot(Vector2 aimDir)
    {
        float angle = Mathf.Atan2(aimDir.y, aimDir.x) * Mathf.Rad2Deg - 90.0f;        // -90 degree offset due to original direction being 90 degrees from x axis
        float[] angles = { angle - 18.0f, angle - 12.0f, angle, angle - 6.0f, angle, angle + 6.0f, angle + 12.0f, angle + 18.0f };
        Vector2 bulletPosition = new Vector2(transform.position.x, transform.position.y) + 1.2f * aimDir;
        Vector2 flashPosition = new Vector2(transform.position.x, transform.position.y) + 0.5f * aimDir;
        Instantiate(flashPrefab, flashPosition, Quaternion.identity);
        yield return new WaitForSeconds(0.15f);
        foreach (float bulletAngle in angles)
        {
            Vector2 rotatedAimDir = Quaternion.Euler(0.0f, 0.0f, bulletAngle - angle) * aimDir;
            Quaternion bulletRotation = Quaternion.Euler(0.0f, 0.0f, bulletAngle);
            GameObject bulletObject = Instantiate(bulletPrefab, bulletPosition, bulletRotation);
            Bullet bullet = bulletObject.GetComponent<Bullet>();
            bullet.SetInitialDirection(rotatedAimDir);
        }
        ReduceAmmo(1);
    }

    public void IncreaseAmmo(int ammo)
    {
        currentAmmo += ammo;
        currentAmmo = Mathf.Min(currentAmmo, maxAmmo);
        if (parentWeaponManager != null)
        {
            parentWeaponManager.AmmoChanged?.Invoke();
        }
    }

    public void ReduceAmmo(int ammo)
    {
        currentAmmo -= ammo;
        currentAmmo = Mathf.Max(currentAmmo, 0);
        if (parentWeaponManager != null)
        {
            parentWeaponManager.AmmoChanged?.Invoke();
        }
    }

    public override int GetAmmo()
    {
        return currentAmmo;
    }
}
