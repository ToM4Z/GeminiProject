using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cannon_Ball : MonoBehaviour
{
    // Start is called before the first frame update
    public float speed = 0.1f;
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        transform.Translate(0,0,speed * Time.deltaTime);
        Destroy (gameObject, 10);
    }

    void OnTriggerEnter(Collider other) {
        PlayerStatisticsController player = other.GetComponent<PlayerStatisticsController>();
        //Check if the other object is a PlayerController
        if (player != null) {
            //If it is the player, damage it
            player.hurt(DeathEvent.HITTED);
        }
        
        if(!other.isTrigger)
            Destroy(this.gameObject);
    }
}
