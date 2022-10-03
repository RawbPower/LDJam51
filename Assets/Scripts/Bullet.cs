using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    public int damage = 1;
    public float speed;
    public float speedVariation;
    public AnimationCurve acceleration;
    public float time;
    public float timeVariation;
    public bool bounce;
    public bool penetrate;
    public float knockback;
    public GameObject explosion;
    public Hitbox hitbox;
    public bool noRotation = false;
    [HideInInspector]
    public string ownerTag;

    private float startSpeed;
    private float bulletTime;
    private Vector2 velocity;
    private Vector2 initalDirection;
    private Vector2 initialPosition;
    private Vector2 lastBounceNormal;
    private float timeAlive;
    private float currentSpeed;

    private Rigidbody2D rb;

    private void Start()
    {
        hitbox.SetDamage(damage);
        hitbox.Hit += OnHit;
        rb = GetComponent<Rigidbody2D>();
        initialPosition = rb.position;

        timeAlive = 0.0f;

        if (speedVariation > 0.0f)
        {
            startSpeed = Random.Range(speed - speedVariation, speed + speedVariation);
        }
        else
        {
            startSpeed = speed;
        }

        if (timeVariation > 0.0f)
        {
            bulletTime = Random.Range(time - timeVariation, time + timeVariation);
        }
        else
        {
            bulletTime = time;
        }
    }

    private void Update()
    {
        Vector2 aimDir = initalDirection;
        float angle = Mathf.Atan2(aimDir.y, aimDir.x) * Mathf.Rad2Deg - 90.0f;        // -90 degree offset due to original direction being 90 degrees from x axis
        Quaternion bulletRotation = Quaternion.Euler(0.0f, 0.0f, angle);
        transform.rotation = bulletRotation;

        if (noRotation)
        {
            transform.rotation = Quaternion.identity;
        }
    }

    private void FixedUpdate()
    {
        currentSpeed = startSpeed;
        if (bulletTime > 0)
        {
            float timeRatio = timeAlive / bulletTime;
            if (timeRatio >= 1.0f)
            {
                if (explosion != null)
                {
                    Instantiate(explosion, transform.position, Quaternion.identity);
                }
                Destroy(gameObject);
            }
            timeRatio = Mathf.Clamp(timeRatio, 0.0f, 1.0f);
            currentSpeed = startSpeed * acceleration.Evaluate(timeRatio);
        }

        velocity = currentSpeed * initalDirection;
        rb.MovePosition(rb.position + new Vector2(velocity.x, velocity.y) * Time.fixedDeltaTime);
        timeAlive += Time.fixedDeltaTime;
    }

    private void OnHit(Collider2D collision)
    {
        if (penetrate)
        {
            // NOthing
        }
        else
        {
            if (explosion != null)
            {
                Instantiate(explosion, transform.position, Quaternion.identity);
            }
            Destroy(gameObject);
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        GameObject hitObject = collision.transform.parent.gameObject;
        Health health = hitObject.GetComponent<Health>();
        if (health != null)
        {
            health.ReduceHealth(damage);
            Debug.Log(hitObject.name + " took " + damage + " damage and now has " + health.GetHealth() + " health");
        }

        if (bounce && bulletTime > 0)
        {
            initalDirection = Vector2.Reflect(initalDirection, collision.contacts[0].normal);
            lastBounceNormal = collision.contacts[0].normal;
        }
        else
        {
            if (explosion != null)
            {
                Instantiate(explosion, transform.position, Quaternion.identity);
            }
            Destroy(gameObject);
        }
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        if (bounce && bulletTime > 0)
        {
            if (collision.contacts[0].normal != lastBounceNormal)
            {
                Debug.Log("Stay Bounce");
                initalDirection = Vector2.Reflect(initalDirection, collision.contacts[0].normal);
                lastBounceNormal = collision.contacts[0].normal;
            }
        }
    }

    public void SetInitialDirection(Vector2 direction)
    {
        initalDirection = direction;
    }

    public Vector2 GetVelocity() { return velocity; }
}
