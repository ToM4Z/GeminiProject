using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NewPlatform : MonoBehaviour
{
    private Transform playerParent;
    private Transform player;

    Rigidbody m_Rigidbody;
    public float m_Speed = 1f;
    CharacterController cc;

    void Start()
    {
        player = PlayerStatisticsController.instance.transform;
        playerParent = player.parent;
        m_Rigidbody = GetComponent<Rigidbody>();
    }

    private void FixedUpdate()
    {
        m_Rigidbody.MovePosition(transform.position + transform.worldToLocalMatrix.MultiplyVector(transform.forward) * m_Speed * Time.deltaTime);
    }


    void OnTriggerEnter(Collider coll)
    {
        if (coll.gameObject.tag == "Player")
        {
            cc = coll.GetComponent<CharacterController>();
        }
    }
    private void OnTriggerStay(Collider other)
    {

        if (other.gameObject.tag == "Player")
        {
            cc.Move(m_Rigidbody.velocity * Time.deltaTime);
        }
    }


    //// Update is called once per frame
    //void Update()
    //{
    //    transform.Translate(transform.worldToLocalMatrix.MultiplyVector(transform.forward) * 1f * Time.deltaTime);
    //}


    //void OnTriggerEnter(Collider coll)
    //{
    //    print("enter. " + coll.gameObject.name);
    //    if (coll.gameObject.tag == "Player")
    //    {
    //        player.parent = this.transform;
    //    }
    //}

    //void OnTriggerExit(Collider coll)
    //{
    //    print("exit: " + coll.gameObject.name);
    //    if (coll.gameObject.tag == "Player")
    //    {
    //        player.transform.parent = playerParent;
    //    }
    //}
    //private void  OnTriggerStay(Collider other)
    //{

    //    if (other.gameObject.tag == "Player")
    //    {
    //        print("setting player pos");
    //        other.transform.position = transform.position;
    //    }
    //}

}
