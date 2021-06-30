using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

/*
 *  Class: ActivateVirtualCamera
 *  
 *  Description:
 *  When player is inside this trigger, I change the main camera with another, this is useful
 *  in little area where the main camera is not adapt to rotate.
 *  So, in that cases, I change main camera with a camera that show better the area where player is.
 *  This change is performed by setting an higher priority on this camera.
 *  Moreover I use ActivateRotateToDirection so player is free to rotate in every direction
 *  
 *  Author: Thomas Voce
*/

public class ActivateVirtualCamera : MonoBehaviour
{
    [SerializeField] public CinemachineVirtualCamera vrcam;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            other.GetComponent<PlayerController>().ActivateRotateToDirection(true);
            vrcam.Priority = 11;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            other.GetComponent<PlayerController>().ActivateRotateToDirection(false);
            vrcam.Priority = 0;
        }
    }

}
