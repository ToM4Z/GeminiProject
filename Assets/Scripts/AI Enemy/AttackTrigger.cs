using UnityEngine;
using System.Collections;
/*
 * AttackTrigger is a script used to detect if the character's hand/foot hit other one
*/
[RequireComponent(typeof(Collider))]
public class AttackTrigger : MonoBehaviour
{
    public bool EnteredTrigger;

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
            EnteredTrigger = true;
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag(collisionTag))
            EnteredTrigger = false;
    }
}
