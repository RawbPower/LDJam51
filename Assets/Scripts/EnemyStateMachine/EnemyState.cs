using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Base class of enemy state in the AI Agent FSM
public class EnemyState
{
    protected AIAgent _agent;
    protected Entity _entity;
    protected PathfindingAgent _pathfindingAgent;

    public EnemyState(AIAgent agent) 
    {
        _agent = agent;
        _entity = _agent.GetEntity();

        if (_agent)
        {
            _pathfindingAgent = _agent.pathfindingAgent;
        }
    }
    ~EnemyState() { }

    // Check for any state transitions
    public virtual EnemyState UpdateTransitions() { return null; }
    // Update current state
    public virtual void UpdateState() { }
    // Enter state code
    public virtual void OnEnterState() { }
    // Exit state code
    public virtual void OnExitState() { }
    // Animation played in current state
    public virtual void StateAnimation() { }
}
