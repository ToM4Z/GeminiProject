using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gear : MonoBehaviour
{
    public float speedSpin = 1.0f;
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        transform.Rotate(0,speedSpin,0);
    }

    void OnTriggerEnter(Collider other) {
        PlayerStatistics player = other.GetComponent<PlayerStatistics>();
        //Check if the other object is a PlayerController
        if (player != null) {
            //If it is the player, increase Gear Counter
            Managers.Audio.PlayTin();
            Managers.Collectables.CollectedItem(this.gameObject);
            player.increaseNormalGear();
            //Destroy(this.gameObject);
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
