using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;

public class EndPoint : MonoBehaviour
{
    [SerializeField] public CinemachineVirtualCamera vrcam;
    private PatrolPath patrolPath;
    private GameObject player;

    private bool activated = false;
    private int pathIndex = 0;

    private void Start()
    {
        patrolPath = GetComponentInChildren<PatrolPath>();
    }

    void Update()
    {
        if (activated)
        {

            if (pathIndex != patrolPath.GetPathLength())
            {
                player.transform.rotation = Quaternion.Slerp(player.transform.rotation, transform.rotation, Time.deltaTime * 3f);

                Vector3 target = patrolPath.GetPositionOfPathNode(pathIndex);
                player.transform.position = Vector3.Lerp(player.transform.position, target, 3f * Time.deltaTime);
                if (Vector3.Distance(player.transform.position, target) < .1f)
                    pathIndex++;
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            activated = true;
            other.GetComponent<PlayerInputModelController>().OnVictory();
            UIManager.instance.GetVictoryScreen().ActiveVictoryScreen();
            GetComponentInChildren<Animator>().SetTrigger("EndScene");
            vrcam.Priority = 20;


            player = other.gameObject;
            pathIndex = GetClosestPatrolNode(player.transform.position);
        }
    }

    protected int GetClosestPatrolNode(Vector3 target)
    {
        int closestPathNodeIndex = 0;
        for (int i = 0; i < patrolPath.PathNodes.Count; i++)
        {
            float distanceToPathNode = patrolPath.GetDistanceToNode(target, i);
            if (distanceToPathNode < patrolPath.GetDistanceToNode(target, closestPathNodeIndex))
            {
                closestPathNodeIndex = i;
            }
        }
        return closestPathNodeIndex;
    }
}
