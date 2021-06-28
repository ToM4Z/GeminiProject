using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 *  Class: FallingBlockBehaviour
 *  
 *  Description:
 *  Script to handle the collider of the falling block  
 *  
 *  Author: Gianfranco Sapia
*/
public class FallingBlockBehaviour : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    //During the lerp from startPos to EndPos of the falling block this collider is enabled and if the player is below it will be killed (fatal damage)
    private void OnTriggerEnter(Collider collision) {
        if(collision.gameObject.tag == "Player") {
            collision.GetComponent<PlayerStatistics>().hurt(DeathEvent.MASHED,true);
        }
    }
}
