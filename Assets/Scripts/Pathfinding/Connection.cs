using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Pathfinder connections between nodes
public class Connection
{
    public enum ConnectionType { Walk, Fly}
    public float cost;
    public ConnectionType connectionType;
    public Node fromNode;
    public Node toNode;
    public JumpInfo jumpInfo;

    public Connection(Node _fromNode, Node _toNode, ConnectionType _connectionType, float _cost)
    {
        fromNode = _fromNode;
        toNode = _toNode;
        connectionType = _connectionType;
        cost = _cost;
        jumpInfo = null;
    }

    public Connection(Node _fromNode, Node _toNode, ConnectionType _connectionType, float _cost, JumpInfo _jumpInfo)
    {
        fromNode = _fromNode;
        toNode = _toNode;
        connectionType = _connectionType;
        cost = _cost;
        jumpInfo = _jumpInfo;
    }
}
