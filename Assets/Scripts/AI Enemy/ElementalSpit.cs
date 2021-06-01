using System.Collections.Generic;
using UnityEngine;

/*
 *  Class: ElementalSpit
 *  
 *  Description:
 *  This script checks that when the particles are triggered by player, then I hit the player.
 *  
 *  Author: Thomas Voce
*/
public class ElementalSpit : MonoBehaviour
{
    ParticleSystem particle;

    enum Element
    {
        FIRE, ICE, ELECTRIC
    }

    [SerializeField]
    private Element element;

    [SerializeField] private int numberParticleTrigger;

    void Start()
    {
        particle = GetComponent<ParticleSystem>();
        particle.trigger.AddCollider(PlayerStatisticsController.instance.GetComponent<Collider>());
    }

    // there is no need to check if the gameobject that triggered the particles is the player
    // since it's specified in ParticleSystem's Trigger module
    void OnParticleTrigger()
    {
        int insideCount = particle.GetTriggerParticles(ParticleSystemTriggerEventType.Inside, new List<ParticleSystem.Particle>());

        if (insideCount >= numberParticleTrigger)
            PlayerStatisticsController.instance.hurt( 
                element == Element.FIRE ? 
                    DeathEvent.BURNED
                :
                element == Element.ICE ?                    
                    DeathEvent.FROZEN
                :
                    DeathEvent.BURNED);
    }
}
