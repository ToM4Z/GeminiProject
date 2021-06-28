using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 *  Class: PoolBehaviour
 *  
 *  Description:
 *  Script to handle the lava pool obstacle  
 *  
 *  Author: Gianfranco Sapia
*/
public class PoolBehaviour : MonoBehaviour
{
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {

    }

    /*
    * If the player step on the lava pool it will be killed immediately with the effect "Burned"
    */
    private void OnTriggerEnter(Collider collision) {
        if(collision.gameObject.tag == "Player") {
            collision.GetComponent<PlayerStatistics>().hurt(DeathEvent.BURNED,true);
        }
    }
}
