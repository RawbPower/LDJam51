using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using System.Diagnostics;
using Debug = UnityEngine.Debug;
using UnityEngine.InputSystem;

// Navmesh for pathfinding grid
public class NavigationMesh
{
    public PathfindingAgentInfo pathfindingAgentInfo;
    public List<Connection>[] gridConnections;
    public List<Vector2> trajectoryPoints;

    public List<JumpInfo>[,] jumpArray;

    public NavigationMesh(PathfindingAgentInfo agentInfo, Vector2Int gridWorldSize)
    {
        pathfindingAgentInfo = agentInfo;
        Init(gridWorldSize);
    }

    public void Init(Vector2Int gridWorldSize)
    {
        gridConnections = new List<Connection>[gridWorldSize.x * gridWorldSize.y];
        for (int i = 0; i < gridConnections.Length; i++)
        {
            gridConnections[i] = new List<Connection>();
        }

        trajectoryPoints = new List<Vector2>();
        jumpArray = new List<JumpInfo>[pathfindingAgentInfo.jumpRange + 1, pathfindingAgentInfo.jumpHeight + 1];
        for (int i = 0; i < pathfindingAgentInfo.jumpRange + 1; i++)
        {
            for (int j = 0; j < pathfindingAgentInfo.jumpHeight + 1; j++)
            {
                jumpArray[i, j] = new List<JumpInfo>();
            }
        }
    }
}

// Pathfinding grid
public class PathfindingGrid : MonoBehaviour
{
    public bool gridDebugDraw;
    public LayerMask collisionMask;
    public Vector2Int gridWorldSize;
    public bool debugDraw;
    public bool shouldRunSelf = false;
    public Node[,] grid;
    private Dictionary<string, NavigationMesh> navMeshes;

    public List<Connection>[] walkableConnections;
    public List<Connection>[] flyableConnections;

    Stopwatch sw;

    private Vector2 mousePosition;

    private void Start()
    {
        if (shouldRunSelf)
        {
            Init();
        }
    }

    public void Init()
    {
        sw = new Stopwatch();
        sw.Start();
        navMeshes = new Dictionary<string, NavigationMesh>();

        PathfindingAgent[] agentsInScene = GetAgentsInScene();

        GetNavigationMeshes(agentsInScene);

        walkableConnections = new List<Connection>[gridWorldSize.x * gridWorldSize.y];
        for (int i = 0; i < walkableConnections.Length; i++)
        {
            walkableConnections[i] = new List<Connection>();
        }

        flyableConnections = new List<Connection>[gridWorldSize.x * gridWorldSize.y];
        for (int i = 0; i < flyableConnections.Length; i++)
        {
            flyableConnections[i] = new List<Connection>();
        }

        CreateGrid();
        Debug.Log("Grid built: " + sw.ElapsedMilliseconds + "ms");
        CreateConnections();
        Debug.Log("Connections made: " + sw.ElapsedMilliseconds + "ms");
        sw.Stop();
        Debug.Log("Grid finished: " + sw.ElapsedMilliseconds + "ms");
    }

    private void CreateGrid()
    {
        // Position the grid so it lines up with tiles
        if (gridWorldSize.x % 2 == 0)
        {
            transform.position = new Vector3(Mathf.Round(transform.position.x), transform.position.y, transform.position.z);
        }
        else
        {
            transform.position = new Vector3(Mathf.Round(transform.position.x) + 0.5f, transform.position.y, transform.position.z);
        }

        if (gridWorldSize.y % 2 == 0)
        {
            transform.position = new Vector3(transform.position.x, Mathf.Round(transform.position.y), transform.position.z);
        }
        else
        {
            transform.position = new Vector3(transform.position.x, Mathf.Round(transform.position.y) + 0.5f, transform.position.z);
        }

        grid = new Node[gridWorldSize.x, gridWorldSize.y];
        Vector2 worldBottomLeft = new Vector2(transform.position.x, transform.position.y) - Vector2.right * gridWorldSize.x / 2.0f - Vector2.up * gridWorldSize.y / 2.0f;
        worldBottomLeft.x += 0.5f;
        worldBottomLeft.y += 0.5f;

        for (int y = 0; y < gridWorldSize.y; y++)
        {
            for (int x = 0; x < gridWorldSize.x; x++)
            {
                Vector2 worldPoint = worldBottomLeft + Vector2.right * x + Vector2.up * y;
                if (CheckIfTileIsValidGround(worldPoint))
                {
                    Node.NodeType nodeType = Node.NodeType.Walkable;
                    grid[x, y] = new Node(nodeType, worldPoint);
                }
                else
                {
                    //Node.NodeType nodeType = Node.NodeType.None;
                    //grid[x, y] = new Node(nodeType, worldPoint);
                }
            }
        }
    }

