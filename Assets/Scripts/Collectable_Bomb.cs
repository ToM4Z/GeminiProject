using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Collectable_Bomb : MonoBehaviour
{
    void OnTriggerEnter(Collider other) {
        PlayerStatistics player = other.GetComponent<PlayerStatistics>();

        //Check if the other object is a PlayerController
        if (player != null) {
            //If it is the player, increase its Bomb Counter
            Managers.Audio.PlayTin();
            player.increaseBomb();

            //Destroy whole gameobject
            Destroy(transform.parent.gameObject);   
        }        
    }
}
