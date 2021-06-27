using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/*
 *  Class: Gear
 *  
 *  Description:
 *  This script handles the Gear behaviour.
 *  
 *  Author: Andrea De Seta, Thomas Voce
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

            Managers.Collectables.CollectedItem(this.gameObject);
            player.increaseNormalGear();
            this.gameObject.SetActive(false);
        }
    }

    public void ActivateFallDown()
    {
        GetComponent<Rigidbody>().isKinematic = false;
        GetComponent<Rigidbody>().useGravity = true;
        GetComponent<Rigidbody>().AddExplosionForce(5f, transform.position, 4f, 1f, ForceMode.Impulse);

        StartCoroutine(StopGravity());
    }

    private IEnumerator StopGravity()
    {
        yield return new WaitForSeconds(1f);
        GetComponent<Rigidbody>().isKinematic = true;
        GetComponent<Rigidbody>().useGravity = false;
    }
}
