using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Diagnostics;
using Debug = UnityEngine.Debug;

// Pathfinding code
public class Pathfinding : MonoBehaviour
{
    //public Transform seeker, target;

    PathRequestManager requestManager;
    //PathfindingGrid grid;

    private void Awake()
    {
        requestManager = GetComponent<PathRequestManager>();
        //grid = GetComponent<PathfindingGrid>();
    }

    private void Update()
    {
    }

    public void StartFindPath(PathfindingGrid grid, Vector2 startPosition, Vector2 targetPosition, string navMeshID)
    {
        StartCoroutine(FindPath(grid, startPosition, targetPosition, navMeshID));
    }

    IEnumerator FindPath(PathfindingGrid grid, Vector2 startPosition, Vector2 targetPosition, string navMeshID)
    {
        Stopwatch sw = new Stopwatch();
        sw.Start();

        Connection[] pathConnections = new Connection[0];
        bool pathSuccess = false;

        Node startNode = grid.GetNodeFromWorldPosition(startPosition);
        Node targetNode = grid.GetNodeFromWorldPosition(targetPosition);
        Node currentNode = null;

        if ((startNode.IsWalkableNode() && targetNode.IsWalkableNode()) || grid.IsFlyingNavMesh(navMeshID))
        {
            // Initialize the record for the start node
            NodeRecord startRecord = new NodeRecord();
            startRecord.connection = null;
            startRecord.costSoFar = 0.0f;
            startRecord.estimatedTotalCost = GetHeuristic(startNode, targetNode);

            startNode.nodeRecord = startRecord;

            // Initialize the open and closed lists
            // The data structure for this will be improved soon
            Heap<Node> openSet = new Heap<Node>(grid.gridWorldSize.x * grid.gridWorldSize.y);
            //List<Node> openSet = new List<Node>();
            Dictionary<int, Node> closedSet = new Dictionary<int, Node>();
            openSet.Add(startNode);

            // Iterate through procesing each node
            while (openSet.Count > 0)
            {
                // Find the smallest element in the list using the heuristic
                currentNode = openSet.GetFirstItem();
                //currentNode = GetSmallestElement(openSet);

                // If it's the target node, then terminate
                if (currentNode == targetNode)
                {
                    break;
                }

                // Otherwise get the outgoing connections
                List<Connection> connections = grid.GetConnections(currentNode, navMeshID);

                foreach (Connection connection in connections)
                {
                    // Get the cost estimate for the end node
                    NodeRecord endNodeRecord;
                    Node endNode = connection.toNode;
                    float endNodeCost = currentNode.nodeRecord.costSoFar + connection.cost;
                    float endNodeHeuristic = 0.0f;

                    // If the node is closed we have to skip, or remove it from the closed list
                    if (closedSet.ContainsKey(GetHashCode(grid, endNode)))
                    {
                        // Find record in closed list corresponding to the end node
                        endNodeRecord = endNode.nodeRecord;

                        // If we didnt find a shorter path then skip
                        if (endNodeRecord.costSoFar <= endNodeCost)
                        {
                            continue;
                        }

                        // Otherwise remove it from the closed list
                        closedSet.Remove(GetHashCode(grid, endNode));

                        // We can use the node's old cost values to calculate its
                        // heuristic without calling the heurestic function
                        endNodeHeuristic = endNodeRecord.estimatedTotalCost - endNodeRecord.costSoFar;
                    }
                    // Skip if node is open and we've not found a better route
                    else if (openSet.Contains(endNode))
                    {
                        // Find record in closed list corresponding to the end node
                        endNodeRecord = endNode.nodeRecord;

                        // If our route is no better, then skip
                        if (endNodeRecord.costSoFar <= endNodeCost)
                        {
                            continue;
                        }

                        // We can use the node's old cost values to calculate its
                        // heuristic without calling the heurestic function
                        endNodeHeuristic = endNodeRecord.estimatedTotalCost - endNodeRecord.costSoFar;
                    }
                    // Otherwise we know we've got an unvisited node, so make a record for it
                    else
                    {
                        endNodeRecord = new NodeRecord();
                        endNode.nodeRecord = endNodeRecord;

                        // Calculate the heuristic value
                        endNodeHeuristic = GetHeuristic(endNode, targetNode);
                    }

                    // We're here if we need to update the node
                    endNodeRecord.costSoFar = endNodeCost;
                    endNodeRecord.connection = connection;
                    endNodeRecord.estimatedTotalCost = endNodeCost + endNodeHeuristic;

                    endNode.nodeRecord = endNodeRecord;

                    // Add to open list
                    if (!openSet.Contains(endNode))
                    {
                        openSet.Add(endNode);
                    }
                    else
                    {
                        openSet.UpdateItem(endNode);
                    }


                }

                // We've finished looking at the connections for the current node, so add it
                // to the closed list and remove it from the open list
                openSet.Remove(currentNode);
                closedSet.Add(GetHashCode(grid, currentNode), currentNode);
            }

            sw.Stop();
            //Debug.Log("Path found: " + sw.Elapsed);

            // We're here if we've either found the goal or there is no more nodes to search
            if (currentNode != targetNode)
            {
                // We've run out of nodes without finding the goal, so there's no solution
                Debug.Log("No path found");
            }
            else
            {
                pathSuccess = true;
            }
        }

        yield return null;
        if (pathSuccess)
        {
            pathConnections = RetracePath(grid, startNode, currentNode);
        }
        requestManager.FinishedProcessingPath(pathConnections, pathSuccess);

    }

    Connection[] RetracePath(PathfindingGrid grid, Node startNode, Node endNode)
    {
        List<Connection> path = new List<Connection>();
        Node currentNode = endNode;

        // Work back along the path, accumulating connections
        while (currentNode != startNode)
        {
            path.Add(currentNode.nodeRecord.connection);
            currentNode = currentNode.nodeRecord.connection.fromNode;
        }

        path.Reverse();
        grid.path = path;

        Connection[] pathConnections = path.ToArray();

        return pathConnections;
    }

    private int GetHashCode(PathfindingGrid grid, Node node)
    {
        Vector2Int nodeIndex = grid.GetNodeIndexInGrid(node);
        return nodeIndex.y * grid.gridWorldSize.x + nodeIndex.x;
    }

    // Simple Euclidean Heuristic
    private float GetHeuristic(Node currentNode, Node targetNode)
    {
        return Vector2.Distance(currentNode.worldPosition, targetNode.worldPosition);
    }

    private Node GetSmallestElement(List<Node> list)
    {
        Node currentNode = list[0];
        for (int i = 1; i < list.Count; i++)
        {
            if (list[i].nodeRecord.estimatedTotalCost < currentNode.nodeRecord.estimatedTotalCost || (list[i].nodeRecord.estimatedTotalCost == currentNode.nodeRecord.estimatedTotalCost && list[i].nodeRecord.costSoFar < currentNode.nodeRecord.costSoFar))
            {
                currentNode = list[i];
            }
        }

        return currentNode;
    }
}
