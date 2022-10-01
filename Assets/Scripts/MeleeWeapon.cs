using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeleeWeapon : Weapon
{
    public float attackRate;
    public int damage;
    public Hitbox hitbox;

    private Animator animator;

    protected override void Start()
    {
        hasAmmo = false;
        animator = GetComponent<Animator>();
        hitbox.SetDamage(damage);

        base.Start();
    }

    public override void Attack(Vector2 aimDir)
    {
        Swing(aimDir);
    }

    public void Swing(Vector2 aimDir)
    {
        float angle = Mathf.Atan2(aimDir.y, aimDir.x) * Mathf.Rad2Deg - 90.0f;
        transform.rotation = Quaternion.Euler(new Vector3(0.0f, 0.0f, angle));
        animator.SetTrigger("Attack");
    }
}