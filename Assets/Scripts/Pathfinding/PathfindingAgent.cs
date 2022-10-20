using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Class for how an agent moves along path
public class PathfindingAgent : MonoBehaviour
{
    //private float slowDistance = 2.5f;
    private Vector2 _moveDirection;
    private float _distance;
    private Node _targetWaypoint;
    private Connection _targetConnection;
    int _targetIndex;
    //private bool jumpedToNode = false;

    public PathfindingAgentInfo pathfindingInfo;

    public Transform pathfindingBase;
    public PathfindingGrid grid;

    private Entity _entity;
    private AIAgent _agent;

    [HideInInspector]
    public Connection[] path;

    public float pathfindTime = 1.0f;
    [HideInInspector]
    public float pathfindCounter = 0.0f;
    [HideInInspector]
    public bool processingPath = false;
    [HideInInspector]
    public bool shouldFollowPath = false;

    // Start is called before the first frame update
    void Start()
    {
        _entity = GetComponent<Entity>();
        _agent = GetComponent<AIAgent>();
    }

    private void Update()
    {
        if (pathfindCounter > 0)
        {
            pathfindCounter -= Time.deltaTime;
        }
        else
        {
            pathfindCounter = 0.0f;
        }

        shouldFollowPath = path != null;

        if (path != null && path.Length > 0)
        {

            if (_targetWaypoint == null)
            {
                _targetIndex = 0;
                _targetConnection = path[0];
                _targetWaypoint = path[0].toNode;
            }

            Node enemyNode = grid.GetNodeFromWorldPosition(pathfindingBase.transform.position);
            if (enemyNode == _targetWaypoint)
            {
                _targetIndex++;
                //jumpedToNode = false;
                if (_targetIndex >= path.Length)
                {
                    path = null;
                    _targetConnection = null;
                    _targetWaypoint = null;
                }
                else
                {
                    _targetConnection = path[_targetIndex];
                    _targetWaypoint = path[_targetIndex].toNode;
                }
            }
        }
    }

    public bool GetPath(Transform target)
    {
        Node currentNode = grid.GetNodeFromWorldPosition(pathfindingBase.position);
        Node targetNode = grid.GetNodeFromWorldPosition(target.position);

        if (pathfindingInfo.isFlying)
        {
            if (targetNode.nodeType != Node.NodeType.None)
            {
                PathRequestManager.RequestPath(grid, pathfindingBase.transform.position, targetNode.worldPosition + new Vector2(0.0f, 2.0f), pathfindingInfo.pathfindingInfoID, OnPathFound);
                processingPath = true;
                pathfindCounter = pathfindTime;
            }
        }
        else
        {

            if (targetNode.nodeType == Node.NodeType.Air)
            {
                targetNode = grid.FindNearestWalkableNode(targetNode, currentNode);
            }

            if (pathfindCounter == 0.0f && !processingPath && currentNode.IsWalkableNode())
            {
                PathRequestManager.RequestPath(grid, pathfindingBase.transform.position, targetNode.worldPosition, pathfindingInfo.pathfindingInfoID, OnPathFound);
                processingPath = true;
                pathfindCounter = pathfindTime;
            }
        }

        return processingPath;
    }

    public void FollowPath()
    {
        if (path != null && _targetWaypoint != null)
        {
            _moveDirection = new Vector2(Mathf.Sign(_targetWaypoint.worldPosition.x - pathfindingBase.transform.position.x), Mathf.Sign(_targetWaypoint.worldPosition.y - pathfindingBase.transform.position.y));
            _distance = Vector2.Distance(_targetWaypoint.worldPosition, pathfindingBase.transform.position);

            if (_targetConnection.connectionType == Connection.ConnectionType.Walk)
            {
            }
        }
    }

    public void OnPathFound(Connection[] newPath, bool pathSuccessful)
    {
        processingPath = false;
        if (pathSuccessful)
        {
            path = newPath;
            if (path.Length > 0)
            {
                _targetIndex = 0;
                _targetConnection = path[0];
                _targetWaypoint = path[0].toNode;
            }
        }
    }

    /*private void OnDrawGizmosSelected()
    {
        if (grid)
        {
            List<Connection>[] gridConnections = grid.GetGridConnections(pathfindingInfo.pathfindingInfoID);
            if (gridConnections == null)
            {
                return;
            }

            foreach (List<Connection> l in gridConnections)
            {
                foreach (Connection c in l)
                {
                    Color gizmoColor = Gizmos.color;
                    float gizmoAlpha = 1.0f;
                    switch (c.connectionType)
                    {
                        case Connection.ConnectionType.Walk:
                            gizmoColor = Color.green;
                            break;
                        case Connection.ConnectionType.Fly:
                            gizmoColor = Color.blue;
                            gizmoAlpha = 0.2f;
                            break;
                    }
                    Gizmos.color = new Color(gizmoColor.r, gizmoColor.g, gizmoColor.b, gizmoAlpha);
                    Gizmos.DrawLine(c.fromNode.worldPosition, c.toNode.worldPosition);
                }
            }

            /*Node mouseNode = grid.GetNodeFromWorldPosition(Camera.main.ScreenToWorldPoint(Input.mousePosition));
            foreach (List<Connection> l in gridConnections)
            {
                foreach (Connection c in l)
                {
                    if (mouseNode == c.fromNode)
                    {
                        Gizmos.color = Color.cyan;
                        Gizmos.DrawLine(c.fromNode.worldPosition, c.toNode.worldPosition);
                    }
                }
            }

            if (path != null && path.Length > 0)
            {
                List<Vector2> pathNodePositions = new List<Vector2>();
                pathNodePositions.Add(path[0].fromNode.worldPosition);
                Gizmos.color = Color.red;
                Gizmos.DrawSphere(path[0].fromNode.worldPosition, 0.3f);
                foreach (Connection c in path)
                {
                    pathNodePositions.Add(c.toNode.worldPosition);
                    Gizmos.DrawSphere(c.toNode.worldPosition, 0.3f);
                }

                LineController lineController = grid.GetComponent<LineController>();
                if (lineController)
                {
                    lineController.AddLine(pathNodePositions);
                }
            }

            if (grid != null && grid.grid != null)
            {
                Node enemyNode = grid.GetNodeFromWorldPosition(pathfindingBase.transform.position);
                Gizmos.color = Color.white;
                Gizmos.DrawWireCube(enemyNode.worldPosition, new Vector3(1.0f, 1.0f, 1.0f));
            }

            /*if (trajectoryPoints != null)
            {
                foreach (Vector2 trajectoryPoint in trajectoryPoints)
                {
                    Gizmos.color = Color.white;
                    Gizmos.DrawSphere(trajectoryPoint, 0.1f);
                    Vector2 collisionBound = new Vector2(1.0f, pathfindingInfo.agentCollider.bounds.size.y);
                    Gizmos.DrawWireCube(trajectoryPoint, collisionBound);
                }
            }
        }
    }

#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        if (UnityEditor.Selection.activeGameObject && !UnityEditor.Selection.activeGameObject.GetComponent<PathfindingAgent>())
        {
            LineController lineController = grid.GetComponent<LineController>();
            if (lineController)
            {
                lineController.RemoveLine();
            }
        }
    }
#endif*/
}
