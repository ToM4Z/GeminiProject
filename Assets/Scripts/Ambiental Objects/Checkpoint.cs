using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 *  Class: Checkpoint
 *  
 *  Description:
 *  This script enable player checkpoint
 *  
 *  Author: Thomas Voce
*/

public class Checkpoint : MonoBehaviour
{
    // when player trigger this checkpoint, i change material with this
    [SerializeField] private Material activatedMAT;
    private Renderer myRenderer;
    private bool activated = false;

    // this ID is needed to keep track of the last checkpoint activated
    // In the scene, these id must start by 1; 0 is the initial position.
    [SerializeField] int CheckPointID;

    private void Start()
    {
        myRenderer = GetComponentInChildren<Renderer>();
    }

    // when checkpoint is activated,
    // i instantiate a particle
    // I say to hud to start checkpoint hud animation
    // and finally set this position as new respawn
    private void ActivateCheckPoint()
    {
        activated = true;
        UIManager.instance.GetHUD().ActivateCheckpointImage();
        Vector3 pos = transform.position;
        pos.y += 1;
        Instantiate(Managers.Collectables.eventFX, pos, Quaternion.identity);

        Managers.Audio.PlayTin();
        Managers.Respawn.setRespawn(CheckPointID, transform.position, transform.rotation);
    }

    private void OnTriggerEnter(Collider other)
    {
        if(!activated && other.gameObject.CompareTag("Player"))
        {
            myRenderer.material = activatedMAT;

            ActivateCheckPoint();
        }
    }
}
