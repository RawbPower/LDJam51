using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Enemy state for when enemy is attacking
public class AttackEnemyState : EnemyState
{
    public AttackEnemyState(AIAgent agent) : base(agent)
    {
    }
    public override EnemyState UpdateTransitions() 
    {
        if (!_agent.GetTarget())
        {
            return new IdleEnemyState(_agent);
        }

        /*Vector2 moveDirection = _agent.GetTarget().transform.position - _agent.transform.position;
        float distanceSq = moveDirection.sqrMagnitude;
        moveDirection.Normalize();

        if (distanceSq > _agent.attackDistance * _agent.attackDistance)
        {
            return new FollowEnemyState(_agent);
        }*/

        return null; 
    }
    public override void UpdateState()
    {
        if (_agent.cooldownCounter <= 0.0f)
        {
            _agent.Attack();
            GunWeapon gun = _agent.GetGun();
            if (gun != null && gun.GetCurrentClip() <= 0)
            {
                _agent.cooldownCounter = _agent.cooldown;
            }
            else if (gun == null)
            {
                _agent.cooldownCounter = _agent.cooldown;
            }
        }
    }
    public override void OnEnterState() 
    {
    }
    public override void OnExitState() 
    {
    }

    public override void StateAnimation()
    {
    }
}
