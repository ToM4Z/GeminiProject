using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/*
 *  Class: Moving Platform Controller.
 *  
 *  Description:
 *  This script handles the Controller of Moving Platform.
 *  
 *  Author: Andrea De Seta
*/
public class MovingPlatformController : MonoBehaviour
{
    //This class is used to activate all the slider platform that are child to the controller
    public SlidingMovement[] slidingMovement;
    void Start()
    {
        slidingMovement = GetComponentsInChildren<SlidingMovement>();
    }
    public void ActivatePlatforms(){

        //Looping on all childs, I activate them
        for (int i = 0; i < slidingMovement.Length; i++) {
            slidingMovement[i].SetPlatformActivated(true);
        }
    }
}
