using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

// Invokes an event when something enters or exits it's trigger collider.
[RequireComponent(typeof(Collider2D))]
public class EventArea : MonoBehaviour
{
    public Action<Transform> OnEnterEventArea;
    public Action<Transform> OnExitEventArea;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        OnEnterEventArea?.Invoke(collision.transform);
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        OnExitEventArea?.Invoke(collision.transform);
    }
}
