using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
    [SerializeField] private BoxCollider FallingBoxCollider;
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
    }

    // Update is called once per frame
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
        }
        if(transform.position == startPos && !descending) {
            triggered = false;
            descending = true;
            startTime = 0;
            timePassed = 0;
        }
    }
    private void OnTriggerEnter(Collider coll) {
        if(transform.position == startPos && !isTimed) {
            triggered = true;
            descending = true;
        }
    }
}
