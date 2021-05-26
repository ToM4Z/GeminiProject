using UnityEngine;

/*
 *  Class: UpdateAudioSourceVolume
 *  
 *  Description:
 *  This script sets the audio volume.
 *  
 *  Author: Thomas Voce
*/
public class UpdateAudioSourceVolume : MonoBehaviour
{
    private void Start()
    {
        UpdateVolume();
    }

    private void UpdateVolume()
    {
        GetComponent<AudioSource>().volume = GlobalVariables.Volume;
    }


    private void Awake()
    {
        Messenger.AddListener(GameEvent.AUDIO_VOLUME_CHANGED, UpdateVolume);
    }

    private void OnDestroy()
    {
        Messenger.RemoveListener(GameEvent.AUDIO_VOLUME_CHANGED, UpdateVolume);
    }
}
