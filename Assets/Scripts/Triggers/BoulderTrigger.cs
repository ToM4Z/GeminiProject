using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 *  Class: BoulderTrigger
 *  
 *  Description:
 *  Script to handle the collider of the falling block  
 *  
 *  Author: Gianfranco Sapia
*/
public class BoulderTrigger : MonoBehaviour
{
    public GameObject boulder;

    //The boulder in the level 2 will start moving (isActive = true) when the player enter in this trigger
    private void OnTriggerEnter(Collider collision) {
        if(collision.gameObject.tag == "Player") {
            boulder.GetComponent<BoulderPath>().isActive = true;
        }
    }
}
