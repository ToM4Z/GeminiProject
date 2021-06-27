using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour, IGameManager
{
    public ManagerStatus status { get; private set; }

    [SerializeField] private AudioSource soundSource;
    [SerializeField] private AudioSource musicSource;

    [SerializeField] private AudioClip tinSFX;
    private AudioClip GameOverMusic, VictoryMusic, LevelMusic;

    public void Startup()
    {
        musicSource.ignoreListenerVolume = true;
        musicSource.ignoreListenerPause = true;

        UpdateVariables();

        PlayLevelMusic();

        status = ManagerStatus.Started;
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

    public bool musicMute
    {
        get
        {
            if (musicSource != null)
                return musicSource.mute;
            return false;
        }
        set { if (musicSource != null) musicSource.mute = value; }
    }

    public void PlayClip(AudioClip clip)
    {
        soundSource.PlayOneShot(clip);
    }

    private void PlayMusic(AudioClip clip)
    {
        musicSource.clip = clip;
        musicSource.Play();
    }

    public void PlayLevelMusic()
    {
        PlayMusic(LevelMusic == null ? LevelMusic = (AudioClip)Resources.Load(GlobalVariables.GetLevelMusic()) : LevelMusic);
    }

    public void PlayGameOver()
    {
        musicSource.loop = false;
        PlayMusic(GameOverMusic == null ? GameOverMusic = (AudioClip)Resources.Load(GlobalVariables.GAMEOVER_MUSIC) : GameOverMusic);
    }

    public void PlayVictory()
    {
        musicSource.loop = false;
        PlayMusic(VictoryMusic == null ? VictoryMusic = (AudioClip)Resources.Load(GlobalVariables.VICTORY_MUSIC) : VictoryMusic);
    }

    public void StopMusic()
    {
        musicSource.Stop();
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

    // da testare
    public static IEnumerator FadeIn(AudioSource audioSource, float FadeTime)
    {
        float originVolume = audioSource.volume;
        float startVolume = 0.1f;

        audioSource.volume = 0;
        audioSource.Play();

        while (audioSource.volume < originVolume)
        {
            audioSource.volume += startVolume * Time.deltaTime / FadeTime;

            yield return null;
        }

        audioSource.volume = originVolume;
    }

    public void PlayTin()
    {
        PlayClip(tinSFX);
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
