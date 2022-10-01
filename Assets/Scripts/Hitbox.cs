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
        if (health != null)
        {
            health.ReduceHealth(damage);
            Debug.Log(hitObject.name + " took " + damage + " damage and now has " + health.GetHealth() + " health");
        }

        GameObject damageSource = transform.parent.gameObject;
        Bullet bullet = damageSource.GetComponent<Bullet>();
        if (bullet != null)
        {
            Entity hitEntity = hitObject.GetComponent<Entity>();
            if (hitEntity != null)
            {
                Vector2 hitDirection = bullet.GetVelocity().normalized;
                hitEntity.SetVelocity(hitDirection * bullet.knockback);
            }
        }

        Hit?.Invoke(collision);
    }
}
