using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
            this.gameObject.SetActive(false);
        }        
    }
}
