using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public class SlidingMovement : MonoBehaviour
{
    public enum LeftOrRight {
        Left = -1, Default = 0, Right = 1
    }

    public enum Type {
        Slider = 0, Elevator = 1, PopUp = 2
    }

    private float speed = 5.0f;
    public float distance = 10.0f;
    private Vector3 start_position;
    private Quaternion start_rotation;
    private Vector3 end_position_slider;
    private Vector3 end_position_elevator;
    private Quaternion end_rotation;
    private LeftOrRight platformDirection = LeftOrRight.Default;
    public Type type;
    public CharacterController PlayerController;
    private bool playerOnPlatform;
    private bool triggerPopUp;
    private float TimePassed;
    public MeshCollider childWithColl;
    public bool platformActivated = true;

    private float durationTime;
    // Time when the movement started.
    private float startTime = 0;
    // Total distance between the markers.
    private float journeyLength;
    private float eps = 0.2f;
    bool once = true;
    // Start is called before the first frame update
    void Start()
    {
        this.platformDirection = LeftOrRight.Right;
        this.start_position = new Vector3(transform.position.x, transform.position.y, transform.position.z);
        this.start_position = this.transform.position; //new Vector3(transform.position.x, transform.position.y, transform.position.z);
        this.end_position_slider = start_position + transform.forward * distance * (int)platformDirection;
        this.end_position_elevator = start_position + transform.up * distance * (int)platformDirection;
        this.start_rotation = this.transform.rotation;
        //this.end_rotation = start_rotation * Quaternion.Euler(Vector3.right * distance);
        this.end_rotation = Quaternion.Euler(start_rotation.eulerAngles.x + distance, start_rotation.eulerAngles.y, start_rotation.eulerAngles.z);
        //this.end_rotation = transform.rotation * distance;//new Quaternion(transform.rotation.eulerAngles.x + distance, 0, 0, 0);
        this.TimePassed = 0;
        durationTime = 1.0f * distance;
        //Debug.Log("x %f, %fy, %fz: " + (start_rotation.eulerAngles.x + distance, start_rotation.eulerAngles.y, start_rotation.eulerAngles.z));
        //Debug.Log("start r : " + start_rotation);
        //Debug.Log("rot x:" + transform.rotation.eulerAngles.x);
        //Debug.Log("end r : " + end_rotation.eulerAngles);   
        //Debug.Log("fw : " + transform.forward);
    }

    /*IEnumerator SliderMovement()
    {
        float time = 0;
        
        float minimum = -distance;
        float maximum = distance;
        
        while (true)
        {
            //float speed_ = Mathf.Lerp(minimum, maximum, time);
            //transform.position = start_position * speed_;
            transform.position = Vector3.Lerp(start_position, end_position, time/durationTime);
            time += Time.deltaTime * speed;
            //Debug.Log("time: " + time + " expected time: " + durationTime);
            if(transform.position == end_position) {
                Debug.Log("end: " + end_position);
                Debug.Log("trans: " + transform.position );
                Debug.Log("CAMBIO DIREZIONE");  
                SwitchDirection();
                once=false;
                //Debug.Log("switch");
                Vector3 temp = start_position;
                start_position = end_position;
                end_position = temp + (transform.forward * distance * (int)platformDirection);
                Debug.Log("end after: " + end_position);
                Debug.Log("trans after: " + transform.position );
                time = 0;
            }
            yield return null;
        }
        Debug.Log("transform pos " + transform.position);
        //transform.position = end_position;
    }*/

    // Update is called once per frame
    void Update()
    {
        /*float distCovered = (Time.deltaTime - startTime) * speed;
        Debug.Log("Dist covered: " + distCovered);
        // Fraction of journey completed equals current distance divided by total distance.
        float fractionOfJourney = distCovered / journeyLength;*/

        if(platformActivated) {
            if(this.type == Type.Slider) {
                //transform.position += transform.forward * (int)platformDirection * speed * Time.deltaTime;
                //StartCoroutine(SliderMovement());
                transform.position = Vector3.Lerp(start_position, end_position_slider, startTime/durationTime);
                startTime += Time.deltaTime * speed;
                //transform.position = Vector3.Lerp(start_position,end_position,fractionOfJourney);
                //transform.position += transform.forward * (int)platformDirection * speed * Time.deltaTime;
                /*if(playerOnPlatform) {
                    PlayerController.Move(transform.forward * (int)platformDirection * speed * Time.deltaTime);
                }
                }*/
            } else if (this.type == Type.Elevator) {
                //transform.position += transform.up * (int)platformDirection * speed * Time.deltaTime;
                transform.position = Vector3.Lerp(start_position, end_position_elevator, startTime/durationTime);
                startTime += Time.deltaTime * speed;
                /*transform.position += transform.up * (int)platformDirection * speed * Time.deltaTime;
                if(playerOnPlatform) {
                    PlayerController.Move(transform.up * (int)platformDirection * speed * Time.deltaTime);
                }
                }*/
            } else {
                if (triggerPopUp) {
                    //this.transform.rotation = Quaternion.Lerp(start_rotation,end_rotation,(int)platformDirection * this.TimePassed);
                    //this.TimePassed += Time.deltaTime;
                    //this.transform.Rotate((int)platformDirection * speed * 7 * Time.deltaTime,0,0);
                    //if (this.platformDirection == LeftOrRight.Right) {
                    //    this.transform.rotation = Quaternion.Slerp(end_rotation, start_rotation, startTime/durationTime);
                    //} else if (this.platformDirection == LeftOrRight.Left) {
                        this.transform.rotation = Quaternion.Slerp(start_rotation, end_rotation, startTime/durationTime);   // ruoto da start_' a end_' rotation
                    //}
                    this.startTime += Time.deltaTime * speed * 5;
                    this.TimePassed += Time.deltaTime;
                    //this.transform.Rotate((int)platformDirection * speed * 7 * Time.deltaTime,0,0);
                    //Debug.Log("time passed: " + TimePassed);
                }
                //if(playerOnPlatform) {
                //}
                if(TimePassed >= 2.0f) {
                    this.childWithColl.enabled = false;
                    Vector3 scale = new Vector3(-1,-1,-1);
                    if(this.transform.localScale.x > 0.1f) {
                        this.transform.localScale += scale * speed * Time.deltaTime;
                    } else {
                        this.transform.localScale = Vector3.zero;
                    }
                    StartCoroutine(PopUp());
                }
                if(this.childWithColl.enabled) {
                    if(this.transform.localScale.x != 1.0f) {    
                        Vector3 scale = new Vector3(1,1,1);
                        if(this.transform.localScale.x < 0.9f) {
                            Quaternion rot = new Quaternion();
                            rot.eulerAngles = new Vector3(0,this.transform.rotation.eulerAngles.y,0);
                            this.transform.rotation = rot;
                            this.transform.localScale += scale * speed * Time.deltaTime;
                            this.triggerPopUp = false;
                        }
                        else if(this.transform.localScale.x > 1.0f) {
                            this.transform.localScale = scale;
                        }
                    } 
                }
            }
            if(end_position_slider == transform.position && this.type == Type.Slider) {
                SwitchDirection();
                Vector3 temp = start_position;
                Vector3 deltaVec = end_position_slider-start_position;
                start_position = end_position_slider;
                end_position_slider = end_position_slider + deltaVec * (int)platformDirection;
                startTime = 0;
            } else if (end_position_elevator == transform.position && this.type == Type.Elevator) {
                SwitchDirection();
                Vector3 temp = start_position;
                Vector3 deltaVec = end_position_elevator-start_position;
                start_position = end_position_elevator;
                end_position_elevator = end_position_elevator + deltaVec * (int)platformDirection;
                startTime = 0;
            }
            else if(this.type == Type.PopUp && 
                    (  (platformDirection == LeftOrRight.Right && Mathf.Approximately(transform.rotation.eulerAngles.x, distance))  // Approximately fa la stessa cosa del check '< d+eps'
                    || (platformDirection == LeftOrRight.Left  && Mathf.Approximately(transform.rotation.eulerAngles.x, 360-distance)) // quando il grado va in negativo in realtà scende da 360 (es. -1 sarebbe 359)
                    )) {
                //if( (transform.rotation.eulerAngles.x >= (distance-eps) && platformDirection == LeftOrRight.Right) 
                //|| (transform.rotation.eulerAngles.x <= (-distance+eps) && platformDirection == LeftOrRight.Left)) {
                //Debug.Log("euler x: " + transform.rotation.eulerAngles.x);
                //Debug.Log("dir: " + platformDirection);
                SwitchDirection();
                //Quaternion temp = transform.rotation;
                this.start_rotation = this.transform.rotation = end_rotation;
                //this.end_rotation = temp;//Quaternion.Euler(Vector3.right * distance * (int)platformDirection);
                this.end_rotation = Quaternion.Euler((distance * (int)platformDirection), transform.rotation.eulerAngles.y, transform.rotation.eulerAngles.z);
                this.startTime = 0;
                //Debug.Log("changed dir: " + platformDirection);
                //Debug.Log("end r in switch: " + end_rotation);
                //Debug.Log("====================================");
            }
        }
    }

    private void SwitchDirection() {
        if (this.platformDirection == LeftOrRight.Right)
        {
            this.platformDirection = LeftOrRight.Left;
        } else
        {
            this.platformDirection = LeftOrRight.Right;
        }
    }

    void OnTriggerEnter(Collider coll) {
        if (PlayerController == null) {
            PlayerController = coll.GetComponent<CharacterController>();
            playerOnPlatform = true;
        }
        if (this.type == Type.PopUp) {
            this.triggerPopUp = true;
        }
    }

    void OnTriggerExit(Collider coll) {
        if (PlayerController == coll.GetComponent<CharacterController>()) {
            playerOnPlatform = false;
            PlayerController = null;
        }
        if(this.type == Type.PopUp) {
            this.TimePassed = 0.0f;
        }
    }

    private IEnumerator Shake(){
        yield return new WaitForSeconds(3.0f);
    }

    private IEnumerator PopUp(){
        yield return new WaitForSeconds(3.0f);
        this.childWithColl.enabled = true;
        this.TimePassed = 0.0f;
    }

    public void SetPlatformActivated(bool activated) {
        this.platformActivated = activated;
    }

    public bool GetPlatformActivated() {
        return this.platformActivated;
    }


}