using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Main FSM for enemies
public class EnemyStateMachine
{
    private EnemyState _initialState;
    private EnemyState _currentState;
    private AIAgent _agent;

    public EnemyStateMachine(AIAgent agent)
    {
        _agent = agent;
        _initialState = new IdleEnemyState(_agent);
        _currentState = _initialState;
        _currentState.OnEnterState();
    }

    public void UpdateFSM()
    {
        EnemyState transitionState = null;

        transitionState = _currentState.UpdateTransitions();

        if (transitionState != null)
        {
            _currentState.OnExitState();
            transitionState.OnEnterState();
            _currentState = transitionState;
        }

        _currentState.UpdateState();

        _currentState.StateAnimation();
    }

    void OnDestroy()
    {
        _currentState.OnExitState();
    }
}
