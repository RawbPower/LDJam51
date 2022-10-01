using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class CameraFollow : MonoBehaviour
{
    public Camera cam;
    public Transform target;
    public float smoothSpeed = 0.125f;
    public float offsetDistance;

    private Vector2 offset;
    private Vector2 mousePosition;

    private void FixedUpdate()
    {
        if (target != null)
        {
            Vector3 mouseWorldPosition = cam.ScreenToWorldPoint(mousePosition);
            if (mouseWorldPosition.y > target.position.y)
            {
                offset = new Vector2(0.0f, 1.0f);
            }
            else
            {
                offset = new Vector2(0.0f, -1.0f);
            }
            offset *= offsetDistance;

            Vector3 desiredPosition = target.position + new Vector3(offset.x, offset.y, -10);
            Vector3 smoothPosition = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed);
            transform.position = smoothPosition;
        }
    }

    public void LookInput(InputAction.CallbackContext context)
    {
        mousePosition = context.ReadValue<Vector2>();
    }
}