    private void CreateConnections()
    {
        for (int y = 0; y < gridWorldSize.y; y++)
        {
            for (int x = 0; x < gridWorldSize.x; x++)
            {
                // Ground connections
                GetWalkableConnections(x, y);

                // Air connections
                //GetFlyableConnections(x, y);
            }
        }

        Debug.Log("Walkable Connections made: " + sw.ElapsedMilliseconds + "ms");

        foreach (KeyValuePair<string, NavigationMesh> navMesh in navMeshes)
        {
            if (!navMesh.Value.pathfindingAgentInfo.isFlying)
            {
                // Add walkableConnections if needed
                for (int i = 0; i < walkableConnections.Length; i++)
                {
                    foreach (Connection connection in walkableConnections[i])
                    {
                        navMesh.Value.gridConnections[i].Add(connection);
                    }
                }
            }
            else
            {
                // Add flyableConnections if needed
                for (int i = 0; i < flyableConnections.Length; i++)
                {
                    foreach (Connection connection in flyableConnections[i])
                    {
                        navMesh.Value.gridConnections[i].Add(connection);
                    }
                }
            }
        }

        //StartCoroutine(GetJumpConnections(6, 8));
    }

    private void GetWalkableConnections(int x, int y)
    {
        Node targetNode = grid[x, y];

        if (targetNode == null)
        {
            return;
        }

        if (targetNode.IsWalkableNode())
        {
            Node nextNode = GetWalkableNode(targetNode, 1, 1);
            if (nextNode != null)
            {
                walkableConnections[y * gridWorldSize.x + x].Add(new Connection(targetNode, nextNode, Connection.ConnectionType.Walk, 1.0f));
                Vector2Int nextNodeIndex = GetNodeIndexInGrid(nextNode);
                walkableConnections[nextNodeIndex.y * gridWorldSize.x + nextNodeIndex.x].Add(new Connection(nextNode, targetNode, Connection.ConnectionType.Walk, 1.0f));
            }

            nextNode = GetWalkableNode(targetNode, 1, 0);
            if (nextNode != null)
            {
                walkableConnections[y * gridWorldSize.x + x].Add(new Connection(targetNode, nextNode, Connection.ConnectionType.Walk, 1.0f));
                Vector2Int nextNodeIndex = GetNodeIndexInGrid(nextNode);
                walkableConnections[nextNodeIndex.y * gridWorldSize.x + nextNodeIndex.x].Add(new Connection(nextNode, targetNode, Connection.ConnectionType.Walk, 1.0f));
            }

            nextNode = GetWalkableNode(targetNode, 1, -1);
            if (nextNode != null)
            {
                walkableConnections[y * gridWorldSize.x + x].Add(new Connection(targetNode, nextNode, Connection.ConnectionType.Walk, 1.0f));
                Vector2Int nextNodeIndex = GetNodeIndexInGrid(nextNode);
                walkableConnections[nextNodeIndex.y * gridWorldSize.x + nextNodeIndex.x].Add(new Connection(nextNode, targetNode, Connection.ConnectionType.Walk, 1.0f));
            }

            nextNode = GetWalkableNode(targetNode, 0, -1);
            if (nextNode != null)
            {
                walkableConnections[y * gridWorldSize.x + x].Add(new Connection(targetNode, nextNode, Connection.ConnectionType.Walk, 1.0f));
                Vector2Int nextNodeIndex = GetNodeIndexInGrid(nextNode);
                walkableConnections[nextNodeIndex.y * gridWorldSize.x + nextNodeIndex.x].Add(new Connection(nextNode, targetNode, Connection.ConnectionType.Walk, 1.0f));
            }
        }
    }

