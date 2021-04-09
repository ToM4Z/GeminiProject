using System.Collections;
using System.Collections.Generic;
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
    public CharacterController Player;
    private bool moveWithPlatform;
    private bool showPlatform;

    // Start is called before the first frame update
    void Start()
    {
        this.platformDirection = LeftOrRight.Right;
        this.start_position = new Vector3(transform.position.x, transform.position.y, transform.position.z);
        this.showPlatform = true;
    }

    // Update is called once per frame
    void Update()
    {
        if(this.type == Type.Slider) {
            transform.position += transform.forward * (int)platformDirection * speed * Time.deltaTime;
            if(moveWithPlatform) {
                Player.Move(transform.forward * (int)platformDirection * speed * Time.deltaTime);
            }
        } else if (this.type == Type.Elevator) {
            transform.position += transform.up * (int)platformDirection * speed * Time.deltaTime;
            if(moveWithPlatform) {
                Player.Move(transform.up * (int)platformDirection * speed * Time.deltaTime);
            }
        } else
        {
            StartCoroutine(PopUp());
        }
        if((transform.position - start_position).magnitude >= distance) {
            SwitchDirection();
        } else if ((transform.position - start_position).magnitude <= -distance ) {
            SwitchDirection();
        }
        /*if(moveWithPlatform) {
            //Player.transform.position += transform.forward * (int)platformDirection * speed * Time.deltaTime;
            Player.Move(transform.forward * (int)platformDirection * speed * Time.deltaTime);
            //Debug.Log(Player.transform.position);
            //Debug.Log(Player.transform.position+= transform.forward * (int)platformDirection * speed * Time.deltaTime);
            //Debug.Log("-------------------------------");
        }*/
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
        Player = coll.GetComponent<CharacterController>();
        if (Player != null ){
            moveWithPlatform = true;
            Debug.Log("Player sulla piattaforma");
        }

    }

    void OnTriggerExit(Collider coll) {
        moveWithPlatform = false;
        Player = null;
        Debug.Log(Player);
    }

    private IEnumerator PopUp(){
        if(showPlatform) {
            BoxCollider[] myColliders = gameObject.GetComponents<BoxCollider>();
            yield return new WaitForSeconds(3.0f);
            this.showPlatform = false;
            foreach(BoxCollider bc in myColliders) bc.enabled = false;
            //gameObject.GetComponent<BoxCollider>().enabled = false;
            Debug.Log("Not visibile");
        } else {
            BoxCollider[] myColliders = gameObject.GetComponents<BoxCollider>();
            yield return new WaitForSeconds(3.0f);
            this.showPlatform = true;
            foreach(BoxCollider bc in myColliders) bc.enabled = true;
            //gameObject.GetComponent<BoxCollider>().enabled = true;
            Debug.Log("Now visibile");
        }
            
    }
}
