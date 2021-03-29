using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Coin : MonoBehaviour
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
        PlayerController player = other.GetComponent<PlayerController>();
        //Check if the other object is a PlayerCharacter.
        if (player != null) {
            player.increaseGoldCoins();
        }

        Destroy(this.gameObject);
        
    }
}
