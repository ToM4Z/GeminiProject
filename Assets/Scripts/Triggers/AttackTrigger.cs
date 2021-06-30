using UnityEngine;
using System.Collections;

/*
 *  Class: AttackTrigger
 *  
 *  Description:
 *  This is a script used to detect if the entity's hand/foot hit other one
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

    Collider _collider;

    private void Start()
    {
        if (!(collisionTag.Equals("Player") || collisionTag.Equals("Enemy")))
            throw new System.Exception("Tag not valid");

        _collider = GetComponent<Collider>();
    }

    // entity enable the attack trigger only when he attacks
    public void EnableTrigger()
    {
        _collider.enabled = true;
    }

    // entity disable the attack trigger when he finish to attack
    public void DisableTrigger()
    {
        _collider.enabled = false;
        hitted = null;
        EnteredTrigger = false;
    }

    // If I collide another entity with collisionTag, I register it and the entity will handles to call 'hit' method on it
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

    // on trigger exit, I reset variables
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
