using UnityEngine;
using UnityEditor;
using UnityEngine.AI;
using System.Collections;

/*
 *  Class: AIEnemy
 *  
 *  Description:
 *  This script describes the general behaviour of all enemies.
 *  
 *  Author: Thomas Voce
*/
[RequireComponent(typeof(FOVDetection))]
[RequireComponent(typeof(NavMeshAgent))]
[RequireComponent(typeof(Rigidbody))]
public class AIEnemy : MonoBehaviour, IHittable, IResettable
{
    //--------------GENERAL-------------
    [Tooltip("If enabled, prints will be enabled")]
    public bool debug = false;

    // if enemy start from INACTIVE status, this variable, if enabled, allows to pass at IDLE status
    private bool spawn = false, spawning = false;

    [Tooltip("Acceptable initial status are INACTIVE or IDLE")]
    [SerializeField] private Status initialStatus;

    // Denote the status of the enemy
    protected Status status;
    protected enum Status
    {
        INACTIVE,   // In INACTIVE status, the enemy wait the event to 'spawn' and pass to IDLE status
        IDLE,       // In IDLE status, the enemy can either follow a patrol path or stand still (it depends from 'patrolPath' value)
        WARNED,     // In WARNED status, the enemy, if 'enableMoveToPlayer' is enabled, follow and attack the player
        DEAD        // In DEAD status, the enemy start animation of death and then it will disappear
    }

    protected FOVDetection fov;
    protected Transform player;
    protected NavMeshAgent agent;

    //--------------ATTACK VARIABLES-------------
    
    [Tooltip("The time it takes to perform an attack")]
    public float attackTime = 2f;

    [Tooltip("The time passes between one attack and another")]
    public float pauseBetweenAttacksTime = 1f;

    [Tooltip("Indicates the waiting time before returning to IDLE status from last time it saw the player")] 
    public float lostViewTime = 2f;

    [Tooltip("It's the distance needed to attack and hit the player")]
    public float minDistanceToAttack = 1.3f;

    [Tooltip("It's the rotation speed when he face to target")]
    public float rotationSpeed = 3f;

    // It's a timer used in some attack fases in 'warned' method
    protected float warnedTimer = 0f;

    // Denota the attack fase
    protected AttackFase attackFase = AttackFase.NO;
    protected enum AttackFase 
    { 
        NO,         // The enemy can see the player but he is not attacking him yet
        ATTACK,     // The enemy is attacking the player  
        PAUSE,      // The enemy just ends to attack player and will wait a bit before to attack again
        LOSTVIEW    // The enemy doesn't see more the player. He will wait a bit and then, if the player is not visible yet, he will turn on IDLE status
    }

    [Tooltip("It allows the enemy to reach the player to attack")]
    public bool enableMoveToPlayer = true;

    [Tooltip("It allows the enemy to move while he's still executing an attack")]
    public bool moveWhileAttack = false;


    //--------------PATROL VARIABLES-------------

    // This are the initial position and rotation of the enemy. In case the patrol path is null, it will use this variables.
    private Vector3 originPos;
    private Quaternion originRot;

    public enum IdleType { NONE, PATROL, RANDOM }
    [Tooltip("Set the idle type")]
    [SerializeField]
    public IdleType idleType;

    // It's the patrolpath that enemy will follow
    [Tooltip("Drag here a gameobject with a PatrolPath component")]
    [SerializeField]
    public PatrolPath patrolPath;

    [Tooltip("The distance at which the enemy considers that it has reached its current path destination point")]
    private float PathReachingRadius = 0.25f;

    [Tooltip("It's the maximum distance that the enemy can reach from the nearest patrolpath node or from its original position")]
    public float maxDistancePatrol = 6f;

    private Vector3 randomDestination;  // it's the destination path created randomly

    [SerializeField] private float idleTime;    // it's used in IdleType.RANDOM mode to wait some seconds before find a new randomDestination
    private float idleTimer;
    private bool idleWaiting = false;

    // Indicate the node at which the enemy is pointing
    int m_PathDestinationNodeIndex = 1;


    //--------------ANIMATION VARIABLES-------------

    protected Animator animator;
    protected AnimatorStateInfo animStateInfo;

    // Animation state names
    protected string idleStateAnim = "Idle",
        walkStateAnim = "Walk",
        attackStateAnim = "Attack",
        deathStateAnim = "Death",

