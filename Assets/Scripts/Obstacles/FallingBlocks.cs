using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 *  Class: FallingBlocks
 *  
 *  Description:
 *  Script to handle this obstacle that can be triggered every X second or when you walk under it   
 *  
 *  Author: Gianfranco Sapia
*/
public class FallingBlocks : MonoBehaviour
{
    // Start is called before the first frame update
    private Vector3 startPos;
    private Vector3 endPos;
    private float startTime;
    private float durationTime;
    private float timePassed;
    private float deltaFall; //how much time must pass between a fall
    private bool triggered;
    private bool descending;
    public float speed;
    public bool isTimed;
    [SerializeField] private BoxCollider FallingBoxCollider; //falling 
    AudioSource _audio;

    //the ray in the start is used to calculate how much the block has to travel
    void Start()
    {
        this.startPos = transform.position;
        RaycastHit hit;
        if(Physics.Raycast(transform.position,Vector3.down, out hit)) {
            this.endPos = new Vector3(transform.position.x,transform.position.y - hit.distance, transform.position.z);
            this.durationTime = 1f * hit.distance;
        }
        this.startTime = 0;
        this.triggered = false;
        this.descending = true;
        if(isTimed) {
            deltaFall = 5.0f;
        }
        _audio = this.GetComponent<AudioSource>();
    }

    //the obstacle is handled by LerpBlocks function every deltaFall seconds, if timed, or when you step under it 
    void Update() {
        if (triggered) {
            LerpBlocks();
        } else if(isTimed) { 
            if(timePassed >= deltaFall) {
                LerpBlocks();
            }
            timePassed += Time.deltaTime;
        }
    }


    /*
    * If the block is able to descend, it will lerp to the endPos and it will enable the hitbox in order to hit the player is he is below it.
    * The block will go back in its original position, but slowly 
    */
    private void LerpBlocks() {
        if(descending) {
                FallingBoxCollider.enabled = true;
                transform.position = Vector3.Lerp(startPos, endPos, startTime/durationTime);
                startTime += Time.deltaTime * speed;
        } else {
            FallingBoxCollider.enabled = false;
            transform.position = Vector3.Lerp(endPos, startPos, startTime/durationTime);
            startTime += Time.deltaTime * (speed/2);
        }
        if(transform.position == endPos && descending) {
            descending = false;
            startTime = 0;
            _audio.Play();
        }
        if(transform.position == startPos && !descending) {
            triggered = false;
            descending = true;
            startTime = 0;
            timePassed = 0;
        }
    }

    //If the mode of the block is not isTimed then the block now is able to descend 
    private void OnTriggerEnter(Collider coll) {
        if(transform.position == startPos && !isTimed) {
            triggered = true;
            descending = true;
        }
    }
}
