using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    public Camera cam;

    private bool facingForward;

    private Vector2 aimDirection;

    private Vector2 movementInput;
    private Vector2 mousePosition;

    private Entity entity;
    private Rigidbody2D rb;
    private SpriteRenderer sprite;
    private Animator animator;
    private Health health;

    // Start is called before the first frame update
    void Start()
    {
        entity = GetComponent<Entity>();
        rb = GetComponent<Rigidbody2D>();
        sprite = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();
        health = GetComponent<Health>();
    }

    private void Update()
    {
        //float inputX = Input.GetAxis("Horizontal");
        //float inputY = Input.GetAxis("Vertical");

        //movementInput = new Vector2(inputX, inputY);

        //mousePosition = cam.ScreenToWorldPoint(Input.mousePosition);
        Vector2 mouseWorldPosition = cam.ScreenToWorldPoint(mousePosition);
        //aimDirection = mouseWorldPosition - rb.position;
        //aimDirection.Normalize();
        //float angle = Mathf.Atan2(aimDirection.y, aimDirection.x) * Mathf.Rad2Deg + 90.0f;
        //transform.rotation = Quaternion.Euler(0.0f, 0.0f, angle);

        facingForward = mouseWorldPosition.y < transform.position.y;

        if (facingForward)
        {
            animator.SetBool("Front", true);
        }
        else
        {
            animator.SetBool("Front", false);
        }

        if (movementInput.x > 0.0f)
        {
            sprite.flipX = false;
        }
        else if (movementInput.x < 0.0f)
        {
            sprite.flipX = true;
        }
    }

    public bool IsFacingForward()
    {
        return facingForward;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        entity.Move(movementInput);

        // Aim
        Vector2 mouseWorldPosition = cam.ScreenToWorldPoint(mousePosition);
        aimDirection = mouseWorldPosition - rb.position;
        aimDirection.Normalize();

    }

    public Vector2 GetAimDirection()
    {
        return aimDirection;
    }

    public void MoveInput(InputAction.CallbackContext context)
    {
        movementInput = context.ReadValue<Vector2>();
    }

    public void LookInput(InputAction.CallbackContext context)
    {
        mousePosition = context.ReadValue<Vector2>();
    }
}
