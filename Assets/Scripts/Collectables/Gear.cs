using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/*
 *  Class: Gear
 *  
 *  Description:
 *  This script handles the Gear behaviour.
 *  
 *  Author: Andrea De Seta, Thomas Voce, Gianfranco Sapia
*/
public class Gear : MonoBehaviour
{

    void OnTriggerEnter(Collider other) {
        PlayerStatistics player = other.GetComponent<PlayerStatistics>();
        //Check if the other object is a PlayerController (Andrea)
        if (player != null) {
            //If it is the player, increase Gear Counter (Andrea)
            Managers.Audio.PlayTin();
            Instantiate(Managers.Collectables.eventFX, transform.position, Quaternion.identity);

            // I say to collectable manager that this gear was collected (Gianfranco)
            Managers.Collectables.CollectedItem(this.gameObject);
            player.increaseNormalGear();
            this.gameObject.SetActive(false);
        }
    }

    // when enemy drop a gear, i use gravity to make a little animation towards up (Thomas)
    public void ActivateFallDown()
    {
        GetComponent<Rigidbody>().isKinematic = false;
        GetComponent<Rigidbody>().useGravity = true;
        GetComponent<Rigidbody>().AddExplosionForce(5f, transform.position, 4f, 1f, ForceMode.Impulse);

        StartCoroutine(StopGravity());
    }

    // after 1 second, I reset the rigidbody to kinematic (Thomas)
    private IEnumerator StopGravity()
    {
        yield return new WaitForSeconds(1f);
        GetComponent<Rigidbody>().isKinematic = true;
        GetComponent<Rigidbody>().useGravity = false;
    }
}
