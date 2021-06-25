using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    [SerializeField] private GameObject menuPanel;
    [SerializeField] private GameObject optionPanel;
    [SerializeField] private GameObject videoSettingsPanel;
    [SerializeField] private AudioSource clickClip;

    [SerializeField] private GameObject keyboardMappingImage;
    [SerializeField] private GameObject gamepadMappingImage;


    public void Play(){
        LevelLoader.instance.LoadLevel(GlobalVariables.HUB_SCENE); 
    }

    public void MenuToOption(){
        menuPanel.SetActive(false);
        optionPanel.SetActive(true);
    }

    public void OptionToMenu(){
        optionPanel.SetActive(false);
        menuPanel.SetActive(true);
    }

    public void OptionToVideoSettings(){
        optionPanel.SetActive(false);
        videoSettingsPanel.SetActive(true);
    }

    public void VideoSettingsToOption(){
        videoSettingsPanel.SetActive(false);
        optionPanel.SetActive(true);
    }

    public void QuitGame(){
        Application.Quit();
    }

    public void updateVolume(float v){
        GlobalVariables.SoundVolume = v;
    }

    public void updateMusicVolume(float v)
    {
        GlobalVariables.MusicVolume = v;
    }

    public void goToController(){
        keyboardMappingImage.SetActive(false);
        gamepadMappingImage.SetActive(true);
    }

    public void goToKeyboard(){
        gamepadMappingImage.SetActive(false);
        keyboardMappingImage.SetActive(true);
    }

    public void playClick(){
        clickClip.Play();
    }
}
