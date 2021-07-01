using System.Collections;
using UnityEngine;
using Cinemachine;

/*
 * Class: ResetDollyTrack
 * 
 * Description:
 * This script was necessary to fix a bug in level 2.
 * The bug was: when you don't activate the first checkpoint 
 * and you die near to second checkpoint, the vcam doesn't return where player is.
 * So this script is needed to reset vcam position on dolly manually.
 * 
 * Author: Thomas Voce
 */
public class ResetDollyTrack : MonoBehaviour
{
    CinemachineVirtualCamera VCam;
    CinemachineTrackedDolly Dolly;
    public float[] positions;

    private void Start()
    {
        VCam = GetComponent<CinemachineVirtualCamera>();
        Dolly = VCam.GetCinemachineComponent<CinemachineTrackedDolly>();
    }

    public void ResetCameraPosition()
    {
        StartCoroutine(_ResetCameraPosition());
    }

    // I have to do this in a coroutine because changing directly m_PathPosition the modify not will be apply
    // So first of all, I need to unfollow the player, change position and then refollow player
    private IEnumerator _ResetCameraPosition()
    {
        Transform t = VCam.m_Follow;
        VCam.m_Follow = null;
        yield return null;
        Dolly.m_PathPosition = positions[Managers.Respawn.CheckPointID];
        yield return null;
        VCam.m_Follow = t;
    }

    private void Awake()
    {
        Messenger.AddListener(GlobalVariables.RESET, ResetCameraPosition);
    }

    private void OnDestroy()
    {
        Messenger.RemoveListener(GlobalVariables.RESET, ResetCameraPosition);
    }
}
