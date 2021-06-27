using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class ActivateVirtualCamera : MonoBehaviour
{
    [SerializeField] public CinemachineVirtualCamera vrcam;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            vrcam.Priority = 11;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            vrcam.Priority = 0;
        }
    }

}
