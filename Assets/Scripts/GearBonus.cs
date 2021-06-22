using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GearBonus : MonoBehaviour
{
    public float time = 10f;

    void Start()
    {
        Destroy(gameObject, time);
    }


    void OnTriggerEnter(Collider other) {
        PlayerStatistics player = other.GetComponent<PlayerStatistics>();
        //Check if the other object is a PlayerController
        if (player != null) {
            //If it is the player, increas its Bonus Gear Counter
            player.increaseBonusGear();
            Managers.Audio.PlayTin();
            Destroy(this.gameObject);
        }
    }

    public void setTime(float t){
        time = t;
    }
}
