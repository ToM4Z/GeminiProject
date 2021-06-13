using UnityEngine;

public class AudioSourcePauseManager : MonoBehaviour
{

    private void TogglePlayResume(bool isEnabled)
    {
        if(isEnabled)
            foreach (AudioSource sfx in GetComponents<AudioSource>())
                sfx.UnPause();
        else
            foreach (AudioSource sfx in GetComponents<AudioSource>())
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
