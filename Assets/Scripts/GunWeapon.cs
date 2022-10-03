using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GunWeapon : Weapon
{
    public enum ShotType { Single, Triple, Burst, Circle };

    public GameObject bulletPrefab;
    public GameObject flashPrefab;
    public ShotType shotType;
    public float fireRate;
    public int maxAmmo;
    public int startAmmo = -1;
    public int clipSize = 1;
    public float spray;
    public float flashOffset = 0.5f;
    public float bulletOffset = 1.0f;

    private int currentAmmo;
    private int currentClip;

    // Start is called before the first frame update
    protected override void Start()
    {
        hasAmmo = true;
        currentClip = clipSize;
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

    public void Reload()
    {
        currentClip = clipSize;
    }

    public int GetCurrentClip()
    {
        return currentClip;
    }

    public void Shoot(Vector2 aimDir)
    {

        if (base.cooldown <= 0.0f && currentAmmo > 0)
        {
            if (currentClip <= 0)
            {
                Reload();
                return;
            }

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
                case ShotType.Circle:
                    StartCoroutine(CircleShot(aimDir));
                    break;
            }

            if (fireRate > 0.0f)
            {
                base.cooldown = 1.0f / fireRate;
            }

            currentClip--;
        }
    }

    IEnumerator SingleShot(Vector2 aimDir)
    {
        float angle = Mathf.Atan2(aimDir.y, aimDir.x) * Mathf.Rad2Deg - 90.0f;        // -90 degree offset due to original direction being 90 degrees from x axis
        float sprayAngle = Random.Range(-spray, spray);
        Vector2 rotatedAimDir = Quaternion.Euler(0.0f, 0.0f, sprayAngle) * aimDir;
        Vector2 bulletPosition = new Vector2(transform.position.x, transform.position.y) + bulletOffset * aimDir;
        Quaternion bulletRotation = Quaternion.Euler(0.0f, 0.0f, angle);
        Vector2 flashPosition = new Vector2(transform.position.x, transform.position.y) + flashOffset * aimDir;
        Instantiate(flashPrefab, flashPosition, Quaternion.identity);
        yield return new WaitForSeconds(0.15f);
        GameObject bulletObject = Instantiate(bulletPrefab, bulletPosition, bulletRotation);
        Bullet bullet = bulletObject.GetComponent<Bullet>();
        bullet.SetInitialDirection(rotatedAimDir);
        bullet.ownerTag = transform.parent.gameObject.tag;
        ReduceAmmo(1);
    }

    IEnumerator TripleShot(Vector2 aimDir)
    {
        float angle = Mathf.Atan2(aimDir.y, aimDir.x) * Mathf.Rad2Deg - 90.0f;        // -90 degree offset due to original direction being 90 degrees from x axis
        float[] angles = { angle - 25.0f, angle, angle + 25.0f };
        Vector2 bulletPosition = new Vector2(transform.position.x, transform.position.y) + bulletOffset * aimDir;
        Vector2 flashPosition = new Vector2(transform.position.x, transform.position.y) + flashOffset * aimDir;
        Instantiate(flashPrefab, flashPosition, Quaternion.identity);
        yield return new WaitForSeconds(0.15f);
        foreach (float bulletAngle in angles)
        {
            if (currentAmmo >= 0)
            {
                Vector2 rotatedAimDir = Quaternion.Euler(0.0f, 0.0f, bulletAngle - angle) * aimDir;
                Quaternion bulletRotation = Quaternion.Euler(0.0f, 0.0f, bulletAngle);
                GameObject bulletObject = Instantiate(bulletPrefab, bulletPosition, bulletRotation);
                Bullet bullet = bulletObject.GetComponent<Bullet>();
                bullet.SetInitialDirection(rotatedAimDir);
                bullet.ownerTag = transform.parent.gameObject.tag;
                ReduceAmmo(1);
            }
        }
    }

    IEnumerator CircleShot(Vector2 aimDir)
    {
        float angle = Random.Range(-180.0f, 180.0f);        // -90 degree offset due to original direction being 90 degrees from x axis
        float[] angles = { angle, angle + 60.0f, angle + 120.0f, angle + 180.0f, angle + 240.0f, angle + 300.0f };
        Vector2 bulletPosition = new Vector2(transform.position.x, transform.position.y) + bulletOffset * aimDir;
        Vector2 flashPosition = new Vector2(transform.position.x, transform.position.y) + flashOffset * aimDir;
        Instantiate(flashPrefab, flashPosition, Quaternion.identity);
        yield return new WaitForSeconds(0.15f);
        foreach (float bulletAngle in angles)
        {
            if (currentAmmo >= 0)
            {
                Vector2 rotatedAimDir = Quaternion.Euler(0.0f, 0.0f, bulletAngle - angle) * aimDir;
                Quaternion bulletRotation = Quaternion.Euler(0.0f, 0.0f, bulletAngle);
                GameObject bulletObject = Instantiate(bulletPrefab, bulletPosition, bulletRotation);
                Bullet bullet = bulletObject.GetComponent<Bullet>();
                bullet.SetInitialDirection(rotatedAimDir);
                bullet.ownerTag = transform.parent.gameObject.tag;
                ReduceAmmo(1);
            }
        }
    }

    IEnumerator BurstShot(Vector2 aimDir)
    {
        float angle = Mathf.Atan2(aimDir.y, aimDir.x) * Mathf.Rad2Deg - 90.0f;        // -90 degree offset due to original direction being 90 degrees from x axis
        float[] angles = { angle - 12.0f, angle, angle - 6.0f, angle, angle + 6.0f, angle + 12.0f};
        Vector2 bulletPosition = new Vector2(transform.position.x, transform.position.y) + bulletOffset * aimDir;
        Vector2 flashPosition = new Vector2(transform.position.x, transform.position.y) + flashOffset * aimDir;
        Instantiate(flashPrefab, flashPosition, Quaternion.identity);
        yield return new WaitForSeconds(0.15f);
        foreach (float bulletAngle in angles)
        {
            Vector2 rotatedAimDir = Quaternion.Euler(0.0f, 0.0f, bulletAngle - angle) * aimDir;
            Quaternion bulletRotation = Quaternion.Euler(0.0f, 0.0f, bulletAngle);
            GameObject bulletObject = Instantiate(bulletPrefab, bulletPosition, bulletRotation);
            Bullet bullet = bulletObject.GetComponent<Bullet>();
            bullet.SetInitialDirection(rotatedAimDir);
            bullet.ownerTag = transform.parent.gameObject.tag;
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
