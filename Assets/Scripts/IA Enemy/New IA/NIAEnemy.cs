using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public abstract class NIAEnemy : MonoBehaviour
{
    public enum EStatus
    {
        IDLE,
        WARNED
    }

    protected NFOVDetection fov;

    protected Transform player;
    protected NavMeshAgent agent;
    protected Rigidbody rig;

    protected bool alive = true;
    protected EStatus status, oldStatus;
    public bool debug = true;
    protected bool attacking;
    protected Coroutine attackCoroutine;
    public float distanceFromPlayer = 3f;

    [HideInInspector]
    public PatrolPath patrolPath;

    protected virtual void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
        rig = GetComponent<Rigidbody>();
        agent = GetComponent<NavMeshAgent>();
        fov = GetComponent<NFOVDetection>();
    }

    void Update()
    {
        if (alive)
        {
            if (status == EStatus.WARNED)
                warned();
            else
                idle();
        }
    }

    protected abstract void idle();
    protected abstract void warned();
    protected abstract void lostwarned();
    protected abstract void attack();

    public void die()
    {
        alive = false;
        // animazione morte
        Destroy(this.gameObject);
    }

    public void Warned() { 
        if(status != EStatus.WARNED)
        {
            agent.stoppingDistance = distanceFromPlayer ;
            ChangeStatus(EStatus.WARNED);
        }
    }

    protected void ChangeStatus(EStatus s)
    {
        if(debug)
            print(s);

        oldStatus = status;
        status = s;
    }

    protected void FaceTarget(Vector3 target)
    {
        Vector3 direction = (target - transform.position).normalized;
        Quaternion lookRotation = Quaternion.LookRotation(new Vector3(direction.x, 0, direction.z));
        transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * 3f);
    }
    protected void FacePlayer()
    {
        FaceTarget(player.position);
    }
}
