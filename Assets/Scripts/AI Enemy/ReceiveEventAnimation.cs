using UnityEngine;

public class ReceiveEventAnimation : MonoBehaviour
{
    private AIEnemy enemy;

    private void Start()
    {
        enemy = GetComponentInParent<AIEnemy>();
    }

    public void PlaySpawnSound()
    {
        enemy.PlaySpawnSound();
    }

    public void PlayIdleSound()
    {
        enemy.PlayIdleSound();
    }

    public void PlayWalkSound()
    {
        enemy.PlayWalkSound();
    }

    public void PlayAttackSound()
    {
        enemy.PlayAttackSound();
    }

}
