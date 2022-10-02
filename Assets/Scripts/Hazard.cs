using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hazard : MonoBehaviour
{
    public int damage;
    public Hitbox hitbox;
    public float knockback;

    // Start is called before the first frame update
    void Awake()
    {
        hitbox.SetDamage(damage);
    }
}
