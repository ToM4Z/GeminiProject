using System.Collections;
using System.Collections.Generic;
using UnityEngine.AI;
using UnityEngine;
using PathCreation;

/*
 *  Class: BoulderPath
 *  
 *  Description:
 *  Script to handle the boulder that chase the player in the level 2  
 *  
 *  Author: Gianfranco Sapia, Thomas Voce
*/
public class BoulderPath : MonoBehaviour, IResettable
{
    private bool _isActive;
    public bool isActive
    {
        get { return _isActive; }
        set
        {
            _isActive = value;
            animator.SetBool("Move", isActive);
            if (isActive)
                _audio.Play();
            else
                _audio.Stop();
        }
    }

    private Vector3 originPos;
    private Quaternion originRot;
    public PathCreator pathCreator;
    public float speed = 7;
    float distanceTravelled = 0;
    Vector3 endpoint;
    Animator animator;
    AudioSource _audio;

    void Start()
    {
        animator = this.GetComponentInChildren<Animator>();
        _audio = this.GetComponent<AudioSource>();

        isActive = false;

        originPos = transform.position;
        originRot = transform.rotation;

        transform.position = pathCreator.path.GetPointAtDistance(0);
        endpoint = pathCreator.path.GetPoint(pathCreator.path.NumPoints - 1);
    }

    //The boulder will travel a distance and with the help of the PathCreator will set position and rotation until it reach the endpoint (Thomas, Gianfranco)
    void Update()
    {
        if (isActive)
        {
            distanceTravelled += speed * Time.deltaTime;
            transform.position = pathCreator.path.GetPointAtDistance(distanceTravelled);
            transform.rotation = pathCreator.path.GetRotationAtDistance(distanceTravelled);

            if (Vector3.Distance(transform.position, endpoint) < 2) {
                isActive = false;
            }
        }
    }

    //If the player die, on its respawn the boulder will be resetted (Thomas)
    public void Reset()
    {
        isActive = false;
        transform.position = originPos;
        transform.rotation = originRot;
        distanceTravelled = 0;
    }


    //When the boulder go forward along its path can kill every enemy that run in it and the player (Gianfranco)
    private void OnTriggerEnter(Collider collision) {
        if(collision.gameObject.tag == "Player") {
            collision.GetComponent<PlayerStatistics>().hurt(DeathEvent.MASHED,true);
        }
        else if (collision.gameObject.tag == "Enemy")
        {
            collision.GetComponent<AIEnemy>().hit();
        }
    }
}
