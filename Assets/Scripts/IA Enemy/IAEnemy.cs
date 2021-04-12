using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public abstract class IAEnemy : MonoBehaviour
{
    public enum Status
    {
        IDLE,
        WARNED
    }

    protected FOVDetection fov;

    protected Transform player;
    protected NavMeshAgent agent;
    protected Rigidbody rig;
    private Vector3 originPos;
    private Quaternion originRot;

    protected bool alive = true;
    protected Status status;
    public bool debug = true;
    protected bool attacking;
    protected Coroutine attackCoroutine;
    public float distanceFromPlayer = 3f;

    public bool enablePatrol = true;
    public bool enableMoveToPlayer = true;

    [Tooltip("The distance at which the enemy considers that it has reached its current path destination point")]
    public float PathReachingRadius = 0.5f;
    public float maxDistancePatrol = 6f;
    public PatrolPath patrolPath;
    int m_PathDestinationNodeIndex = 1;

    protected virtual void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
        rig = GetComponent<Rigidbody>();
        agent = GetComponent<NavMeshAgent>();
        fov = GetComponent<FOVDetection>();

        originPos = transform.position;
        originRot = transform.rotation;
    }

    void Update()
    {
        if (alive)
        {
            if (status == Status.WARNED)
                warned();
            else
                idle();
        }
    }

    protected virtual void idle()
    {
        if (enablePatrol)
        {
            UpdatePathDestination();
            SetNavDestination(GetDestinationOnPath());
        }
        else
        {
            if ((transform.position - originPos).magnitude > PathReachingRadius)
            {
                SetNavDestination(originPos);
            }
            else
            {
                transform.rotation = Quaternion.Slerp(transform.rotation, originRot, Time.deltaTime * 3f);
            }
        }
    }

    protected virtual void warned()
    {
        if (enableMoveToPlayer)
        {
            float distanceFromClosest = 0;
            GetClosestPatrolNode(out distanceFromClosest);

            if (distanceFromClosest <= maxDistancePatrol)
                agent.SetDestination(player.transform.position);
            else
                agent.ResetPath();
        }

        FacePlayer();
        attack();
    }
    protected virtual void lostwarned()
    {
        agent.stoppingDistance = 0f;
        if (enablePatrol)
            SetPathDestinationToClosestNode();

        ChangeStatus(Status.IDLE);
    }
    protected abstract void attack();

    public void die()
    {
        alive = false;
        // animazione morte
        Destroy(this.gameObject);
    }

    public void SetWarned() { 
        if(status != Status.WARNED)
        {
            agent.stoppingDistance = distanceFromPlayer ;
            ChangeStatus(Status.WARNED);
        }
    }

    protected void ChangeStatus(Status s)
    {
        if(debug)
            print(s);
        status = s;
    }

    protected void FaceTarget(Vector3 target)
    {
        Vector3 direction = (target - transform.position).normalized;
        Quaternion lookRotation = Quaternion.LookRotation(new Vector3(direction.x, 0, direction.z));
        transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * 3f);
    }

    protected void FacePlayer()
    {
        FaceTarget(player.position);
    }

    public void SetPathDestinationToClosestNode()
    {
        m_PathDestinationNodeIndex = GetClosestPatrolNode();
    }

    private int GetClosestPatrolNode()
    {
        float x = 0;
        return GetClosestPatrolNode(out x);
    }

    private int GetClosestPatrolNode(out float distanceToNode)
    {
        int closestPathNodeIndex = 0;
        distanceToNode = 0;
        for (int i = 0; i < patrolPath.PathNodes.Count; i++)
        {
            float distanceToPathNode = patrolPath.GetDistanceToNode(transform.position, i);
            if (distanceToPathNode < patrolPath.GetDistanceToNode(transform.position, closestPathNodeIndex))
            {
                closestPathNodeIndex = i;
                distanceToNode = distanceToPathNode;
            }
        }
        return closestPathNodeIndex;
    }

    public Vector3 GetDestinationOnPath()
    {
        return patrolPath.GetPositionOfPathNode(m_PathDestinationNodeIndex);
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
        // Check if reached the path destination
        if ((transform.position - GetDestinationOnPath()).magnitude <= PathReachingRadius)
        {
            // increment path destination index
            m_PathDestinationNodeIndex = m_PathDestinationNodeIndex + 1;

            m_PathDestinationNodeIndex = m_PathDestinationNodeIndex % patrolPath.PathNodes.Count;
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.grey;
        if(enablePatrol)
            Gizmos.DrawRay(transform.position, GetDestinationOnPath());
        else
            Gizmos.DrawRay(transform.position, transform.position - originPos);
    }
}
