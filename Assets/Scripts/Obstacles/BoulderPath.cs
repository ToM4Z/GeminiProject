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

    void Start()
    {
        isActive = false;

        originPos = transform.position;
        originRot = transform.rotation;

        transform.position = pathCreator.path.GetPointAtDistance(0);
        endpoint = pathCreator.path.GetPoint(pathCreator.path.NumPoints - 1);
    }

    void Update()
    {
        if (isActive)
        {
            distanceTravelled += speed * Time.deltaTime;
            transform.position = pathCreator.path.GetPointAtDistance(distanceTravelled);

            if (Vector3.Distance(transform.position, endpoint) < 2)
                isActive = false;
        }
    }

    public void Reset()
    {
        isActive = false;

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
