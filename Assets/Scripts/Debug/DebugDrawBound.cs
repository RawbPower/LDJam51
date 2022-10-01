using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct Bound
{
    public BoundType type;
    public string name;
    public Color color;
    public bool isTrigger;
}

public enum BoundType { Collision, Hitbox, AttackRange }

[ExecuteInEditMode]
public class DebugDrawBound : MonoBehaviour
{
    // TODO: Add debug class of player that controls if it shows the bound. Make this part of the bound struct so all like bounds are effected.
    public bool showDebug;
    public BoundType boundType;

    private Bound bound;
    private SpriteRenderer sr;
    private BoxCollider2D bc;

    // Start is called before the first frame update
    void Start()
    {
        sr = GetComponent<SpriteRenderer>();
        bc = GetComponent<BoxCollider2D>();

        InitBound(boundType);
    }

    // Update is called once per frame
    void Update()
    {
        if (showDebug)
        {
            sr.enabled = true;
            sr.color = bound.color;
            sr.sortingLayerName = bound.name;
            bc.isTrigger = bound.isTrigger;

            if (bound.type != boundType)
            {
                InitBound(boundType);
            }
        }
        else
        {
            sr.enabled = false;
        }
    }

    private void InitBound(BoundType boundType)
    {
        bound.type = boundType;
        if (bound.type == BoundType.Collision)
        {
            bound.color = new Color(0f, 0f, 1f, 0.5f);
            bound.name = "Collision";
            bound.isTrigger = false;
        }
        else if (bound.type == BoundType.Hitbox)
        {
            bound.color = new Color(0f, 1f, 0f, 0.5f);
            bound.name = "Hitbox";
            bound.isTrigger = true;
        }
        else if (bound.type == BoundType.AttackRange)
        {
            bound.color = new Color(1f, 0f, 0f, 0.5f);
            bound.name = "AttackRange";
            bound.isTrigger = true;
        }
        else
        {
            Debug.LogError("Invalid Bound Type");
        }
    }
}
