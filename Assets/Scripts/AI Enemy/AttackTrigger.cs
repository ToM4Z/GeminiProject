using UnityEngine;
/*
 * AttackTrigger is a script used to detect if an attack hit the player 
*/
[RequireComponent(typeof(Collider))]
public class AttackTrigger : MonoBehaviour
{
    public bool EnteredTrigger;
    private readonly string collisionTag = "Player";

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