    private void GetFlyableConnections(int x, int y)
    {
        Node targetNode = grid[x, y];

        if (targetNode.nodeType != Node.NodeType.None)
        {
            for (int i = -1; i <= 1; i++)
            {
                for (int j = -1; j <= 1; j++)
                {
                    if (i == 0 && j == 0)
                    {
                        continue;
                    }

                    if (x + i < 0 || x + i >= gridWorldSize.x || y + j < 0 || y + j >= gridWorldSize.y)
                    {
                        continue;
                    }

                    Node nextNode = grid[x+i, y+j];

                    if (nextNode.nodeType != Node.NodeType.None)
                    {
                        flyableConnections[y * gridWorldSize.x + x].Add(new Connection(targetNode, nextNode, Connection.ConnectionType.Fly, Mathf.Abs(i) + Mathf.Abs(j) == 2 ? 1.4f : 1.0f)); ;
                    }
                }
            }
        }
    }

    private Node GetWalkableNode(Node node, int horizontalShift, int verticalShift)
    {
        Node nextNode = GetNodeFromWorldPosition(new Vector2(node.worldPosition.x + horizontalShift, node.worldPosition.y + verticalShift));
            
        if (nextNode != null && nextNode.IsWalkableNode())
        {
            return nextNode;
        }

        return null;
    }

    public Vector2Int GetNodeIndexInGrid(Node node)
    {
        Vector2 worldPos = node.worldPosition;
        float posX = ((worldPos.x - transform.position.x) + gridWorldSize.x / 2.0f);
        float posY = ((worldPos.y - transform.position.y) + gridWorldSize.y / 2.0f);

        int x = Mathf.FloorToInt(posX);
        int y = Mathf.FloorToInt(posY);
        x = Mathf.Clamp(x, 0, gridWorldSize.x - 1);
        y = Mathf.Clamp(y, 0, gridWorldSize.y - 1);

        return new Vector2Int(x, y);
    }

    public Node GetNodeFromWorldPosition(Vector2 worldPos)
    {
        float posX = ((worldPos.x - transform.position.x) + gridWorldSize.x / 2.0f);
        float posY = ((worldPos.y - transform.position.y) + gridWorldSize.y / 2.0f);

        int x = Mathf.FloorToInt(posX);
        int y = Mathf.FloorToInt(posY);
        x = Mathf.Clamp(x, 0, gridWorldSize.x - 1);
        y = Mathf.Clamp(y, 0, gridWorldSize.y - 1);
        return grid[x, y];
    }

    public List<Connection> path;
    private void OnDrawGizmos()
    {
        if (debugDraw)
        {
            Gizmos.DrawWireCube(transform.position, new Vector3(gridWorldSize.x, gridWorldSize.y, 1));

            if (grid != null && gridDebugDraw)
            {
                Node mouseNode = GetNodeFromWorldPosition(mousePosition);
                foreach (Node n in grid)
                {
                    if (n == null)
                    {
                        continue;
                    }

                    switch (n.nodeType)
                    {
                        case Node.NodeType.None:
                            Gizmos.color = Color.gray;
                            break;
                        case Node.NodeType.Walkable:
                            Gizmos.color = Color.green;
                            break;
                        case Node.NodeType.Air:
                            Gizmos.color = Color.blue;
                            break;
                    }

                    if (mouseNode == n)
                    {
                        Gizmos.color = Color.cyan;
                    }
                    Gizmos.DrawSphere(n.worldPosition, 0.2f);
                }

                for (int i = 0; i < walkableConnections.Length; i++)
                {
                    foreach (Connection connection in walkableConnections[i])
                    {
                        switch (connection.connectionType)
                        {
                            case Connection.ConnectionType.Walk:
                                Gizmos.color = Color.green;
                                break;
                            case Connection.ConnectionType.Fly:
                                Gizmos.color = Color.blue;
                                break;
                        }
                        Gizmos.DrawLine(connection.fromNode.worldPosition, connection.toNode.worldPosition);
                    }
                }

                for (int i = 0; i < walkableConnections.Length; i++)
                {
                    foreach (Connection connection in walkableConnections[i])
                    {
                        if (mouseNode == connection.fromNode)
                        {
                            Gizmos.color = Color.cyan;
                            Gizmos.DrawLine(connection.fromNode.worldPosition, connection.toNode.worldPosition);
                        }
                    }
                }
            }
        }
    }

