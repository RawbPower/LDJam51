using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Enemy state when being damaged or is incapacitated (not always needed)
public class ProneEnemyState : EnemyState
{
    private Vector2 _movementDirection;
    private float _knockbackCounter;
    private float _knockbackSpeed;

    public ProneEnemyState(AIAgent agent, Vector2 direction) : base(agent)
    {
        _movementDirection = direction;
    }

    // No transitions by default since state is pretty case specific
    public override EnemyState UpdateTransitions()
    {
        return null;
    }
    public override void UpdateState()
    {
    }
    public override void OnEnterState()
    {
        Debug.Log("Enter Prone State");
    }
    public override void OnExitState()
    {
        _knockbackCounter = 0.0f;
        Debug.Log("Exit Prone State");
    }

    public override void StateAnimation()
    {
    }
}
