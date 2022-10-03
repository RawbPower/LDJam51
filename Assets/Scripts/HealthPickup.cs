using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthPickup : Pickup
{
    public int health;
    public override void Activate(GameObject otherGameObject)
    {
        Health healthComponent = otherGameObject.GetComponent<Health>();
        if (healthComponent != null)
        {
            healthComponent.IncreaseHealth(health);
        }

        base.Activate(otherGameObject);
    }
}
