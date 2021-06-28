using UnityEngine;

/*
 *  Class: ReceiveEventAnimation
 *  
 *  Description:
 *  If a script is not placed on the gameobject where is placed a animator, 
 *  that script cannot receive the events animation.
 *  
 *  This script allow to forward that event on the parent script.
 *  
 *  This script is used by player and enemies. 
 *  
 *  Author: Thomas Voce
*/

public class ReceiveEventAnimation : MonoBehaviour
{
    private GameObject go;

    private void Start()
    {
        go = this.transform.parent.gameObject;
    }

    public void PlaySpawnSound()
    {
        go.SendMessage("PlaySpawnSound");
    }

    public void PlayIdleSound()
    {
        go.SendMessage("PlayIdleSound");
    }

    public void PlayWalkSound()
    {
        go.SendMessage("PlayWalkSound");
    }

    public void PlayAttackSound()
    {
        go.SendMessage("PlayAttackSound");
    }

    public void PlayDeathSound()
    {
        go.SendMessage("PlayDeathSound");
    }

    public void StartDespawn()
    {
        go.SendMessage("StartDespawn");
    }

    public void Disable()
    {
        go.SendMessage("Disable");
    }

    public void StopSlide()
    {
        go.SendMessage("StopSlide");
    }

}