    // Animation variable names
        animVarSpawn = "Spawn",
        animVarSpeed = "Speed";

    private Transform model;
    private Vector3 originModelPos;
    private Quaternion originModelRot;


    //AUDIO VARIABLES
    protected AudioSource[] soundSource;
    // this array will contains two soundSources,
    // the first will used to play all little clips
    // the second will be used to play long clips which have to fade out (dragon attack)
    // the third will be used to idle clips which have to loop
    [SerializeField] protected AudioClip spawnClip, idleClip, walkClip, attackClip, deathClip;

    [SerializeField]
    private AttackTrigger[] attackTriggers;

    [SerializeField] private DeathEvent typeAttack = DeathEvent.HITTED;

    // General Start Method in which the enemy start from IDLE state
    protected virtual void Start()
    {
        player = PlayerStatisticsController.instance.transform;
        agent = GetComponent<NavMeshAgent>();
        fov = GetComponent<FOVDetection>();
        animator = GetComponentInChildren<Animator>();

        soundSource = GetComponentsInChildren<AudioSource>();

        originPos = transform.position;
        originRot = transform.rotation;

        if (!isInitialStatusAcceptable(initialStatus))
            throw new System.Exception("Error: initial status not acceptable for gameobject " + gameObject.name);

        if(patrolPath != null && idleType != IdleType.PATROL)
            throw new System.Exception("Error: cannot assign a PatrolPath with a IdleType different from PATROL for object: " + gameObject.name);
        else if (patrolPath == null && idleType == IdleType.PATROL)
            throw new System.Exception("Error: PatrolPath not assigned for object: " + gameObject.name);

        if (initialStatus != Status.INACTIVE)
        {
            spawn = true;
            animator.SetTrigger(animVarSpawn);
        }

        //agent.autoBraking = false;
        idleTimer = -1; // this will cause a call to newRandomDestination

        model = transform.GetChild(0);
        originModelPos = model.localPosition;
        originModelRot = model.localRotation;

        ChangeStatus(initialStatus);
    }

    // Update method equal to all subclasses
    void Update()
    {
        animStateInfo = animator.GetCurrentAnimatorStateInfo(0);

        switch (status)                                 // in base of the status, I execute the relative method
        {
            case Status.INACTIVE: 
                inactiveIdle(); 
                break; 

            case Status.IDLE:
                fov.checkIsPlayerVisible();
                idle(); 
                break; 
                
            case Status.WARNED:
                fov.checkIsPlayerVisible();
                warned(); 
                break;

            //case Status.DEAD  this event is handled in 'hurt' method 
        }

        animator.SetFloat(animVarSpeed, agent.velocity.magnitude);      // updates the Speed variable of the animator
    }

    // method called when status is INACTIVE
    // if spawn is true, activated from external event, it passes to IDLE state
    protected virtual void inactiveIdle()
    {
        if (spawn && !spawning)
        {
            spawning = true;
            animator.SetTrigger(animVarSpawn);
        }
        else if (animStateInfo.IsName(idleStateAnim))
        {
            spawning = false;
            ChangeStatus(Status.IDLE);
        }
    }

    // method called when status is IDLE
    protected virtual void idle()
    {
        if (idleWaiting)
            idleTimer -= Time.deltaTime;

        switch (idleType)
        {
            case IdleType.NONE: 
                // In this mode, I have to stay in my originPos, so if I followed the player, I have to return home.

                // If I'm so close to my originPos
                if (fov.distanceTo(originPos) < PathReachingRadius)
                    // I rotate to the originRot
                    transform.rotation = Quaternion.Slerp(transform.rotation, originRot, Time.deltaTime * rotationSpeed);
                else
                    // else, I go in my originPos
                    SetNavDestination(originPos);
                break;


            case IdleType.PATROL:
                // In this mode, if I have a patrol path component attached to me, I follow its node.

                if (patrolPath)
                {
                    // if I reached the path destination I go to the next patrol path node
                    if (agent.remainingDistance < PathReachingRadius)//fov.distanceTo(GetDestinationOnPath()) <=
                    {
                        m_PathDestinationNodeIndex = (m_PathDestinationNodeIndex + 1) % patrolPath.PathNodes.Count;
                        SetNavDestination(GetDestinationOnPath());
                    }
                }
                break;


            case IdleType.RANDOM:
                // In this mode, I can move randomly
                // I set a random destination and I go there,
                // When I arrive there, I wait 'IdleTime' seconds
                // and then I will set a new random destination 

                if(idleTimer < 0)
                {
                    idleWaiting = false;
                    idleTimer = 0;
                    setRandomDestination();
                }
                else
                if (!idleWaiting && fov.distanceTo(randomDestination) < PathReachingRadius)
                {
                    idleWaiting = true;
                    idleTimer = idleTime;
                }
                break;
        }

        // if I see the player, I pass in WARNED state
        if (fov.isPlayerVisible)
        {
            idleTimer = -1;
            idleWaiting = false;
            agent.stoppingDistance = minDistanceToAttack;
            fov.viewRadius *= 1.5f;
            ChangeStatus(Status.WARNED);
        }
    }

