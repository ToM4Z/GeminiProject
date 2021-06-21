using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PoolBehaviour : MonoBehaviour
{
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {

    }

    private void OnTriggerEnter(Collider collision) {
        if(collision.gameObject.tag == "Player") {
            Debug.Log("SONO ENTRATO NELLA POZZA DI VELENO");
            collision.GetComponent<PlayerStatistics>().hurt(DeathEvent.HITTED,true);
        }
    }
}
