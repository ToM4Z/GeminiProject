using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HelixBehaviour : MonoBehaviour
{
    public enum ClockVerse {
        AntiClockWise = -1, ClockWise = 1
    }
    private float y_angle;
    private float startTime;
    private float durationTime;
    public ClockVerse clockVerse;
    private Quaternion startRot;
    private Quaternion endRot;

    // Start is called before the first frame update
    void Start()
    {
        this.y_angle = 0;
        this.startTime = 0;
        this.durationTime = 10.0f;
        startRot.eulerAngles = Vector3.zero;
        endRot.eulerAngles = new Vector3(0,(int)clockVerse*45,0);
    }

    // Update is called once per frame
    void Update()
    {
        //y_angle += Time.deltaTime;
        //transform.Rotate(0,Time.deltaTime*50,0);
        //transform.rotation = Quaternion.Euler(0,Mathf.Lerp(0.0f,360.0f,startTime/durationTime),0);
        this.transform.rotation = Quaternion.Lerp(startRot,endRot,startTime/durationTime);
        /*Debug.Log("start r:" + startRot.eulerAngles);
        Debug.Log("end r:" + endRot.eulerAngles);
        Debug.Log("trans r:" + transform.rotation.eulerAngles.y%45);
        Debug.Log("start time:" + startTime);
        Debug.Log("approx: " + Mathf.Approximately(transform.rotation.eulerAngles.y%45, 0.0f));*/
        if(Approx((transform.rotation.eulerAngles.y + 0.01f)%45, 0.0f) && startTime > 0) {
            Quaternion temp = startRot;
            startRot = endRot;
            endRot.eulerAngles = new Vector3(0,(startRot.eulerAngles.y+(int)clockVerse*45)%360,0);
            startTime = 0;
        }
        startTime += Time.deltaTime * 10;
    }

    private bool Approx(float a, float b) {
        Debug.Log("abs diff: " + Mathf.Abs(a-b));
        return Mathf.Abs(a-b) <= 0.1f;
    }
}