    protected void warned()
    {
        if (attackFase != AttackFase.NO)                 // if I'm executing some attack fase, I decrease the timer
            warnedTimer -= Time.deltaTime;

        FacePlayer();   // first of all, it rotates towards the player

        float actualDistanceFromPlayer = fov.distanceTo(fov.lastPlayerPositionKnown);

        if (enableMoveToPlayer)
        {
            // IF I can move towards the player &&
            // the player or enemy is not so far from the origin point / patrol path &&
            // the enemy is not attacking or he can move while is attacking
            // so he can move towards the player
            // Otherwise, the agent is resetted

            GetClosestPatrolNode(out float distanceFromClosestNode);
            float distancePlayerFromMyOrigin = Vector3.Distance(GetPositionClosestPatrolNode(), fov.lastPlayerPositionKnown);
            
            if (
                    (distancePlayerFromMyOrigin <= maxDistancePatrol || distanceFromClosestNode <= maxDistancePatrol) && 
                    actualDistanceFromPlayer > minDistanceToAttack && 
                    (moveWhileAttack || attackFase != AttackFase.ATTACK)
                )
            {
                SetNavDestination(fov.lastPlayerPositionKnown);
            }
            else
            {
                agent.ResetPath();
            }
        }

        // IF I'm not attacking 
        if (attackFase == AttackFase.NO)
        {
            // and I'm close enough to the player
            if (actualDistanceFromPlayer <= minDistanceToAttack)
            {
                ChandeAttackState(AttackFase.ATTACK);   // I attack him
                warnedTimer = attackTime;
                startAttack();
            }
            // otherwise, if I don't see the player, began the lost view timer 
            else if (!fov.isPlayerVisible)
            {
                ChandeAttackState(AttackFase.LOSTVIEW);
                warnedTimer = lostViewTime;
            }
        }

        else
        {
            switch (attackFase)
            {
                case AttackFase.NO:     break;

                case AttackFase.ATTACK: // IF I'm attacking
                    
                        // I execute some control during attack (implemented by subclasses)
                        duringAttack();

                        // if the attack time is finished, It will start the pause fase
                        if (warnedTimer <= 0f)
                        {
                            stopAttack();
                            warnedTimer = pauseBetweenAttacksTime;
                            ChandeAttackState(AttackFase.PAUSE);
                        }
                        break;
                    
                case AttackFase.PAUSE:
                    
                        // When i finish the pause time
                        if (warnedTimer <= 0f)
                        {
                            // if the player is not visible yet, I turn in IDLE state
                            if (!fov.isPlayerVisible)
                            {
                                ChandeAttackState(AttackFase.LOSTVIEW);
                                warnedTimer = lostViewTime;
                            }
                            else
                            // if I'm close enough to the player, I start another attack 
                            if (actualDistanceFromPlayer <= minDistanceToAttack)
                            {
                                ChandeAttackState(AttackFase.ATTACK);
                                warnedTimer = attackTime;
                                startAttack();
                            }
                            // Otherwise, I don't do anything (while in I should walk against the player to start another actions)
                            else
                            {
                                ChandeAttackState(AttackFase.NO);
                                warnedTimer = 0f;
                            }
                        }
                        break;
                    
                case AttackFase.LOSTVIEW:   // IF I'm in LOSTVIEW fase
                    
                        // if I see the player, I turn in No attack fase
                        if (fov.isPlayerVisible)
                        {
                            ChandeAttackState(AttackFase.NO);
                            warnedTimer = 0f;
                        }
                        // if the timer is finished and the player is not visible until now, I turn in IDLE state
                        else if (warnedTimer <= 0f)
                        {
                            lostWarned();
                        }
                        break;
            }

        }
    }

