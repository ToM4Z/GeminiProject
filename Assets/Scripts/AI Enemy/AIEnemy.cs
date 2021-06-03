using UnityEngine;
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
[RequireComponent(typeof(AudioSource))]
[RequireComponent(typeof(Rigidbody))]
public class AIEnemy : MonoBehaviour
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
        NO,         // The enemy is not attacking the player
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

    // It's the patrolpath that enemy will follow
    [Tooltip("Drag here a gameobject with a PatrolPath component")]
    [SerializeField]
    public PatrolPath patrolPath;

    [Tooltip("The distance at which the enemy considers that it has reached its current path destination point")]
    public float PathReachingRadius = 0.5f;

    [Tooltip("It's the maximum distance that the enemy can reach from the nearest patrolpath node or from its original position")]
    public float maxDistancePatrol = 6f;

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


    //AUDIO VARIABLES
    protected AudioSource soundSource;
    [SerializeField] protected AudioClip spawnClip, idleClip, walkClip, attackClip, dieClip;
    [SerializeField] private AudioSource idleLoopSoundSource;


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

        soundSource = GetComponent<AudioSource>();

        originPos = transform.position;
        originRot = transform.rotation;

        if (!isInitialStatusAcceptable(initialStatus))
            throw new System.Exception("Error: initial status not acceptable for gameobject " + gameObject.name);

        if (initialStatus != Status.INACTIVE)
        {
            spawn = true;
            animator.SetTrigger(animVarSpawn);
        }

        //agent.autoBraking = false;

        ChangeStatus(initialStatus);
    }

    // Update method equal to all subclasses
    void Update()
    {
        animStateInfo = animator.GetCurrentAnimatorStateInfo(0);

        if(attackFase != AttackFase.NO)                 // if I'm executing some attack fase, I decrease the timer
            warnedTimer -= Time.deltaTime;

        switch (status)                                 // in base of the status, I execute the relative method
        {
            case Status.INACTIVE: { inactiveIdle(); break; }
            case Status.IDLE: { idle(); break; }
            case Status.WARNED: { warned(); break; }
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
        // if patrolPath is enabled, it follows the path.
        if (patrolPath)
        {
            UpdatePathDestination();
            SetNavDestination(GetDestinationOnPath());
        }
        else
        {
            // Otherwise, if I'm far from my origin point, I go there
            if (fov.distanceTo(originPos) > PathReachingRadius)
                SetNavDestination(originPos);
            else
                // if I'm are already there, I look in the initial direction
                transform.rotation = Quaternion.Slerp(transform.rotation, originRot, Time.deltaTime * 3f);
        }

        // if I see the player, I pass in WARNED state
        if (fov.isPlayerVisible())
        {
            agent.stoppingDistance = minDistanceToAttack;
            ChangeStatus(Status.WARNED);
        }
    }

    // With this method, the player can hit and kill the enemy
    public void hurt()
    {
        if (status == Status.DEAD || status == Status.INACTIVE)
            return;

        ChangeStatus(Status.DEAD);

        agent.ResetPath();
        foreach(Collider c in GetComponentsInChildren<Collider>())
            c.enabled = false;
        
        if(attackFase != AttackFase.NO)
            stopAttack();

        animator.SetTrigger(deathStateAnim);

        if(idleLoopSoundSource != null)
        {
            idleLoopSoundSource.loop = false;
            StartCoroutine(AudioManager.FadeOut(idleLoopSoundSource, 0.5f));
        }

        OnDeath();

        Managers.Enemies.EnemyDie(this.gameObject);
        StartCoroutine(Disappear());
    }

    private IEnumerator Disappear()     //the enemy will disappear after 5 seconds
    {
        yield return new WaitForSeconds(5f);
        if(status == Status.DEAD)       // if the enemy is still dead (during this 5 seconds the player should be die) 
            this.gameObject.SetActive(false);   //i disactive the enemy
    }

    // this method handles the behaviur of the enemy when have to attack the player
    protected void warned()
    {
        FacePlayer();   // first of all, it rotates towards the player

        if (enableMoveToPlayer)  
        {
            // IF I can move towards the player &&
            // the player / enemy is not so far from the origin point / patrol path &&
            // the enemy is not attacking or he can move while is attacking
            // so he can move towards the player
            // Otherwise, the agent is resetted

            GetClosestPatrolNode(out float distanceFromClosestNode);
            float distancePlayerFromMyOrigin = Vector3.Distance(GetPositionClosestPatrolNode(), player.position);

            if ((distancePlayerFromMyOrigin <= maxDistancePatrol || distanceFromClosestNode <= maxDistancePatrol) && (moveWhileAttack || attackFase != AttackFase.ATTACK))
            {
                SetNavDestination(player.transform.position);
            }
            else
                agent.ResetPath();
        }

        // IF I'm not attacking 
        float actualDistanceFromPlayer = fov.distanceTo(player.position);
        if (attackFase == AttackFase.NO)
        {
            // and I'm close enough to the player
            if (actualDistanceFromPlayer <= minDistanceToAttack)
            {
                attackFase = AttackFase.ATTACK;     // I attack him
                warnedTimer = attackTime;
                startAttack();
            }
            // otherwise, if I don't see the player, began the lost view timer 
            else if(!fov.isPlayerVisible())
            {
                attackFase = AttackFase.LOSTVIEW;
                warnedTimer = lostViewTime;
            }
        }
        else if (attackFase != AttackFase.NO)
        {
            switch (attackFase)
            {
                case AttackFase.NO:
                    {
                        break;
                    }
                case AttackFase.ATTACK: // IF I'm attacking
                    {
                        // I execute some control during attack (implemented by subclasses)
                        duringAttack();

                        // if the attack time is finished, It will start the pause fase
                        if (warnedTimer <= 0f)
                        {
                            stopAttack();
                            warnedTimer = pauseBetweenAttacksTime;
                            attackFase = AttackFase.PAUSE;
                        }
                        break;
                    }
                case AttackFase.PAUSE:
                    {
                        // When i finish the pause time
                        if (warnedTimer <= 0f)
                        {
                            // if the player is not visible yet, I turn in IDLE state
                            if (!fov.isPlayerVisible())
                            {
                                lostWarned();
                            }
                            else
                            // if I'm close enough to the player, I start another attack 
                            if (actualDistanceFromPlayer <= minDistanceToAttack)
                            {
                                attackFase = AttackFase.ATTACK;
                                warnedTimer = attackTime;
                                startAttack();
                            }
                            // Otherwise, I don't do anything (while in I should walk against the player to start another actions)
                            else
                            {
                                attackFase = AttackFase.NO;
                                warnedTimer = 0f;
                            }
                        }
                        break;
                    }
                case AttackFase.LOSTVIEW:   // IF I'm in LOSTVIEW fase
                    {
                        // if I see the player, I turn in No attack fase
                        if (fov.isPlayerVisible())
                        {
                            attackFase = AttackFase.NO;
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
    }


    //  this method reset some variables and turn enemy on IDLE status
    protected virtual void lostWarned()
    {
        warnedTimer = 0f;
        attackFase = AttackFase.NO;

        agent.stoppingDistance = 0f;
        if (patrolPath)
            SetPathDestinationToClosestNode();

        ChangeStatus(Status.IDLE);
    }

    // simple change the status and, if debug is enabled, prints the change
    protected void ChangeStatus(Status s)
    {
        if (debug)
            print(s);
        status = s;
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
    protected virtual void OnDeath() 
    {
        soundSource.PlayOneShot(dieClip);
    }





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

    // Check if reached the path destination so that it set the next path destination
    protected void UpdatePathDestination()
    {        //fov.distanceTo(GetDestinationOnPath()) <=
        if (agent.remainingDistance < PathReachingRadius)
        {
            m_PathDestinationNodeIndex++;
            m_PathDestinationNodeIndex %= patrolPath.PathNodes.Count;
        }
    }

    public virtual void ResetEnemy()
    {
        transform.position = originPos;
        transform.rotation = originRot;

        foreach (Collider c in GetComponentsInChildren<Collider>())
            c.enabled = true;

        m_PathDestinationNodeIndex = 1;
        agent.ResetPath();
        agent.stoppingDistance = 0f;

        warnedTimer = 0f;
        attackFase = AttackFase.NO;

        animator.Rebind();

        if (idleLoopSoundSource != null)
        {
            idleLoopSoundSource.loop = true;
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
        soundSource.PlayOneShot(spawnClip);
    }

    public virtual void PlayIdleSound()
    {
        soundSource.PlayOneShot(idleClip);
    }

    public virtual void PlayWalkSound()
    {
        soundSource.PlayOneShot(walkClip);
    }

    public virtual void PlayAttackSound()
    {
        soundSource.PlayOneShot(attackClip);
    }
}
