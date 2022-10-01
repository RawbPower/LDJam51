using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

// Info about pathfinding agent movement info
[CreateAssetMenu()]
public class PathfindingAgentInfo : ScriptableObject
{
    public string pathfindingInfoID;
    public BoxCollider2D agentCollider;
    [HideInInspector]
    public bool isFlying = false;
    [HideInInspector]
    public bool canJump = true;
    [HideInInspector]
    public int jumpRange;
    [HideInInspector]
    public int jumpHeight;
    [HideInInspector]
    public bool canFall = true;
}

#if UNITY_EDITOR
[CustomEditor(typeof(PathfindingAgentInfo))]
public class PathfindingAgentInfoScriptEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        PathfindingAgentInfo pathfindingInfo = (PathfindingAgentInfo)target;

        pathfindingInfo.isFlying = EditorGUILayout.Toggle("Is Flying", pathfindingInfo.isFlying);

        if (!pathfindingInfo.isFlying)
        {
            pathfindingInfo.canJump = EditorGUILayout.Toggle("Can Jump", pathfindingInfo.canJump);

            if (pathfindingInfo.canJump)
            {
                pathfindingInfo.jumpRange = EditorGUILayout.IntField("Jump Range", pathfindingInfo.jumpRange);
                pathfindingInfo.jumpHeight = EditorGUILayout.IntField("Jump Height", pathfindingInfo.jumpHeight);
            }
            pathfindingInfo.canFall = EditorGUILayout.Toggle("Can Drop", pathfindingInfo.canFall);
        }
    }
}
#endif
