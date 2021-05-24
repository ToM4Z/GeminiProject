using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Checkpoint : MonoBehaviour
{
    [SerializeField] private Material activatedMAT;
    private Renderer myRenderer;
    private bool activated = false;

    private void Start()
    {
        myRenderer = GetComponentInChildren<Renderer>();
    }

    private void ActivateCheckPoint()
    {
        activated = true;

        Managers.Respawn.setRespawn(transform.position, transform.rotation);
    }

    private void OnTriggerEnter(Collider other)
    {
        if(!activated && other.gameObject.tag == "Player")
        {
            myRenderer.material = activatedMAT;

            ActivateCheckPoint();
        }
    }
}
