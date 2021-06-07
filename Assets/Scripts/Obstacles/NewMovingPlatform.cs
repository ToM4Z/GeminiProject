using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NewMovingPlatform : MonoBehaviour
{
    public enum Type { Slider, Elevator, PopUp }

    private Transform playerParent;
    private Transform player;

    public float speed = 2f;
    public float distance = 7f;
    [SerializeField] private Type type;

    public bool activated { get; set; }

    private Vector3 //startPos, 
        endPos, endPos1, endPos2;

    //private float startTime, journeyLength;

    void Start()
    {
        player = PlayerStatisticsController.instance.transform;
        playerParent = player.parent;


        //startPos = 
        endPos1 = endPos2 = transform.position;

        switch (type)
        {
            case Type.Elevator:
                endPos1 -= transform.up * distance;
                endPos2 += transform.up * distance;
                break;

            case Type.Slider:
                endPos1 -= transform.right * distance;
                endPos2 += transform.right * distance;
                break;

            case Type.PopUp:
                break;
        }
        endPos = endPos2;

        //startTime = Time.time;
        //journeyLength = Vector3.Distance(startPos, endPos);

        activated = true;
    }

    private void SetColliderOnOff(bool b)
    {
        foreach (Collider c in GetComponents<Collider>())
            c.enabled = b;
    }
    public void SetColliderOn()
    {
        SetColliderOnOff(true);
    }
    public void SetColliderOff()
    {
        SetColliderOnOff(false);
    }

    private Vector3 velocity = Vector3.zero;
    void FixedUpdate()
    {
        if (activated && (type == Type.Elevator || type == Type.Slider))
        {
            //try1
            //transform.position = Vector3.LerpUnclamped(transform.position, endPos, speed * Time.fixedDeltaTime);

            //try2
            //transform.position += transform.right * speed * Time.fixedDeltaTime;

            //try3
            //transform.Translate(transform.right * speed * Time.fixedDeltaTime);

            //try4
            //float distCovered = (Time.time - startTime) * speed;
            //float fractionOfJourney = distCovered / journeyLength;
            //transform.position = Vector3.Lerp(startPos, endPos, fractionOfJourney);

            //try5
            transform.position = Vector3.SmoothDamp(transform.position, endPos, ref velocity, .3f, 2f, Time.fixedDeltaTime);

            //if (Mathf.Approximately(Vector3.Distance(transform.position, endPos2), 0))
            if(Vector3.Distance(transform.position, endPos2) <= 0.1f)
            {
                //startPos = endPos2;
                endPos = endPos1;
                //startTime = Time.time;
                //journeyLength = Vector3.Distance(startPos, endPos);
            }
            else
            //if (Mathf.Approximately(Vector3.Distance(transform.position, endPos1), 0))
            if (Vector3.Distance(transform.position, endPos1) <= 0.1f)
            {
                //startPos = endPos1;
                endPos = endPos2;
                //startTime = Time.time;
                //journeyLength = Vector3.Distance(startPos, endPos);
            }
        }
    }

    void OnTriggerEnter(Collider coll)
    {
        if (coll.gameObject.tag == "Player")
        {
            print("enter");
            if(type == Type.PopUp)
            {
                GetComponent<Animator>().Play("Popup");
            }
            else
                coll.transform.SetParent(this.transform);
        }
    }

    void OnTriggerExit(Collider coll)
    {
        if (coll.gameObject.tag == "Player")
        {
            print("exit");
            if (type != Type.PopUp)
                coll.transform.parent = playerParent;
        }
    }

}
