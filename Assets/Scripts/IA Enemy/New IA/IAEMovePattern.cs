using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class IAEMovePattern : NIAEnemy
{

    [Tooltip("The distance at which the enemy considers that it has reached its current path destination point")]
    public float PathReachingRadius = 0.5f;

    public float maxDistancePatrol = 6f;

    int m_PathDestinationNodeIndex = 1;
    protected override void Start()
    {
        base.Start();
    }

    protected override void idle()
    {
        UpdatePathDestination();
        SetNavDestination(GetDestinationOnPath());
    }


    protected override void warned()
    {
        float distanceFromClosest = patrolPath.GetDistanceToNode(transform.position, GetClosestPatrolNode());

        if (distanceFromClosest <= maxDistancePatrol)
            agent.SetDestination(player.transform.position);
        else
            agent.ResetPath();
        FacePlayer();
        attack();
    }

    protected override void lostwarned()
    {
        agent.stoppingDistance = 0f;
        SetPathDestinationToClosestNode();
        ChangeStatus(EStatus.IDLE);
    }

    public void SetPathDestinationToClosestNode()
    {
        m_PathDestinationNodeIndex = GetClosestPatrolNode();
    }

    private int GetClosestPatrolNode()
    {
        if (IsPathValid())
        {
            int closestPathNodeIndex = 0;
            for (int i = 0; i < patrolPath.PathNodes.Count; i++)
            {
                float distanceToPathNode = patrolPath.GetDistanceToNode(transform.position, i);
                if (distanceToPathNode < patrolPath.GetDistanceToNode(transform.position, closestPathNodeIndex))
                {
                    closestPathNodeIndex = i;
                }
            }

            return closestPathNodeIndex;
        }
        else
        {
            return 0;
        }
    }


    bool IsPathValid()
    {
        return patrolPath && patrolPath.PathNodes.Count > 0;
    }

    public Vector3 GetDestinationOnPath()
    {
        if (IsPathValid())
        {
            return patrolPath.GetPositionOfPathNode(m_PathDestinationNodeIndex);
        }
        else
        {
            return transform.position;
        }
    }

    public void SetNavDestination(Vector3 destination)
    {
        if (agent)
        {
            agent.SetDestination(destination);
        }
    }

    public void UpdatePathDestination()
    {
        if (IsPathValid())
        {
            // Check if reached the path destination
            if ((transform.position - GetDestinationOnPath()).magnitude <= PathReachingRadius)
            {
                // increment path destination index
                m_PathDestinationNodeIndex = m_PathDestinationNodeIndex + 1;

                m_PathDestinationNodeIndex = m_PathDestinationNodeIndex % patrolPath.PathNodes.Count;
            }
        }
    }
}
