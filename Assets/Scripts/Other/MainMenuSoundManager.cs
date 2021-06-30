using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 *  Class: MainMenuSoundManager
 *  
 *  Description:
 *  It's used in the main menu to change audio source volume in that scene
 *  
 *  Author: Thomas Voce
*/

public class MainMenuSoundManager : MonoBehaviour
{

    [SerializeField] private AudioSource soundSource;
    [SerializeField] private AudioSource musicSource;

    public void Start()
    {
        musicSource.ignoreListenerVolume = true;
        musicSource.ignoreListenerPause = true;

        UpdateVariables();
    }

    private void UpdateVariables()
    {
        soundVolume = GlobalVariables.SoundVolume;
        musicVolume = GlobalVariables.MusicVolume;
        soundMute = GlobalVariables.SoundMute;
    }
    public float soundVolume
    {
        get { return AudioListener.volume; }
        set { AudioListener.volume = value; }
    }

    public bool soundMute
    {
        get { return AudioListener.pause; }
        set { AudioListener.pause = value; }
    }

    public float musicVolume
    {
        get { return musicSource.volume; }
        set { if (musicSource != null) musicSource.volume = value; }
    }

    private void Awake()
    {
        Messenger.AddListener(GlobalVariables.AUDIO_SETTINGS_CHANGED, UpdateVariables);
    }

    private void OnDestroy()
    {
        Messenger.RemoveListener(GlobalVariables.AUDIO_SETTINGS_CHANGED, UpdateVariables);
    }
}
