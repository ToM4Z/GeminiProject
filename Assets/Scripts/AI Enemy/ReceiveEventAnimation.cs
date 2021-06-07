using UnityEngine;

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

}
