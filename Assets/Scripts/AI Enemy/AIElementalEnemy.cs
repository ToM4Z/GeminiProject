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
    [SerializeField] private AudioSource attackSFXSource;

    protected override void Start()
    {
        base.Start();

        lightParticle.enabled = false;
        particle.Stop();        
    }

    // in start attack, I active the particles
    protected override void startAttack()
    {
        base.startAttack();
        attackSFXSource.clip = attackClip;
        attackSFXSource.Play();
        particle.Play();
        StartCoroutine(AbleLight(true));
    }

    // in stop attack, I stop the fire particles
    protected override void stopAttack()
    {
        base.stopAttack();
        StartCoroutine(AudioManager.FadeOut(attackSFXSource, 0.5f));
        particle.Stop();
        StartCoroutine(AbleLight(false));
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

    public override void ResetEnemy()
    {
        base.ResetEnemy();
        particle.Stop();
        lightParticle.enabled = false;
    }
}
