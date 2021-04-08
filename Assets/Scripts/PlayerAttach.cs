using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAttach : MonoBehaviour
{
    public GameObject Player;
    void OnTriggerEnter(Collider coll) {
        if(coll.gameObject == Player) {
            Debug.Log("Player sulla piattaforma");
            Player.transform.parent = transform;
        }
    }

    void OnTriggerExit(Collider coll) {
        if(coll.gameObject == Player) {
            Debug.Log("Player fuori piattaforma");
            Player.transform.parent = null;
        }
    }
}
