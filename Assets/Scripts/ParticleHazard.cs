using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticleHazard : MonoBehaviour
{
    public int damage = 1;

    public ParticleSystem part;
    public List<ParticleCollisionEvent> collisionEvents;

    void Start()
    {
        part = GetComponent<ParticleSystem>();
        collisionEvents = new List<ParticleCollisionEvent>();
    }

    private void OnParticleCollision(GameObject other)
    {
        Health health = other.GetComponent<Health>();

        if (health != null)
        {
            health.ReduceHealth(damage);
        }
    }
}
