using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingPlatformController : MonoBehaviour
{
    //This class is used to activate all the slider platform that are child to the controller
    public SlidingMovement[] slidingMovement;
    void Start()
    {
        slidingMovement = GetComponentsInChildren<SlidingMovement>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void ActivatePlatforms(){

        //Looping on all childs, I activate them
        for (int i = 0; i < slidingMovement.Length; i++) {
            slidingMovement[i].SetPlatformActivated(true);
        }
    }
}
