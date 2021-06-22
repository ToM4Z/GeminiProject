using UnityEngine;
using System.Collections;

/*
 *  Class: AttackTrigger
 *  
 *  Description:
 *  This is a script used to detect if the character's hand/foot hit other one
 *  
 *  Author: Thomas Voce
*/

[RequireComponent(typeof(Collider))]
public class AttackTrigger : MonoBehaviour
{
    [HideInInspector]
    public bool EnteredTrigger;
    [HideInInspector]
    public GameObject hitted;

    [SerializeField]
    public string collisionTag = "Player";


    private void Start()
    {
        if (!(collisionTag.Equals("Player") || collisionTag.Equals("Enemy")))
            throw new System.Exception("Tag not valid");
    }

    public void EnableTrigger()
    {
        GetComponent<Collider>().enabled = true;
    }

    public void DisableTrigger()
    {
        GetComponent<Collider>().enabled = false;
        hitted = null;
        EnteredTrigger = false;
    }


    void OnTriggerEnter(Collider other)
    {
        if (other.isTrigger)
            return;

        if (other.CompareTag(collisionTag))
        {
            hitted = other.gameObject;
            EnteredTrigger = true;
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.isTrigger)
            return;

        if (other.CompareTag(collisionTag))
        {
            hitted = null;
            EnteredTrigger = false;
        }
    }
}
