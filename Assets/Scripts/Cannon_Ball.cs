using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cannon_Ball : MonoBehaviour
{
    // Start is called before the first frame update
    public float speed = 0.1f;
    public int damage = 1;
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
        PlayerController player = other.GetComponent<PlayerController>();
        //Check if the other object is a PlayerCharacter.
        if (player != null) {
            player.Damage(damage);
        }

        Destroy(this.gameObject);
        
    }
}
