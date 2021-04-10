using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BatteryHP : MonoBehaviour
{
    // Start is called before the first frame update
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
        PlayerController player = other.GetComponent<PlayerController>();
        //Check if the other object is a PlayerCharacter.
        if (player != null) {
            player.increaseHP();
        }

        Destroy(this.gameObject);
        
    }
}
