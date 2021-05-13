using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Collectable_Bomb : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnTriggerEnter(Collider other) {
        PlayerStatisticsController player = other.GetComponent<PlayerStatisticsController>();
        //Check if the other object is a PlayerController
        if (player != null) {
            //If it is the player, increase its Bomb Counter
            //player.increaseBomb();
        }

        Destroy(this.gameObject);
        
    }
}
