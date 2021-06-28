using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;

/*
 *  Class: EndPoint
 *  
 *  Description:
 *  Is the script of the end level. It handles the animation of victory.
 *  
 *  Author: Thomas Voce
*/

public class EndPoint : MonoBehaviour
{
    // The endpoint activate a camera to see better final animation
    [SerializeField] public CinemachineVirtualCamera vrcam;
    
    // during animation, the player must be moved to a fixed position 
    private PatrolPath patrolPath;
    private GameObject player;

    private bool activated = false;
    // is used to understand which position the player have to move
    private int pathIndex = 0;

    private void Start()
    {
        patrolPath = GetComponentInChildren<PatrolPath>();
    }

    // if activated, I move player towards the final position, following a path
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

    // when player enter in trigger, I start player win animation, victory screen, endpoint animation, and start to move player to final destination
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            activated = true;
            GlobalVariables.Win = true;

            other.GetComponent<PlayerController>().OnVictory();
            UIManager.instance.GetVictoryScreen().ActiveVictoryScreen();
            GetComponentInChildren<Animator>().SetTrigger("EndScene");
            vrcam.Priority = 20;

            player = other.gameObject;
            pathIndex = GetClosestPatrolNode(player.transform.position);
        }
    }

    // return the closest player patrol node 
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
