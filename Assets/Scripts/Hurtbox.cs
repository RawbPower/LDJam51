using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hurtbox : Box
{
    public LayerMask contactMask;

    protected override void Awake()
    {
        base.Awake();
        gameObject.layer = 10;
    }

    private void Update()
    {
        Collider2D myCollider = gameObject.GetComponent<Collider2D>();
        int numColliders = 10;
        Collider2D[] colliders = new Collider2D[numColliders];
        ContactFilter2D contactFilter = new ContactFilter2D();
        contactFilter.layerMask = contactMask;
        int colliderCount = myCollider.OverlapCollider(contactFilter, colliders);

        for (int i = 0; i < colliderCount; i++)
        {
            Collider2D aCollider = colliders[i];

            if (aCollider.gameObject.CompareTag("Player"))
            {
                // Contact Damage
            }
        }
    }
}
