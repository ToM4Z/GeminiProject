using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/*
 *  Class: Gear Bonus
 *  
 *  Description:
 *  This script handles the Gear Bonus behaviour.
 *  
 *  Author: Andrea De Seta
*/

public class GearBonus : MonoBehaviour
{
    public float time = 10f;

    void Start()
    {
        Destroy(gameObject, time);
    }


    void OnTriggerEnter(Collider other) {
        PlayerStatistics player = other.GetComponent<PlayerStatistics>();
        //Check if the other object is a PlayerController
        if (player != null) {
            //If it is the player, increas its Bonus Gear Counter
            player.increaseBonusGear();
            Managers.Audio.PlayTin();
            Instantiate(Managers.Collectables.eventFX, transform.position, Quaternion.identity);

            Destroy(this.gameObject);
        }
    }

    public void setTime(float t){
        time = t;
    }
}
