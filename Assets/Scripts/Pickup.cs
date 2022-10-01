using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pickup : MonoBehaviour
{
    public EventArea attractRange;
    public float acceleration;
    public float maxSpeed;

    private float speed;
    private bool isAttracted = false;
    private Transform target;

    private Rigidbody2D rb;

    protected void Start()
    {
        speed = 0.0f;
        rb = GetComponent<Rigidbody2D>();
        attractRange.OnEnterEventArea += Attract;
        attractRange.OnExitEventArea += UnAttract;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (isAttracted)
        {
            Vector2 moveDirection = target.position - transform.position;
            moveDirection.Normalize();
            speed += acceleration * Time.fixedDeltaTime;
            Vector2 velocity = speed * moveDirection;
            rb.MovePosition(rb.position + new Vector2(velocity.x, velocity.y) * Time.fixedDeltaTime);
        }
    }

    public virtual void Activate(GameObject otherGameObject)
    {
        Destroy(gameObject);
    }

    public void Attract(Transform transform)
    {
        isAttracted = true;
        target = transform;
        Debug.Log("Attract to " + transform.parent.name);
    }

    public void UnAttract(Transform transform)
    {
        //isAttracted = false;
        //target = null;
        Debug.Log("Stop attract to " + transform.parent.name);
    }
}
