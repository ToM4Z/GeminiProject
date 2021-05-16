using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Collectable_Bomb : MonoBehaviour
{
    void OnTriggerEnter(Collider other) {
        PlayerStatisticsController player = other.GetComponent<PlayerStatisticsController>();

        //Check if the other object is a PlayerController
        if (player != null) {
            //If it is the player, increase its Bomb Counter
            player.increaseBomb();

            //Destroy whole gameobject
            Destroy(transform.parent.gameObject);   
        }        
    }
}
