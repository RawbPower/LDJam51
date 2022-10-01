using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Pathfinder nodes
public class Node : IHeapItem<Node>
{
    public enum NodeType { None, Walkable, Air };

    public NodeType nodeType;
    public Vector2 worldPosition;
    public NodeRecord nodeRecord;
    public int platformIndex;
    int heapIndex;

    public Node(NodeType _nodeType, Vector2 _worldPos)
    {
        nodeType = _nodeType;
        worldPosition = _worldPos;
        nodeRecord = null;
        platformIndex = -1;
    }

    public int HeapIndex
    {
        get
        {
            return heapIndex;
        }
        set
        {
            heapIndex = value;
        }
    }

    public int CompareTo(Node other)
    {
        int compare = nodeRecord.estimatedTotalCost.CompareTo(other.nodeRecord.estimatedTotalCost);
        if (compare == 0)
        {
            compare = nodeRecord.costSoFar.CompareTo(other.nodeRecord.costSoFar);
        }

        return -compare;
    }

    public bool IsWalkableNode()
    {
        return (nodeType != Node.NodeType.None && nodeType != Node.NodeType.Air);
    }
}

// This class is used to keep track of the
// information we need for each node
public class NodeRecord
{
    public Connection connection;
    public float costSoFar;
    public float estimatedTotalCost;
}
