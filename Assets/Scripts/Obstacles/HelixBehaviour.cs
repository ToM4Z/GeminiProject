using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 *  Class: HelixBehaviour
 *  
 *  Description:
 *  Script to handle the helix obstacle  
 *  
 *  Author: Gianfranco Sapia
*/
public class HelixBehaviour : MonoBehaviour
{
    public enum ClockVerse {
        AntiClockWise = -1, ClockWise = 1
    }
    public float speed;
    private float startTime;
    private float durationTime;
    public ClockVerse clockVerse;
    private Quaternion startRot;
    private Quaternion endRot;

    // Start is called before the first frame update
    void Start()
    {
        this.startTime = 0;
        this.durationTime = 10.0f;
        startRot.eulerAngles = transform.rotation.eulerAngles;
        endRot.eulerAngles = new Vector3(transform.rotation.eulerAngles.x,(int)clockVerse*45,transform.rotation.eulerAngles.z);
    }

    //To rotate the helix permanently using Quaternion.Lerp the end rotation point is calculated every 45 degree, otherwise strange behaviour can happen
    void Update()
    {
        this.transform.rotation = Quaternion.Lerp(startRot,endRot,startTime/durationTime);
        if(Approx((transform.rotation.eulerAngles.y + 0.01f)%45, 0.0f) && startTime > 0) {
            Quaternion temp = startRot;
            startRot = endRot;
            endRot.eulerAngles = new Vector3(transform.rotation.eulerAngles.x,(startRot.eulerAngles.y+(int)clockVerse*45)%360,transform.rotation.eulerAngles.z);
            startTime = 0;
        }
        startTime += Time.deltaTime * speed;
    }

    private bool Approx(float a, float b) {
        return Mathf.Abs(a-b) <= 0.1f;
    }

    //If the player is hitted by the log it will be hurt (only 1hp removed)
    private void OnTriggerEnter(Collider collision) {
        if(collision.gameObject.tag == "Player") {
            collision.GetComponent<PlayerStatistics>().hurt(DeathEvent.HITTED,false);
        }
    }
}
