using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReceiveEventAnimation : MonoBehaviour
{
    private AIEnemy enemy;

    private void Start()
    {
        enemy = GetComponentInParent<AIEnemy>();
    }

    public void PlayWalkSound()
    {
        enemy.PlayWalkSound();
    }

    public void PlayAttackSound()
    {
        enemy.PlayAttackSound();
    }

    public void PlayIdleSound()
    {
        enemy.PlayWalkSound();
    }
}
