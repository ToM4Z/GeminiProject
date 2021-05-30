using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReceiveEventAnimation : MonoBehaviour
{
    [SerializeField] private AIEnemy enemy;

    public void PlayWalkSound()
    {
        enemy.PlayWalkSound();
    }

    public void PlayAttackSound()
    {
        enemy.PlayAttackSound();
    }
}
