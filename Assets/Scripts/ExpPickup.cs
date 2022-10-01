using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExpPickup : Pickup
{
    public int exp;
    public override void Activate(GameObject otherGameObject)
    {
        Experience experienceComponent = otherGameObject.GetComponent<Experience>();
        if (experienceComponent != null)
        {
            experienceComponent.IncreaseExp(exp);
        }
        Debug.Log("Exp increased by " + exp);

        base.Activate(otherGameObject);
    }
}
