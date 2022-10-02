using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Entity : MonoBehaviour
{
    public float accelerationMag;
    public float maxSpeed;
    public float frictionCoefficient;
    public bool instantStop;

    private bool dashing = false;
    private float dashTimer = 0.0f;
    private float speed;
    private Vector3 acceleration;
    private Vector3 velocity;
    private Vector3 position;
    private Vector3 friction;

    private Rigidbody2D rb;
    private Animator animator;

    // Start is called before the first frame update
    protected void Start()
    {
        acceleration = Vector3.zero;
        velocity = Vector3.zero;
        position = Vector3.zero;
        speed = 0.0f;

        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        if (dashTimer > 0)
        {
            dashing = true;
            dashTimer -= Time.deltaTime;
            if (dashTimer <= 0.0f)
            {
                dashTimer = 0.0f;
                dashing = false;
            }
        }
    }

    public void SetVelocity(Vector2 setVel)
    {
        velocity = setVel;
    }

    public Vector2 GetVelocity() { return velocity; }

    public void Dash(Vector2 dashVelocity, float dashTime)
    {
        velocity = dashVelocity;
        dashTimer = dashTime;
        dashing = true;
    }

    public void Move(Vector2 movementInput)
    {
        // Movement
        if (!dashing)
        {
            acceleration = new Vector3(accelerationMag * movementInput.x, accelerationMag * movementInput.y, 0.0f);

            velocity += acceleration * Time.deltaTime;

            speed = velocity.magnitude;

            if (speed > 0.0f && movementInput.x == 0.0f && movementInput.y == 0.0f)
            {
                if (instantStop)
                {
                    velocity = Vector2.zero;
                }
                else
                {
                    velocity -= Mathf.Min(velocity.magnitude, frictionCoefficient) * velocity.normalized;
                }
            }

            if (speed > maxSpeed)
            {
                velocity = (velocity / speed) * maxSpeed;
                speed = maxSpeed;
            }

            if (speed < 0.2f)
            {
                velocity = Vector3.zero;
                speed = 0.0f;
            }
        }

        if (animator)
        {
            //animator.SetFloat("Speed", speed);
        }

        rb.MovePosition(rb.position + new Vector2(velocity.x, velocity.y) * Time.fixedDeltaTime);
    }
}
