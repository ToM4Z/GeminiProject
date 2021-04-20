using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlatformActivatorButton : MonoBehaviour
{
    public Animation anim;
    public bool firstTime = true;
    public MovingPlatformController movingPlatformController;
    void Start()
    {
        anim = GetComponentInChildren<Animation>();
        GameObject goParent = transform.parent.gameObject;
        movingPlatformController = goParent.GetComponentInChildren<MovingPlatformController>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnTriggerEnter(Collider other) {
        if(firstTime){
            PlayerController player = other.GetComponent<PlayerController>();
            if (player != null) {
                anim.Play("PlatformActivatorButton_Enter 1");
                movingPlatformController.ActivatePlatforms();
            }
            firstTime = false;
        }
        
    }
}
