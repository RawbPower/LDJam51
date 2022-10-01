using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class PickupCollider : MonoBehaviour
{
    private Pickup pickup;

    private void Start()
    {
        pickup = transform.parent.gameObject.GetComponent<Pickup>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        GameObject otherGameObject = collision.transform.parent.gameObject;
        pickup.Activate(otherGameObject);
    }
}
