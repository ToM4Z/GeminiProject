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

    private float speed = 4.0f;
    public float distance = 10.0f;
    private Vector3 start_position;
    private LeftOrRight platformDirection = LeftOrRight.Default;
    public Type type;
    public CharacterController PlayerController;
    private bool playerOnPlatform;
    private bool triggerPopUp;
    private float TimePassed;
    public MeshCollider childWithColl;
    public bool platformActivated = true;

    // Start is called before the first frame update
    void Start()
    {
        this.platformDirection = LeftOrRight.Right;
        this.start_position = new Vector3(transform.position.x, transform.position.y, transform.position.z);
        this.TimePassed = 0;
    }

    // Update is called once per frame
    void Update()
    {
        if(platformActivated) {
            if(this.type == Type.Slider) {
                transform.position += transform.forward * (int)platformDirection * speed * Time.deltaTime;
                if(playerOnPlatform) {
                    PlayerController.Move(transform.forward * (int)platformDirection * speed * Time.deltaTime);
                }
            } else if (this.type == Type.Elevator) {
                transform.position += transform.up * (int)platformDirection * speed * Time.deltaTime;
                if(playerOnPlatform) {
                    PlayerController.Move(transform.up * (int)platformDirection * speed * Time.deltaTime);
                }
            } else {
                if (triggerPopUp) {
                    this.TimePassed += Time.deltaTime;
                    this.transform.Rotate((int)platformDirection * speed * 7 * Time.deltaTime,0,0);
                }
                if(playerOnPlatform) {
                }
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
            if(Math.Abs((transform.position - start_position).magnitude) >= distance) {
                SwitchDirection();
            }
            if(transform.rotation.eulerAngles.x >= distance && transform.rotation.eulerAngles.x <= (360-distance)) {
                SwitchDirection();
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
