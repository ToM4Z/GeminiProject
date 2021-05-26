using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour, IGameManager
{
    public ManagerStatus status { get; private set; }


    public void Startup()
    {
        status = ManagerStatus.Started;
    }

    // This method create a smoothly fade out of the sound
    public static IEnumerator FadeOut(AudioSource audioSource, float FadeTime)
    {
        float startVolume = audioSource.volume;
        float adjustedVolume = startVolume;

        while (adjustedVolume > 0)
        {
            adjustedVolume -= startVolume * Time.deltaTime / FadeTime;
            audioSource.volume = adjustedVolume;
            //Debug.Log(adjustedVolume);
            yield return null;
        }

        audioSource.Stop();
        audioSource.volume = startVolume;
    }
}
