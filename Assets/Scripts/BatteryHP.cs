using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BatteryHP : MonoBehaviour
{

    public float speedSpin = 1.0f;
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        transform.Rotate(0,0,speedSpin);
    }

    void OnTriggerEnter(Collider other) {
        PlayerStatisticsController player = other.GetComponent<PlayerStatisticsController>();
        //Check if the other object is a PlayerController
        if (player != null) {
            //If it is the player, increase its HP
            player.increaseHP();
        }

        Destroy(this.gameObject);
        
    }
}