    //  this method reset some variables and turn enemy on IDLE status
    protected virtual void lostWarned()
    {
        warnedTimer = 0f;
        ChandeAttackState(AttackFase.NO);

        fov.viewRadius /= 1.5f;
        agent.stoppingDistance = 0f;
        if (patrolPath)
            SetPathDestinationToClosestNode();

        ChangeStatus(Status.IDLE);
    }

    // With this method, the player can hit and kill the enemy
    public void hit()
    {
        if (status == Status.DEAD || status == Status.INACTIVE)
            return;

        ChangeStatus(Status.DEAD);

        agent.ResetPath();
        foreach (Collider c in GetComponentsInChildren<Collider>())
            c.enabled = false;

        if (attackFase != AttackFase.NO)
            stopAttack();

        animator.SetTrigger(deathStateAnim);

        if (soundSource[2].clip != null)
        {
            soundSource[2].loop = false;
            StartCoroutine(AudioManager.FadeOut(soundSource[2], 0.5f));
        }

        OnDeath();

        Managers.Enemies.EnemyDie(this.gameObject);
    }

    // simple change the status and, if debug is enabled, prints the change
    protected void ChangeStatus(Status s)
    {
        status = s;
        if (debug)
            print(gameObject.name + ": " + status);
    }


    // simple change the attack fase and, if debug is enabled, prints the change
    protected void ChandeAttackState(AttackFase fase)
    {
        attackFase = fase;
        if (debug)
            print(gameObject.name + ": " + attackFase);
    }

    // method that subclasses can override to add behaviour when enemy START to attack
    protected virtual void startAttack()
    {
        animator.SetTrigger(attackStateAnim);

        foreach (AttackTrigger t in attackTriggers)
            t.EnableTrigger();
    }

    // method that subclasses can override to add behaviour when enemy STOP to attack
    protected virtual void stopAttack()
    {
        foreach (AttackTrigger t in attackTriggers)
            t.DisableTrigger();
    }

    // method that subclasses MUST override to add behaviour DURING the enemy attack
    // in this method, it must be implemented how to hit the player
    protected virtual void duringAttack()
    {
        foreach (AttackTrigger t in attackTriggers)
            if (t.EnteredTrigger)
            {
                PlayerStatisticsController.instance.hurt(typeAttack);
                break;
            }
    }


    // method that subclasses can override to add behaviour when player hit this enemy
    protected virtual void OnDeath() {}





    //--------------PATROL METHODS-------------

