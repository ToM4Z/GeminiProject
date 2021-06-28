using System.Collections.Generic;
using UnityEngine;

/*
 *  Class: PatrolPath
 *  
 *  Description:
 *  This script describe the patrol path of the enemy.
 *  The patrol path is composed by various gameobject contained in PathNodes list.
 *  
 *  Author: Thomas Voce
*/

public class PatrolPath : MonoBehaviour
{

    [Tooltip("The Nodes making up the path")]
    public List<Transform> PathNodes = new List<Transform>();

    public float GetDistanceToNode(Vector3 origin, int destinationNodeIndex)
    {
        if (destinationNodeIndex < 0 || destinationNodeIndex >= PathNodes.Count ||
            PathNodes[destinationNodeIndex] == null)
        {
            return -1f;
        }

        return (PathNodes[destinationNodeIndex].position - origin).magnitude;
    }

    public Vector3 GetPositionOfPathNode(int nodeIndex)
    {
        if (nodeIndex < 0 || nodeIndex >= PathNodes.Count || PathNodes[nodeIndex] == null)
        {
            return Vector3.zero;
        }

        return PathNodes[nodeIndex].position;
    }

    public Quaternion GetRotationOfPathNode(int nodeIndex)
    {
        if (nodeIndex < 0 || nodeIndex >= PathNodes.Count || PathNodes[nodeIndex] == null)
        {
            return Quaternion.identity;
        }

        return PathNodes[nodeIndex].rotation;
    }

    public int GetPathLength() { return PathNodes.Count; }

    // draw patrol path in editor view
    void OnDrawGizmos()
    {
        Gizmos.color = Color.cyan;
        for (int i = 0; i < PathNodes.Count; i++)
        {
            int nextIndex = i + 1;
            if (nextIndex >= PathNodes.Count)
            {
                nextIndex -= PathNodes.Count;
            }

            Gizmos.DrawLine(PathNodes[i].position, PathNodes[nextIndex].position);
            Gizmos.DrawSphere(PathNodes[i].position, 0.1f);
        }
    }
}