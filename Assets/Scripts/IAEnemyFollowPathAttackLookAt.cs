using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IAEnemyFollowPathAttackLookAt : IAEnemy
{
    public float distanceToWalk = 10f;  // distanza da percorrere 
    public float attackTime = 4.5f;
    public float pauseToAttackTime = 3f;

    private ParticleSystem particle;
    public Vector3 targetPathPoint;
    private float oldRotY;

    protected override void Start()
    {
        base.Start();

        particle = GetComponentInChildren<ParticleSystem>();
        particle.Stop();

        ChangeStatus(EStatus.GO);
        targetPathPoint = transform.position;
        targetPathPoint.z += distanceToWalk;
        oldRotY = 0;
    }


    void FixedUpdate()
    {
        if (alive)  // se sono vivo
        {
            if (status != EStatus.WARNED)    // se non è allarmato cammino
            {
                if (status == EStatus.GO)
                {
                    Vector3 move = Vector3.MoveTowards(transform.position, targetPathPoint, speed * Time.fixedDeltaTime);
                    rig.MovePosition(move);

                    float angle = Vector3.Angle(transform.forward, (targetPathPoint - transform.position).normalized);
                    //print(angle);
                    if (!( angle < 1 && angle > -1))
                    {
                        if(angle >= 180)
                            rig.MoveRotation(Quaternion.Euler(transform.localEulerAngles + new Vector3(0, speedRotate * Time.fixedDeltaTime)));
                        else
                            rig.MoveRotation(Quaternion.Euler(transform.localEulerAngles + new Vector3(0, -speedRotate * Time.fixedDeltaTime)));
                    }

                    //transform.LookAt(targetPathPoint);

                    if (Vector3.Distance(transform.position, targetPathPoint) == 0)
                        ChangeStatus(EStatus.TURNBACK);
                }
                else
                if (status == EStatus.TURNBACK)
                {
                    if (oldRotY == 0)
                    {
                        if (transform.rotation.eulerAngles.y < 180)
                        {
                            rig.MoveRotation(Quaternion.Euler(transform.localEulerAngles + new Vector3(0, speedRotate * Time.fixedDeltaTime)));
                        }
                        else
                        {
                            rig.MoveRotation(Quaternion.Euler(0, 180, 0));
                            ChangeStatus(EStatus.GO);
                            targetPathPoint.z -= distanceToWalk;
                            oldRotY = 180;
                        }
                    }
                    else
                    {
                        if (transform.rotation.eulerAngles.y >= 180)
                            rig.MoveRotation(Quaternion.Euler(transform.localEulerAngles + new Vector3(0, speedRotate * Time.fixedDeltaTime)));
                        else
                        {
                            rig.MoveRotation(Quaternion.Euler(0, 0, 0));
                            ChangeStatus(EStatus.GO);
                            targetPathPoint.z += distanceToWalk;
                            oldRotY = 0;
                        }
                    }
                }
                if (particle.isPlaying)
                {
                    StopCoroutine(attackCoroutine);
                    particle.Stop();
                    attacking = false;
                }
            }
            else
            {
                transform.LookAt(player);
                transform.SetPositionAndRotation(transform.position, Quaternion.Euler(0, transform.rotation.eulerAngles.y, 0));

                if (fov.distanceTo(player) <= fov.viewRadius)
                    Attack();
            }
        }
    }

    protected override void Attack()
    {
        if (attacking)
            return;

        attackCoroutine = StartCoroutine(AttackForTimeAndPauseFor(attackTime, pauseToAttackTime));
    }

    IEnumerator AttackForTimeAndPauseFor(float attackTime, float pauseTime)
    {
        attacking = true;

        particle.Play();

        yield return new WaitForSeconds(attackTime);

        particle.Stop();

        yield return new WaitForSeconds(pauseTime);

        if (!fov.isPlayerVisible())
        {
            ChangeStatus(oldStatus);
            player = null;
        }

        attacking = false;

    }

    /*void OnParticleTrigger()
    {
        print("particle collision");

        //Get all particles that entered a box collider
        List<ParticleSystem.Particle> enteredParticles = new List<ParticleSystem.Particle>();
        int enterCount = particle.GetTriggerParticles(ParticleSystemTriggerEventType.Enter, enteredParticles);

        foreach (ParticleSystem.Particle particle in enteredParticles)
        {
            if(player.gameObject.GetComponent<Collider>().bounds.Contains(particle.position))
                Debug.Log("player hitted by fire dragon");
        }

    }*/
}
