using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * This script allow to pause and unpause all audiosource in a go when player pause the game.
 * 
 * Author: Thomas Voce 
 */
public class AudioSourcePauseManager : MonoBehaviour
{

    AudioSource[] sources;

    private void Start()
    {
        sources = GetComponents<AudioSource>();
    }

    private void TogglePlayResume(bool isEnabled)
    {
        if(isEnabled)
            foreach (AudioSource sfx in sources)
                sfx.UnPause();
        else
            foreach (AudioSource sfx in sources)
                sfx.Pause();
    }


    void Awake()
    {
        Messenger<bool>.AddListener(GlobalVariables.TOGGLE_AUDIO_ON_OFF, TogglePlayResume);
    }

    void OnDestroy()
    {
        Messenger<bool>.RemoveListener(GlobalVariables.TOGGLE_AUDIO_ON_OFF, TogglePlayResume);
    }
}
