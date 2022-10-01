using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

// Manager for pathfinding requests from agent (helps stop multiple pathfinding processes happening at once)
public class PathRequestManager : MonoBehaviour
{
    Queue<PathRequest> pathRequestQueue = new Queue<PathRequest>();
    PathRequest currentPathRequest;

    static PathRequestManager instance;
    Pathfinding pathfinding;

    bool isProcessingPath;

    private void Awake()
    {
        instance = this;
        pathfinding = GetComponent<Pathfinding>();
    }

    public static void RequestPath(PathfindingGrid grid, Vector2 pathStart, Vector2 pathEnd, string navMeshID, Action<Connection[], bool> callback)
    {
        PathRequest newRequest = new PathRequest(grid, pathStart, pathEnd, navMeshID, callback);
        instance.pathRequestQueue.Enqueue(newRequest);
        instance.TryProcessNext();
    }

    void TryProcessNext()
    {
        if (!isProcessingPath && pathRequestQueue.Count > 0)
        {
            currentPathRequest = pathRequestQueue.Dequeue();
            isProcessingPath = true;
            pathfinding.StartFindPath(currentPathRequest.pathGrid, currentPathRequest.pathStart, currentPathRequest.pathEnd, currentPathRequest.navMeshID);
        }
    }

    public void FinishedProcessingPath(Connection[] path, bool success)
    {
        currentPathRequest.callback(path, success);
        isProcessingPath = false;
        instance.TryProcessNext();
    }

    struct PathRequest
    {
        public PathfindingGrid pathGrid;
        public Vector2 pathStart;
        public Vector2 pathEnd;
        public string navMeshID;
        public Action<Connection[], bool> callback;

        public PathRequest(PathfindingGrid _grid, Vector2 _start, Vector2 _end, string _navMeshID, Action<Connection[], bool> _callback)
        {
            pathGrid = _grid;
            pathStart = _start;
            pathEnd = _end;
            navMeshID = _navMeshID;
            callback = _callback;
        }
    }

}
