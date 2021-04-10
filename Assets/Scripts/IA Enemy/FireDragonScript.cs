using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireDragonScript : MonoBehaviour
{
    ParticleSystem particle;

    void OnEnable()
    {
        particle = GetComponent<ParticleSystem>();
    }

    void OnParticleTrigger()
    {
        int insideCount = particle.GetTriggerParticles(ParticleSystemTriggerEventType.Inside, new List<ParticleSystem.Particle>());

        if (insideCount > 40)
            GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>().Damage(1);
    }
}
