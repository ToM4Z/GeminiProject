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
    private AudioSource soundSource;
    [SerializeField] private AudioClip beatWingsSFX;
    [SerializeField] private AudioClip spitSFX;
    [SerializeField] private AudioClip deathSFX;

    protected override void Start()
    {
        base.Start();

        particle = GetComponentInChildren<ParticleSystem>();
        soundSource = GetComponent<AudioSource>();
        particle.Stop();        
    }

    // in start attack, I active the particles
    protected override void startAttack()
    {
        particle.Play();
        soundSource.PlayOneShot(spitSFX);
    }

    // in stop attack, I stop the fire particles
    protected override void stopAttack()
    {
        particle.Stop();
        soundSource.Stop();
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
    protected override void OnDeath()
    {
        soundSource.PlayOneShot(deathSFX);
    }

    public void BeatWings()
    {
        soundSource.PlayOneShot(beatWingsSFX);
    }

    public override void Reset()
    {
        base.Reset();
        particle.Stop();
        soundSource.Stop();

    }
}
