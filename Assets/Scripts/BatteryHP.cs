using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 *  Class: BatteryHP
 *  
 *  Description:
 *  This script allows player to gain HP.
 *  
 *  Author: Andrea De Seta
*/

public class BatteryHP : MonoBehaviour
{

    void OnTriggerEnter(Collider other) {
        PlayerStatistics player = other.GetComponent<PlayerStatistics>();
        //Check if the other object is a PlayerController
        if (player != null) {
            //If it is the player, increase its HP
            player.increaseHP();
            Managers.Collectables.CollectedItem(this.gameObject);
            Managers.Audio.PlayTin();
            Instantiate(Managers.Collectables.eventFX, transform.position, Quaternion.identity);

            this.gameObject.SetActive(false);
        }        
    }
}
