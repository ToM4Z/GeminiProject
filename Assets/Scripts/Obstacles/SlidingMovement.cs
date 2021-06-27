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
    private bool triggerPopUp;
    private bool once;
    private bool isScaling;
    private float TimePassed;
    public MeshCollider childWithColl;
    public bool platformActivated = false;
    private float durationTime;
    private float startTime = 0;
    private GameObject player, playerParent;
    
    // Start is called before the first frame update
    void Start()
    {
        this.player = PlayerStatistics.instance.gameObject;
        playerParent = player.transform.parent.gameObject;
        this.platformDirection = LeftOrRight.Right;
        this.start_position = new Vector3(transform.position.x, transform.position.y, transform.position.z);
        this.start_position = this.transform.position;
        this.end_position_slider = start_position + transform.forward * distance * (int)platformDirection;
        this.end_position_elevator = start_position + transform.up * distance * (int)platformDirection;
        this.start_rotation = this.transform.rotation;
        this.end_rotation = Quaternion.Euler(start_rotation.eulerAngles.x + distance, start_rotation.eulerAngles.y, start_rotation.eulerAngles.z);
        this.TimePassed = 0;
        this.durationTime = 1.0f * distance;
        this.once = true;
        this.isScaling = false;
    }

    // Update is called once per frame
    void FixedUpdate()
    {

        if(platformActivated) {
            if(this.type == Type.Slider) {
                transform.position = Vector3.Lerp(start_position, end_position_slider, startTime/durationTime);
                startTime += Time.fixedDeltaTime * speed;
            } else if (this.type == Type.Elevator) {
                transform.position = Vector3.Lerp(start_position, end_position_elevator, startTime/durationTime);
                startTime += Time.fixedDeltaTime * speed;
            } else {
                if (triggerPopUp) {
                    if (!isScaling) {
                        if (TimePassed < 2.0f) {
                            this.transform.rotation = Quaternion.Slerp(start_rotation, end_rotation, startTime/durationTime);   // ruoto da start_' a end_' rotation
                            this.startTime += Time.fixedDeltaTime * speed * 4;
                            this.TimePassed += Time.fixedDeltaTime;
                        } else if (TimePassed >= 2.0f) {
                            this.childWithColl.enabled = false;
                            this.transform.localScale = Vector3.Lerp(Vector3.one, Vector3.zero, startTime/durationTime);
                            startTime += Time.fixedDeltaTime * speed * 4;
                            if(once) {
                                StartCoroutine(PopUp());
                                once = false;
                            }
                        }
                    }
                    if(this.childWithColl.enabled && isScaling) {
                        if(this.transform.localScale == Vector3.zero) {
                            Quaternion rot = new Quaternion();
                            rot.eulerAngles = Vector3.zero;
                            this.transform.rotation = rot;
                            this.startTime = 0;
                        }
                        startTime += Time.fixedDeltaTime * speed * 4;
                        this.transform.localScale = Vector3.Lerp(this.transform.localScale, Vector3.one, startTime/durationTime);
                        if (this.transform.localScale == Vector3.one) {
                            this.triggerPopUp = false;
                            isScaling = false;
                            this.startTime = 0;
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
                    (  (platformDirection == LeftOrRight.Right && Approx(transform.rotation.eulerAngles.x, distance))  // Approximately fa la stessa cosa del check '< d+eps'
                    || (platformDirection == LeftOrRight.Left  && Approx(transform.rotation.eulerAngles.x, 360-distance)) // quando il grado va in negativo in realt� scende da 360 (es. -1 sarebbe 359)
                    )) {
                SwitchDirection();
                this.start_rotation = end_rotation;
                this.end_rotation = Quaternion.Euler((distance * (int)platformDirection), transform.rotation.eulerAngles.y, transform.rotation.eulerAngles.z);
                this.startTime = 0;
            }
        }
    }

    private bool Approx(float a, float b) {
        return Mathf.Abs(a-b) <= 0.1f;
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
        if(coll.gameObject.tag == "Player" && this.type != Type.PopUp) {
            player.transform.SetParent(this.transform);
        } else if (this.type == Type.PopUp) {
            this.triggerPopUp = true;
        }
    }

    void OnTriggerExit(Collider coll) {
        if(coll.gameObject.tag == "Player" && this.type != Type.PopUp) {
            player.transform.parent = playerParent.transform;
        } else if(this.type == Type.PopUp) {
            this.TimePassed = 0.0f;
        }
    }

    private IEnumerator Shake(){
        yield return new WaitForSeconds(3.0f);
    }

    private IEnumerator PopUp(){
        yield return new WaitForSeconds(3.0f);
        this.isScaling = true;
        this.childWithColl.enabled = true;
        this.TimePassed = 0.0f;
        this.once = true;
    }

    public void SetPlatformActivated(bool activated) {
        this.platformActivated = activated;
    }

    public bool GetPlatformActivated() {
        return this.platformActivated;
    }


}