using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Enemy state for following a target
public class FollowEnemyState : EnemyState
{

    private float slowDistance = 2.5f;
    private float _moveDirection;
    private float _distance;
    private Node _targetWaypoint;
    private Connection _targetConnection;
    int _targetIndex;
    private bool jumpedToNode = false;

    public FollowEnemyState(AIAgent agent) : base(agent)
    {
    }
    public override EnemyState UpdateTransitions()
    {
        // If there is no target or path then return to idle state
        if (!_agent.GetTarget())
        {
            return new IdleEnemyState(_agent);
        }

        if (_pathfindingAgent.grid.grid != null)
        {

            // Get nodes for agent location and target location
            Node currentNode = _pathfindingAgent.grid.GetNodeFromWorldPosition(_pathfindingAgent.pathfindingBase.position);
            Node targetNode = _pathfindingAgent.grid.GetNodeFromWorldPosition(_agent.GetTarget().transform.position);

            // If target is airborn then use the closest ground node
            if (targetNode.nodeType == Node.NodeType.Air)
            {
                targetNode = _pathfindingAgent.grid.FindNearestWalkableNode(targetNode, currentNode);
            }

            // Request a path
            if (_pathfindingAgent.pathfindCounter == 0.0f && !_pathfindingAgent.processingPath && currentNode.IsWalkableNode())
            {
                PathRequestManager.RequestPath(_pathfindingAgent.grid, _pathfindingAgent.pathfindingBase.transform.position, targetNode.worldPosition, _pathfindingAgent.pathfindingInfo.pathfindingInfoID, OnPathFound);
                _pathfindingAgent.processingPath = true;
                _pathfindingAgent.pathfindCounter = _pathfindingAgent.pathfindTime;
            }

            // If you are within attacking range then go to attack state
            Vector2 displacementFromEnemy = _agent.GetTarget().transform.position - _agent.transform.position;
            float distanceFromEnemy = displacementFromEnemy.magnitude;
            displacementFromEnemy.Normalize();

            if (distanceFromEnemy < _agent.attackDistance)
            {
                return new AttackEnemyState(_agent);
            }

            // Update path current connection
            if (_pathfindingAgent.path != null)
            {
                if (_targetWaypoint == null)
                {
                    _targetIndex = 0;
                    _targetConnection = _pathfindingAgent.path[0];
                    _targetWaypoint = _pathfindingAgent.path[0].toNode;
                }

                Node enemyNode = _pathfindingAgent.grid.GetNodeFromWorldPosition(_pathfindingAgent.pathfindingBase.transform.position);
                if (enemyNode == _targetWaypoint)
                {
                    _targetIndex++;
                    jumpedToNode = false;
                    // If target node is past the last node then return to idle state
                    if (_targetIndex >= _pathfindingAgent.path.Length)
                    {
                        _pathfindingAgent.path = null;
                        return new IdleEnemyState(_agent);
                    }

                    // Go to next connection
                    _targetConnection = _pathfindingAgent.path[_targetIndex];
                    _targetWaypoint = _pathfindingAgent.path[_targetIndex].toNode;
                }
            }
        }

        return null;
    }
    public override void UpdateState()
    {
        if (_pathfindingAgent.path != null && _targetWaypoint != null)
        {
            _moveDirection = Mathf.Sign(_targetWaypoint.worldPosition.x - _pathfindingAgent.pathfindingBase.transform.position.x);
            _distance = Mathf.Abs(_targetWaypoint.worldPosition.x - _pathfindingAgent.pathfindingBase.transform.position.x);

            // Code to move when walking (else if for other types of connection movement)
            if (_targetConnection.connectionType == Connection.ConnectionType.Walk)
            {
                Vector2 moveDirection = _targetConnection.toNode.worldPosition - _targetConnection.fromNode.worldPosition;
                moveDirection.Normalize();
                _entity.Move(moveDirection);
            }
        }

        // If you are within attacking range then go to attack state
        Vector2 displacementFromEnemy = _agent.GetTarget().transform.position - _agent.transform.position;
        float distanceFromEnemy = displacementFromEnemy.magnitude;
        displacementFromEnemy.Normalize();

        if (distanceFromEnemy < _agent.shootingDistance)
        {
            if (_agent.cooldownCounter <= 0.0f)
            {
                _agent.Attack();
                _agent.cooldownCounter = _agent.cooldown;
            }
        }
    }
    public override void OnEnterState()
    {
        Debug.Log("Enter Follow State");
    }
    public override void OnExitState()
    {
        Debug.Log("Exit Follow State");
    }

    public override void StateAnimation()
    {
    }

    // Code to run when path is found
    public void OnPathFound(Connection[] newPath, bool pathSuccessful)
    {
        _pathfindingAgent.processingPath = false;
        if (pathSuccessful)
        {
            _pathfindingAgent.path = newPath;
            if (_pathfindingAgent.path.Length > 0)
            {
                _targetIndex = 0;
                _targetConnection = _pathfindingAgent.path[0];
                _targetWaypoint = _pathfindingAgent.path[0].toNode;
            }
        }
    }
}
