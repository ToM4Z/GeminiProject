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
    // Start is called before the first frame update
    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        agent.autoBraking = false;
        isActive = false;
        
    }

    // Update is called once per frame
    void Update()
    {
        if (agent.remainingDistance < 0.25f && isActive)//fov.distanceTo(GetDestinationOnPath()) <=
        {
            m_PathDestinationNodeIndex = (m_PathDestinationNodeIndex + 1); //% patrolPath.PathNodes.Count;
            agent.SetDestination(GetDestinationOnPath());
        }
    }

    public void Reset() {

    }

    protected Vector3 GetDestinationOnPath()
    {
        return patrolPath.GetPositionOfPathNode(m_PathDestinationNodeIndex);
    }

    private void OnTriggerEnter(Collider collision) {
        if(collision.gameObject.tag == "Player") {
            collision.GetComponent<PlayerStatisticsController>().hurt(DeathEvent.HITTED,true);
        }
    }
}
