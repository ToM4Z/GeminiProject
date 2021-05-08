using UnityEngine;
using System.Collections;
/*
 * AttackTrigger is a script used to detect if the character's hand/foot hit other one
*/
[RequireComponent(typeof(Collider))]
public class AttackTrigger : MonoBehaviour
{
    [HideInInspector]
    public bool EnteredTrigger;
    [HideInInspector]
    public GameObject hitted;

    [SerializeField]
    public string collisionTag;


    private void Start()
    {
        if (!(collisionTag.Equals("Player") || collisionTag.Equals("Enemy")))
            throw new System.Exception("Tag not valid");
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(collisionTag))
        {
            hitted = other.gameObject;
            EnteredTrigger = true;
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag(collisionTag))
        {
            hitted = null;
            EnteredTrigger = false;
        }
    }
}
