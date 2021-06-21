using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

        //Check if the other object is a PlayerController
        if (player != null) {
            //If it is the player, increase its Bomb Counter
            Managers.Audio.PlayTin();
            player.increaseBomb();

            // disactivate object and wait respawn time
            timer = RespawnTime;
            _trigger.enabled = false;
            _renderer.enabled = false;
        }        
    }
}
