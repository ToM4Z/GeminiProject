using System.Collections.Generic;
using UnityEngine;

/*
 *  Class: SpitDragon
 *  
 *  Description:
 *  This script checks that when the particles are triggered by player, then I hit the player.
 *  
 *  Author: Thomas Voce
*/
public class SpitDragon : MonoBehaviour
{
    ParticleSystem particle;

    [SerializeField]
    [Tooltip("True if it's fire, otherwise ice.")]
    private bool fire;

    void Start()
    {
        particle = GetComponent<ParticleSystem>();
        particle.trigger.AddCollider(PlayerStatisticsController.instance.GetComponent<Transform>());
    }

    // there is no need to check if the gameobject that triggered the particles is the player
    // since it's specified in ParticleSystem's Trigger module
    void OnParticleTrigger()
    {
        int insideCount = particle.GetTriggerParticles(ParticleSystemTriggerEventType.Inside, new List<ParticleSystem.Particle>());

        if (insideCount > 20)
            PlayerStatisticsController.instance.hurt( fire ? DeathEvent.BURNED : DeathEvent.FROZEN);
    }
}
