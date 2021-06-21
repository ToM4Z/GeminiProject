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
        transform.Rotate(0,speedSpin,0);
    }

    void OnTriggerEnter(Collider other) {
        PlayerStatistics player = other.GetComponent<PlayerStatistics>();
        //Check if the other object is a PlayerController
        if (player != null) {
            //If it is the player, increase its HP
            player.increaseHP();
            Managers.Collectables.CollectedItem(this.gameObject);
            this.gameObject.SetActive(false);
        }

        //Destroy(this.gameObject);
        
    }
}
