using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IADragon : IAEMovePattern
{
    private Transform fireT;
    private ParticleSystem fire;
    public float attackTime = 4.5f;
    public float pauseToAttackTime = 3f;
    private float attackTiming = 0f;
    private float pauseTiming = 0f;

    protected override void Start()
    {
        base.Start();
        fire = GetComponentInChildren<ParticleSystem>();
        fireT = fire.gameObject.transform;
        fire.Stop();
    }

    protected override void attack()
    {
        if (attacking)
        {
            if(pauseTiming == 0f)
            {
                fireT.LookAt(player.position);
                attackTiming -= Time.deltaTime;
                if (attackTiming <= 0f)
                {
                    fire.Stop();

                    pauseTiming = pauseToAttackTime;
                }
            }
            else
            {
                pauseTiming -= Time.deltaTime;
                if(pauseTiming <= 0f)
                {
                    if (!fov.isPlayerVisible())
                    {
                        lostwarned();
                        pauseTiming = attackTiming = 0f;
                        attacking = false;
                    }
                    else
                    {
                        fire.Play();

                        attackTiming = attackTime;
                        pauseTiming = 0f;
                    }
                }
            }
        }
        else
        {
            attacking = true;

            fire.Play();

            attackTiming = attackTime;
        }
    }

    /*protected override void attack()    
    {
        if (attacking)
            return;

        attackCoroutine = StartCoroutine(AttackForTimeAndPauseFor());
    }*/

    IEnumerator AttackForTimeAndPauseFor()
    {
        attacking = true;

        fire.Play();

        yield return new WaitForSeconds(attackTime);

        fire.Stop();

        yield return new WaitForSeconds(pauseToAttackTime);

        if (!fov.isPlayerVisible())
        {
            ChangeStatus(oldStatus);
        }

        attacking = false;

    } 
}
