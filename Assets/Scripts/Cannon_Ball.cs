using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * Authors: Andrea De Seta
 *          Thomas Voce
 */
public class Cannon_Ball : MonoBehaviour
{
    public float speed = 0.1f;
    public float maxDistance = 10;
    private Vector3 originPos;
    [SerializeField] GameObject explosionFX;

    void Start()
    {
        originPos = transform.position;
    }

    void Update()
    {
        transform.Translate(0,0,speed * Time.deltaTime);

        // if I'm over the limit distance from where I shoot, I explode
        if (Vector3.Distance(originPos, transform.position) > maxDistance)
            Explode();
    }

    void OnTriggerEnter(Collider other) {
        // if the gameobject hitted is the player, damage him
        if (other.GetComponent<PlayerStatistics>() != null)
            PlayerStatistics.instance.hurt(DeathEvent.HITTED);

        // if the gameobject hitted can be hitted, damage him
        else if (other.GetComponent<IHittable>() != null)
            other.GetComponent<IHittable>().hit();

        // if the other object isn't a trigger, explode
        if (!other.isTrigger)
            Explode();
    }

    private void Explode()
    {
        Instantiate(explosionFX, transform.position, Quaternion.identity);
        Destroy(this.gameObject);
    }
}
