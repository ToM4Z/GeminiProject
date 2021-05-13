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

        //We need the controller of the moving platforms in order to activate them
        movingPlatformController = goParent.GetComponentInChildren<MovingPlatformController>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnTriggerEnter(Collider other) {
        //It will be activated only the first time that the player touches it
        if(firstTime){
            PlayerStatisticsController player = other.GetComponent<PlayerStatisticsController>();
            if (player != null) {
                anim.Play("PlatformActivatorButton_Enter 1");
                movingPlatformController.ActivatePlatforms();
            }
            firstTime = false;
        }
        
    }
}
