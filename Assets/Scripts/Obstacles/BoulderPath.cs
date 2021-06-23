using System.Collections;
using System.Collections.Generic;
using UnityEngine.AI;
using UnityEngine;
using PathCreation;

public class BoulderPath : MonoBehaviour, IResettable
{
    public bool isActive;
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
        isActive = false;

        originPos = transform.position;
        originRot = transform.rotation;

        transform.position = pathCreator.path.GetPointAtDistance(0);
        endpoint = pathCreator.path.GetPoint(pathCreator.path.NumPoints - 1);

        animator = this.GetComponentInChildren<Animator>();
        _audio = this.GetComponent<AudioSource>();
    }

    void Update()
    {
        animator.SetBool("Move",isActive);
        if (isActive)
        {
            if(!_audio.isPlaying) {
                _audio.Play();
            }
            distanceTravelled += speed * Time.deltaTime;
            transform.position = pathCreator.path.GetPointAtDistance(distanceTravelled);
            transform.rotation = pathCreator.path.GetRotationAtDistance(distanceTravelled);

            if (Vector3.Distance(transform.position, endpoint) < 2) {
                isActive = false;
                _audio.Stop();
            }
        }
    }

    public void Reset()
    {
        isActive = false;
        _audio.Stop();
        transform.position = originPos;
        transform.rotation = originRot;
        distanceTravelled = 0;
    }


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
