using System.Collections;
using UnityEngine;

/*
 *  Class: AIDragon
 *  
 *  Description:
 *  This script extends the enemy behaviour for the Dragon.
 *  Inherite by AI Enemy
 *  
 *  Author: Thomas Voce
*/
public class AIElementalEnemy : AIEnemy
{
    // The dragon hit the player spitting fire/ice with ParticleSystem
    [SerializeField] private ParticleSystem particle;
    [SerializeField] private Light lightParticle;
    [SerializeField] private AudioClip elementalAttackClip;
    [SerializeField] private bool MeleeAttack;
    [SerializeField] private float distanceMeleeAttack;
    [SerializeField] private float MeleeAttackTime;
    [SerializeField] private AttackTrigger[] attackTriggersForMeleeAttackElementalEnemy;   // used by dragons, this edit is needed because bat uses an attackTrigger instead of ElementalSpit script
    private bool usedMeleeAttack;

    protected override void Start()
    {
        base.Start();

        lightParticle.enabled = false;
        particle.Stop();        
    }

    // in start attack, I active the particles
    protected override void startAttack()
    {
        float distanceFromPlayer = fov.distanceTo(player.position);
        if (MeleeAttack && !usedMeleeAttack && distanceFromPlayer <= distanceMeleeAttack)
        {
            animator.Play(attackStateAnim);

            foreach (AttackTrigger t in attackTriggersForMeleeAttackElementalEnemy)
                t.EnableTrigger();

            usedMeleeAttack = true;
            warnedTimer = MeleeAttackTime;
        }
        else
        {
            base.startAttack();
            soundSource[1].clip = elementalAttackClip;
            soundSource[1].Play();
            particle.Play();
            StartCoroutine(AbleLight(true));
        }
    }

    protected override void duringAttack()
    {
        base.duringAttack();

        foreach (AttackTrigger t in attackTriggersForMeleeAttackElementalEnemy)
            if (t.EnteredTrigger)
            {
                PlayerStatistics.instance.hurt(typeAttack);
                break;
            }

        float distanceFromPlayer = fov.distanceTo(player.position);
        if(MeleeAttack && !usedMeleeAttack && distanceFromPlayer <= distanceMeleeAttack)
        {
            stopAttack();
            startAttack();
        }

    }

    // in stop attack, I stop the fire particles
    protected override void stopAttack()
    {
        base.stopAttack();
        foreach (AttackTrigger t in attackTriggersForMeleeAttackElementalEnemy)
            t.DisableTrigger();
        StartCoroutine(AudioManager.FadeOut(soundSource[1], 0.5f));
        particle.Stop();
        StartCoroutine(AbleLight(false));
        usedMeleeAttack = false;
    }

    private IEnumerator AbleLight(bool x)
    {
        yield return new WaitForSeconds(0.1f);
        lightParticle.enabled = x;
    }

    //// during the attack, I adjust the aim of the particles
    //// the hitting player check is made in SpitDragon script
    //protected override void duringAttack()
    //{
    //    //Vector3 posToFire = player.position;
    //    //posToFire.y += 0.5f;
    //    //particle.gameObject.transform.LookAt(posToFire);
    //}

    public override void Reset()
    {
        base.Reset();
        particle.Stop();
        lightParticle.enabled = false;
    }
}
