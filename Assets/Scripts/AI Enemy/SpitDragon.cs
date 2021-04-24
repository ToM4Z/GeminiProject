using System.Collections.Generic;
using UnityEngine;
/*
 * Spit Dragon Script
 * 
 * This script checks that when the particles are triggered by player, then I hit the player
 */
public class SpitDragon : MonoBehaviour
{
    ParticleSystem particle;

    void Start()
    {
        particle = GetComponent<ParticleSystem>();
        particle.trigger.AddCollider(PlayerController.instance.GetComponent<Transform>());
    }

    // there is no need to check if the gameobject that triggered the particles is the player
    // since it's specified in ParticleSystem's Trigger module
    void OnParticleTrigger()
    {
        int insideCount = particle.GetTriggerParticles(ParticleSystemTriggerEventType.Inside, new List<ParticleSystem.Particle>());

        if (insideCount > 40)
            PlayerController.instance.Damage(1);
    }
}
