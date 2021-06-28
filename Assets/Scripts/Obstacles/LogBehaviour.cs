using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 *  Class: LogBehaviour
 *  
 *  Description:
 *  Script to handle the log obstacle generated from the script LogController
 *  
 *  Author: Gianfranco Sapia
*/
public class LogBehaviour : MonoBehaviour
{
    private Vector3 startPos;
    private Vector3 endPos;
    private float distance;
    private float durationTime;
    private float startTime;
    public float speed;

    public void SetPos(Vector3 startPos, Vector3 endPos) {
        this.startPos = startPos;
        this.endPos = endPos;
    }

    // Start is called before the first frame update
    void Start()
    {
        distance = Vector3.Distance(this.startPos,this.endPos);
        this.durationTime = 1.0f * distance;
        this.startTime = 0;
    }

    //The object is translated using Lerp function until it reach endPos
    void Update()
    {
        this.transform.position = Vector3.Lerp(startPos,endPos,startTime/durationTime);
        startTime += Time.deltaTime * speed;
    }

    //If the player is hitted by the log it will be hurt (only 1hp removed)
    private void OnTriggerEnter(Collider collision) {
        if(collision.gameObject.tag == "Player") {
            collision.GetComponent<PlayerStatistics>().hurt(DeathEvent.HITTED,false);
        }
    }
}
