using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Enemy idle state
public class IdleEnemyState : EnemyState
{
    public IdleEnemyState(AIAgent agent) : base(agent)
    {
    }
    public override EnemyState UpdateTransitions() 
    {
        if (_agent.GetTarget())
        {
            //return new FollowEnemyState(_agent);
            return new AttackEnemyState(_agent);
        }

        return null; 
    }
    public override void UpdateState() 
    {
    }
    public override void OnEnterState() 
    {
        Debug.Log("Enter Idle State");
    }
    public override void OnExitState() 
    {
        Debug.Log("Exit Idle State");
    }

    public override void StateAnimation()
    {
    }
}
