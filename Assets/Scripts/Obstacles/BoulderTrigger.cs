using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoulderTrigger : MonoBehaviour
{
    public GameObject boulder;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider collision) {
        if(collision.gameObject.tag == "Player") {
            boulder.GetComponent<BoulderPath>().isActive = true;
        }
    }
}
