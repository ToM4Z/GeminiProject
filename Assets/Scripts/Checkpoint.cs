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
        UIManager.instance.GetHUD().ActivateCheckpointImage();
        Vector3 pos = transform.position;
        pos.y += 1;
        Instantiate(Managers.Collectables.eventFX, pos, Quaternion.identity);

        Managers.Audio.PlayTin();
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
