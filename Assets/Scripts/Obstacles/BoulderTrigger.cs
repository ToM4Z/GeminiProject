using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoulderTrigger : MonoBehaviour
{
    public GameObject boulder;

    private void OnTriggerEnter(Collider collision) {
        if(collision.gameObject.tag == "Player") {
            boulder.GetComponent<BoulderPath>().isActive = true;
        }
    }
}