    private Collider2D GetTileCollider(Vector2 worldPoint)
    {
        Collider2D collider = Physics2D.OverlapCircle(worldPoint, 0.1f, collisionMask);
        if (collider)
        {
            return collider;
        }

        return null;
    }

    private bool CheckIfTileIsValidGround(Vector2 worldPoint)
    {
        return IsColliderValidGround(GetTileCollider(worldPoint));
    }

    private bool IsColliderValidGround(Collider2D collider)
    {
        if (!collider)
        {
            return true;
        }

        return false;
    }

    public List<Connection> GetConnections(Node node, string navMeshID)
    {
        Vector2Int nodeIndex = GetNodeIndexInGrid(node);
        return navMeshes[navMeshID].gridConnections[nodeIndex.y * gridWorldSize.x + nodeIndex.x];
    }

    public bool IsFlyingNavMesh(string navMeshID)
    {
        return navMeshes[navMeshID].pathfindingAgentInfo.isFlying;
    }

    // Do a sort of breath first/flood fill kind of search
    public Node FindNearestWalkableNode(Node targetNode, Node currentNode)
    {
        if (targetNode.IsWalkableNode())
        {
            return targetNode;
        }

        Vector2Int targetNodeIndex = GetNodeIndexInGrid(targetNode);

        Queue<Vector2Int> searchQueue = new Queue<Vector2Int>();
        HashSet<Vector2Int> visitedNodes = new HashSet<Vector2Int>();

        searchQueue.Enqueue(targetNodeIndex);

        // We dont want to stop the search as soon as we find a node, we want to continue checking the other node to see if one is closer
        bool nodeFound = false;
        Node closestNode = null;
        float shortestDistanceToCurrent = 10000.0f;
        while (searchQueue.Count > 0)
        {
            Vector2Int searchNode = searchQueue.Dequeue();

            if (searchNode.x < 0 || searchNode.x >= gridWorldSize.x || searchNode.y < 0 || searchNode.y >= gridWorldSize.y)
            {
                continue;
            }

            if (visitedNodes.Contains(searchNode))
            {
                continue;
            }

            bool isWalkableNode = grid[searchNode.x, searchNode.y].IsWalkableNode();

            if (isWalkableNode)
            {
                if (!nodeFound)
                {
                    nodeFound = true;
                }

                float distanceToCurrent = Vector2.Distance(currentNode.worldPosition, grid[searchNode.x, searchNode.y].worldPosition);

                if (distanceToCurrent < shortestDistanceToCurrent)
                {
                    closestNode = grid[searchNode.x, searchNode.y];
                    shortestDistanceToCurrent = distanceToCurrent;
                }
            }

            if (!nodeFound)
            {
                searchQueue.Enqueue(new Vector2Int(searchNode.x, searchNode.y - 1));
                searchQueue.Enqueue(new Vector2Int(searchNode.x + 1, searchNode.y));
                searchQueue.Enqueue(new Vector2Int(searchNode.x - 1, searchNode.y));
                searchQueue.Enqueue(new Vector2Int(searchNode.x, searchNode.y + 1));
            }

            visitedNodes.Add(searchNode);

        }

        return closestNode;
    }

    private void GetNavigationMeshes(PathfindingAgent[] agentsInScene)
    {
        foreach (PathfindingAgent agent in agentsInScene)
        {
            if (!navMeshes.ContainsKey(agent.pathfindingInfo.pathfindingInfoID))
            {
                NavigationMesh navMesh = new NavigationMesh(agent.pathfindingInfo, gridWorldSize);
                navMeshes.Add(agent.pathfindingInfo.pathfindingInfoID, navMesh);
            }
        }
    }

    private PathfindingAgent[] GetAgentsInScene()
    {
        PathfindingAgent[] agentsInScene = FindObjectsOfType<PathfindingAgent>();

        return agentsInScene;
    }

    public List<Connection>[] GetGridConnections(string navMeshID)
    {
        if (navMeshes != null)
        {
            if (navMeshes.ContainsKey(navMeshID))
            {
                return navMeshes[navMeshID].gridConnections;
            }
        }

        return null;
    }

    public void OnMousePosition(InputAction.CallbackContext context)
    {
        mousePosition = Camera.main.ScreenToWorldPoint(context.ReadValue<Vector2>());
    }
}
