using System.Collections;
using System.Collections.Generic;
using UnityEngine.AI;
using UnityEngine;

[RequireComponent(typeof(NavMeshAgent))]
public class BoulderPath : MonoBehaviour, IResettable
{
    protected NavMeshAgent agent;
    [SerializeField] public PatrolPath patrolPath;
    int m_PathDestinationNodeIndex = 1;
    public bool isActive;
    private Vector3 originPos;
    private Quaternion originRot;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        agent.autoBraking = false;
        isActive = false;

        originPos = transform.position;
        originRot = transform.rotation;
    }

    void Update()
    {
        if (isActive && agent.remainingDistance < 0.25f)
        {
            m_PathDestinationNodeIndex++;

            if (m_PathDestinationNodeIndex == patrolPath.GetPathLength())
            {
                isActive = false;
                gameObject.SetActive(false);
            }
            else
                agent.SetDestination(GetDestinationOnPath());
        }
    }

    public void Reset()
    {
        isActive = false;
        m_PathDestinationNodeIndex = 1;

        transform.position = originPos;
        transform.rotation = originRot;

        agent.ResetPath();
    }

    protected Vector3 GetDestinationOnPath()
    {
        return patrolPath.GetPositionOfPathNode(m_PathDestinationNodeIndex);
    }

    private void OnTriggerEnter(Collider collision) {
        if(collision.gameObject.tag == "Player") {
            collision.GetComponent<PlayerStatisticsController>().hurt(DeathEvent.MASHED,true);
        }
        else if (collision.gameObject.tag == "Enemy")
        {
            collision.GetComponent<AIEnemy>().hit();
        }
    }

    public void Active()
    {
        isActive = true;
        agent.SetDestination(GetDestinationOnPath());
    }
}
