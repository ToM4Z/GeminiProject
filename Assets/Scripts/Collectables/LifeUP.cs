using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 *  Class: LifeUP
 *  
 *  Description:
 *  This script handles the Coin that increase the Lif counter behaviour.
 *  
 *  Author: Andrea De Seta
*/
public class LifeUP : MonoBehaviour
{
    void OnTriggerEnter(Collider other) {
        PlayerStatistics player = other.GetComponent<PlayerStatistics>();
        //Check if the other object is a PlayerController
        if (player != null) {
            //If it is the player, increase its HP
            player.increaseLives();
            Managers.Audio.PlayTin();
            Instantiate(Managers.Collectables.eventFX, transform.position, Quaternion.identity);

            Destroy(this.gameObject);
        }        
    }
}
