using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GearBonus : MonoBehaviour
{
    public float speedSpin = 1.0f;
    public float time = 10f;
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        transform.Rotate(0,0,speedSpin);
        Destroy (gameObject, time);
    }

    void OnTriggerEnter(Collider other) {
        PlayerController player = other.GetComponent<PlayerController>();
        //Check if the other object is a PlayerCharacter.
        if (player != null) {
            player.increaseBonusGear();
        }

        Destroy(this.gameObject);
        
    }

    public void setTime(float t){
        time = t;
    }
}