    // rotate (slerp) the enemy towards a position target
    protected void FaceTarget(Vector3 target)
    {
        Vector3 direction = (target - transform.position).normalized;
        Quaternion lookRotation = Quaternion.LookRotation(new Vector3(direction.x, 0, direction.z));
        transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * rotationSpeed);
    }

    // rotate (slerp) the enemy towards the player
    protected void FacePlayer()
    {
        FaceTarget(player.position);
    }

    // Set the closest node in patrol path as destination
    protected void SetPathDestinationToClosestNode()
    {
        m_PathDestinationNodeIndex = GetClosestPatrolNode();
    }

    // Return the closest node in patrol path
    protected int GetClosestPatrolNode()
    {
        return GetClosestPatrolNode(out _);
    }

    // Return the closest node in patrol path and save the distance to it in distanceToNode
    protected int GetClosestPatrolNode(out float distanceToNode)
    {
        if (!patrolPath)
        {
            distanceToNode = fov.distanceTo(originPos);
            return 0;
        }
        int closestPathNodeIndex = 0;
        distanceToNode = 0;
        for (int i = 0; i < patrolPath.PathNodes.Count; i++)
        {
            float distanceToPathNode = patrolPath.GetDistanceToNode(transform.position, i);
            if (distanceToPathNode < patrolPath.GetDistanceToNode(transform.position, closestPathNodeIndex))
            {
                closestPathNodeIndex = i;
                distanceToNode = distanceToPathNode;
            }
        }
        return closestPathNodeIndex;
    }

    // Return the position of the actual destination in patrol path
    protected Vector3 GetDestinationOnPath()
    {
        return patrolPath.GetPositionOfPathNode(m_PathDestinationNodeIndex);
    }

    // Return the position of the closest patrol node
    // if patrolPath is null, it will return the originPos
    protected Vector3 GetPositionClosestPatrolNode()
    {
        if (!patrolPath)
            return originPos;
        return patrolPath.GetPositionOfPathNode(GetClosestPatrolNode());
    }

    // Set the destination of the nav mesh agent
    protected void SetNavDestination(Vector3 destination)
    {
        agent.SetDestination(destination);
    }

    private void setRandomDestination()
    {
        randomDestination = GenerateRandomPosition(originPos, maxDistancePatrol*.8f, NavMesh.AllAreas);
        SetNavDestination(randomDestination);
    }

    public static Vector3 GenerateRandomPosition(Vector3 origin, float dist, int layermask)
    {
        bool find;
        Vector3 position;
        NavMeshPath path = new NavMeshPath();
        do
        {
            Vector3 randDirection = Random.insideUnitSphere * dist;
            randDirection += origin;
            find = NavMesh.SamplePosition(randDirection, out NavMeshHit navHit, dist, layermask);
            position = navHit.position;
        } while (!find || !NavMesh.CalculatePath(origin, position, layermask, path));

        return position;
    }

    public virtual void Reset()
    {
        transform.position = originPos;
        transform.rotation = originRot;

        foreach (Collider c in GetComponentsInChildren<Collider>())
            c.enabled = true;

        m_PathDestinationNodeIndex = 1;
        agent.ResetPath();
        agent.stoppingDistance = 0f;
        fov.Reset();

        idleTimer = -1;
        idleWaiting = false;

        warnedTimer = 0f;
        attackFase = AttackFase.NO;

        animator.Rebind();

        if (soundSource[2].clip != null)
        {
            soundSource[2].loop = true;
        }

        if (initialStatus != Status.INACTIVE)
        {
            spawn = true;
            animator.SetTrigger(animVarSpawn);
        }
        else
            spawn = false;

        status = initialStatus;
    }

    public void AwakeEnemy()
    {
        spawn = true;
    }

    protected virtual bool isInitialStatusAcceptable(Status s)
    {
        return s == Status.IDLE || s == Status.INACTIVE;
    }

    public virtual void PlaySpawnSound()
    {
        soundSource[0].PlayOneShot(spawnClip);
    }

    public virtual void PlayIdleSound()
    {
        soundSource[0].PlayOneShot(idleClip);
    }

    public virtual void PlayWalkSound()
    {
        soundSource[0].PlayOneShot(walkClip);
    }

    public virtual void PlayAttackSound()
    {
        soundSource[0].PlayOneShot(attackClip);
    }

    public virtual void PlayDeathSound()
    {
        soundSource[0].PlayOneShot(deathClip);
    }

    public void StartDespawn()
    {
        StartCoroutine(StartDespawnDelayed());
    }

    private IEnumerator StartDespawnDelayed()
    {
        yield return new WaitForSeconds(4f);
        if (status == Status.DEAD)       // if the enemy is still dead (during this 5 seconds the player should be die) 
            animator.SetTrigger("Despawn"); // I start Despawn
    }

    public void Disable()
    {
        // reset model transform, despawn and other animations could edit them
        model.localPosition = originModelPos;
        model.localRotation = originModelRot;
        model.localScale    = Vector3.one;
        this.gameObject.SetActive(false);
    }

    void OnDrawGizmosSelected()
    {
        if(originPos != null && originPos != Vector3.zero)
        {
            Gizmos.color = Color.cyan;
            Gizmos.DrawSphere(originPos, 0.1f);
            Gizmos.color = Color.green;
            Gizmos.DrawLine(originPos, originPos + Vector3.up);
            Gizmos.color = Color.red;
            Gizmos.DrawLine(originPos, originPos + originRot * Vector3.right);
            Gizmos.color = Color.blue;
            Gizmos.DrawLine(originPos, originPos + originRot * Vector3.forward);

            GUI.color = Color.black;
            Handles.Label(originPos, Vector3.Distance(originPos, transform.position).ToString());
            Handles.Label( new Vector3(transform.position.x, transform.position.y + 1, transform.position.z), fov.distanceTo(fov.lastPlayerPositionKnown).ToString());
            Handles.color = Color.black;
            Handles.DrawWireDisc(originPos, Vector3.up, maxDistancePatrol * .8f);            
        }
    }
}
