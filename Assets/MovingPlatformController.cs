using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingPlatformController : MonoBehaviour
{
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
        for (int i = 0; i < slidingMovement.Length; i++) {
            slidingMovement[i].SetPlatformActivated(true);
        }
    }
}
