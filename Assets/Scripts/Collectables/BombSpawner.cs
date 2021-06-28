using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 *  Class: BombSpawner
 *  
 *  Description:
 *  this script spawn bombs after few seconds
 *  
 *  Author: Thomas Voce, Andrea De Seta
*/

public class BombSpawner : MonoBehaviour
{
    private readonly float RespawnTime = 6f;
    private float timer = 0;

    private Collider _trigger;
    private Renderer _renderer;

    void Start()
    {
        _trigger = GetComponent<Collider>();
        _renderer = GetComponentInChildren<Renderer>();
    }

    // if a bomb was catched, I wait few seconds and, when timer is finished,
    // I re-enable trigger and renderer in such a way that the player can catch another bomb
    void Update()
    {
        if (!_trigger.enabled)
        {
            timer -= Time.deltaTime;

            if(timer < 0)
            {
                _trigger.enabled = true;
                _renderer.enabled = true;
            }
        }
    }

    void OnTriggerEnter(Collider other) {
        PlayerStatistics player = other.GetComponent<PlayerStatistics>();

        //Check if the other object is a PlayerController (Andrea)
        if (player != null) {
            //If it is the player, increase its Bomb Counter (Andrea)
            Managers.Audio.PlayTin();
            player.increaseBomb();

            Instantiate(Managers.Collectables.eventFX, transform.position, Quaternion.identity);

            // disactivate object and wait respawn time (Andrea)
            timer = RespawnTime;
            _trigger.enabled = false;
            _renderer.enabled = false;
        }        
    }
}
