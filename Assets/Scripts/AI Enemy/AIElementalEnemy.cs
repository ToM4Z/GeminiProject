using System.Collections;
using UnityEngine;

/*
 *  Class: AIElementalEnemy
 *  
 *  Description:
 *  This script extends the enemy behaviour for elemental enemies such as Fire Dragon and Ice Dragon and Bat.
 *  Inherite by AI Enemy
 *  
 *  Author: Thomas Voce
*/
public class AIElementalEnemy : AIEnemy
{
    [SerializeField] private ParticleSystem particle;       // The enemy hit the player using ParticleSystem Trigger
    [SerializeField] private Light lightParticle;           // light of particles
    [SerializeField] private AudioClip elementalAttackClip;
    [SerializeField] private bool MeleeAttack;              // tell if this can perform a melee attack
    [SerializeField] private float distanceMeleeAttack;     // distance to perform an melee attack
    [SerializeField] private float MeleeAttackTime;         
    [SerializeField] private AttackTrigger[] attackTriggersForMeleeAttackElementalEnemy;   // used by dragons, this edit is needed because bat uses an attackTrigger instead of ElementalSpit script
    private bool usedMeleeAttack;                           // memorize if I just perform a melee attack

    protected override void Start()
    {
        base.Start();

        lightParticle.enabled = false;
        particle.Stop();        
    }

    // in start attack
    // if player is so close to enemy, i start a melee attack
    // otherwise I start attack with particles
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

        foreach (AttackTrigger t in attackTriggersForMeleeAttackElementalEnemy) // if i hit player, i hurt him
            if (t.EnteredTrigger)
            {
                PlayerStatistics.instance.hurt(typeAttack);
                break;
            }

        float distanceFromPlayer = fov.distanceTo(player.position);
        if(MeleeAttack && !usedMeleeAttack && distanceFromPlayer <= distanceMeleeAttack)    // if player is so near to enemy (dragons), I start a melee attack
        {
            stopAttack();
            startAttack();
        }

    }

    // in stop attack, I stop the particles
    protected override void stopAttack()
    {
        base.stopAttack();
        foreach (AttackTrigger t in attackTriggersForMeleeAttackElementalEnemy)
            t.DisableTrigger();
        StartCoroutine(AudioManager.FadeOut(soundSource[1], 0.5f)); // fade out attack clip
        particle.Stop();
        StartCoroutine(AbleLight(false));
        usedMeleeAttack = false;
    }

    // activate light after a bit while particles are running
    private IEnumerator AbleLight(bool x)
    {
        yield return new WaitForSeconds(0.1f);
        lightParticle.enabled = x;
    }

    public override void Reset()
    {
        base.Reset();
        particle.Stop();
        lightParticle.enabled = false;
    }
}
