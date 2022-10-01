using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Class for boxes like physics boxes, hitboxes and hurtboxes
public class Box : MonoBehaviour
{
    public enum BoxType { PlayerBox, EnemyBox };

    public Vector2 boxSize;
    public bool isOpen = true;

    protected virtual void Awake()
    {
        if (isActiveAndEnabled)
        {
            DebugBoxManager boxDebug = FindObjectOfType<DebugBoxManager>();
            boxDebug?.AddToDebugBoxes(this);
        }
    }

    protected void OnEnable()
    {
        DebugBoxManager boxDebug = FindObjectOfType<DebugBoxManager>();
        boxDebug?.AddToDebugBoxes(this);
    }

    protected void OnDisable()
    {
        DebugBoxManager boxDebug = FindObjectOfType<DebugBoxManager>();
        boxDebug?.RemoveFromDebugBoxes(this);
    }
}
