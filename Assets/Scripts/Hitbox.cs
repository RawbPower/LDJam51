using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[RequireComponent(typeof(Collider2D))]
public class Hitbox : MonoBehaviour
{
    public Action<Collider2D> Hit;

    private int damage = 0;

    public void SetDamage(int newDamage)
    {
        damage = newDamage;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        GameObject hitObject = collision.transform.parent.gameObject;
        Health health = hitObject.GetComponent<Health>();
        PlayerCombat player = hitObject.GetComponent<PlayerCombat>();
        Decoration decoration = hitObject.GetComponent<Decoration>();
        GameObject damageSource = transform.parent.gameObject;
        Bullet bullet = damageSource.GetComponent<Bullet>();

        if (bullet != null && hitObject.CompareTag(bullet.ownerTag))
        {
            return;
        }

        if (player && player.HasGameEnded())
        {
            return;
        }

        if (decoration != null)
        {
            decoration.OnChopDecoration();
        }

        Vector2 hitDirection = Vector2.zero;

        if (bullet != null)
        {
            Entity hitEntity = hitObject.GetComponent<Entity>();
            if (hitEntity != null)
            {
                hitDirection = bullet.GetVelocity().normalized;
                if (!hitObject.CompareTag("Tenticle"))
                {
                    hitEntity.SetVelocity(hitDirection * bullet.knockback);
                }
            }
        }

        Hazard hazard = damageSource.GetComponent<Hazard>();
        if (hazard != null)
        {
            Entity hitEntity = hitObject.GetComponent<Entity>();
            if (hitEntity != null)
            {
                hitDirection = hitObject.transform.position - damageSource.transform.position;
                hitDirection.Normalize();
                if (!hitObject.CompareTag("Tenticle"))
                {
                    hitEntity.SetVelocity(hitDirection * hazard.knockback);
                }
            }
        }

        MeleeWeapon weapon = damageSource.GetComponent<MeleeWeapon>();
        if (weapon != null)
        {
            Entity hitEntity = hitObject.GetComponent<Entity>();
            if (hitEntity != null)
            {
                hitDirection = hitObject.transform.position - damageSource.transform.parent.position;
                hitDirection.Normalize();
                if (!hitObject.CompareTag("Tenticle"))
                {
                    hitEntity.SetVelocity(hitDirection * weapon.knockback);
                }
            }
        }

        if (health != null)
        {
            health.ReduceHealth(damage);
            Debug.Log(hitObject.name + " took " + damage + " damage and now has " + health.GetHealth() + " health");

            if (hitDirection != Vector2.zero && health.bloodParticle != null)
            {
                float angle = Mathf.Atan2(hitDirection.y, hitDirection.x) * Mathf.Rad2Deg;
                Quaternion bloodAngle = Quaternion.Euler(0.0f, 0.0f, angle);
                GameObject blood = Instantiate(health.bloodParticle.gameObject, hitObject.transform.position, bloodAngle);
                //blood.transform.parent = hitObject.transform;
            }
        }

        Hit?.Invoke(collision);
    }
}
