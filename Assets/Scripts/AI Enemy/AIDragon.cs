using UnityEngine;

/*
 *  Class: AIDragon
 *  
 *  Description:
 *  This script describe the Dragons behaviur.
 *  Inherite by AI Enemy
 *  
 *  Author: Thomas Voce
*/
public class AIDragon : AIEnemy
{
    // The dragon hit the player spitting fire/ice with ParticleSystem
    private ParticleSystem particle;

    protected override void Start()
    {
        base.Start();

        particle = GetComponentInChildren<ParticleSystem>();
        particle.Stop();        
    }

    // in start attack, I active the particles
    protected override void startAttack()
    {
        particle.Play();
    }

    // in stop attack, I stop the fire particles
    protected override void stopAttack()
    {
        particle.Stop();
    }

    // during the attack, I adjust the aim of the particles
    // the hitting player check is made in SpitDragon script
    protected override void duringAttack()
    {
        Vector3 posToFire = player.position;
        posToFire.y += 0.5f;
        particle.gameObject.transform.LookAt(posToFire);
    }

    // when I hurt, I stop to spit
    protected override void AfterHit()
    {
        particle.Stop();
    }

    public override void Reset()
    {
        base.Reset();
        particle.Stop();

    }
}
