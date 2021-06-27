using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
/*
 *  Class: Main Menu
 *  
 *  Description:
 *  This script manages the Main Menu GUI.
 *  
 *  Author: Andrea De Seta
*/
public class MainMenu : MonoBehaviour
{
    [SerializeField] private GameObject menuPanel;
    [SerializeField] private GameObject optionPanel;
    [SerializeField] private GameObject videoSettingsPanel;
    [SerializeField] private AudioSource clickClip;

    [SerializeField] private GameObject keyboardMappingImage;
    [SerializeField] private GameObject gamepadMappingImage;
    [SerializeField] private Slider soundSlider, musicSlider;
    [SerializeField] private Button loadGameButton;

    private void Start()
    {
        //Load the config file.
        ConfigSaveSystem.Load();
        Cursor.visible = true;

        //Set the values of the sliders basing on the values volume of the enitre.
        soundSlider.value = GlobalVariables.SoundVolume;
        musicSlider.value = GlobalVariables.MusicVolume;

        //If a save file doesn't exists, the Load Button will be interactable.
        if (!File.Exists(Application.dataPath + "/player.txt"))
        {
            loadGameButton.interactable = false;
        }
    }

    //New game starts a new game. It will overwrite the old saves.
    public void NewGame()
    {
        PlayerSaveSystem.Save();    // resetto il salvataggio
        LevelLoader.instance.LoadLevel(GlobalVariables.HUB_SCENE);
    }

    //Load game loads an existent save file.
    public void LoadGame(){
        PlayerSaveSystem.Load();
        LevelLoader.instance.LoadLevel(GlobalVariables.HUB_SCENE);
    }

    //It tooks us from Menu to Option panel.
    public void MenuToOption(){
        menuPanel.SetActive(false);
        optionPanel.SetActive(true);
        EventSystem.current.SetSelectedGameObject(null);
        EventSystem.current.SetSelectedGameObject(GameObject.Find("Back Button"));
    }

    //It tooks us from Option to Menu panel.
    public void OptionToMenu(){
        ConfigSaveSystem.Save();

        optionPanel.SetActive(false);
        menuPanel.SetActive(true);
        EventSystem.current.SetSelectedGameObject(null);
        EventSystem.current.SetSelectedGameObject(GameObject.Find("OptionButton"));
    }

    //It tooks us from Option to Video Settings panel.
    public void OptionToVideoSettings(){
        optionPanel.SetActive(false);
        videoSettingsPanel.SetActive(true);
        EventSystem.current.SetSelectedGameObject(null);
        EventSystem.current.SetSelectedGameObject(GameObject.Find("Back Button"));
    }

    //It tooks us from Video Settings to Option panel.
    public void VideoSettingsToOption(){
        videoSettingsPanel.SetActive(false);
        optionPanel.SetActive(true);
        EventSystem.current.SetSelectedGameObject(null);
        EventSystem.current.SetSelectedGameObject(GameObject.Find("VideoSettingsButton"));
    }

    //I think it's cleare, I hope...
    public void QuitGame(){
        Application.Quit();
    }

    //Update the value of Sound Volume by its slider
    public void updateVolume(float v){
        GlobalVariables.SoundVolume = v;
    }

    //Update the value of Music Volume by its slider
    public void updateMusicVolume(float v)
    {
        GlobalVariables.MusicVolume = v;
    }

    //It tooks us from Keyboard to Controller mapping.
    public void goToController(){
        keyboardMappingImage.SetActive(false);
        gamepadMappingImage.SetActive(true);
        EventSystem.current.SetSelectedGameObject(null);
        EventSystem.current.SetSelectedGameObject(GameObject.Find("LeftButton"));

        //I have to re edit the navigation of this button because he will disabled when I change mapping screen.
        Navigation customNav = new Navigation();
        customNav.mode = Navigation.Mode.Explicit;
        customNav.selectOnUp = GameObject.Find("LeftButton").GetComponent<Button>();
        customNav.selectOnDown = GameObject.Find("MusicSlider").GetComponent<Slider>();
        GameObject.Find("VolumeSlider").GetComponent<Slider>().navigation = customNav;

    }

    public void goToKeyboard(){
        gamepadMappingImage.SetActive(false);
        keyboardMappingImage.SetActive(true);
        EventSystem.current.SetSelectedGameObject(null);
        EventSystem.current.SetSelectedGameObject(GameObject.Find("RightButton"));

        //Same as above.
        Navigation customNav = new Navigation();
        customNav.mode = Navigation.Mode.Explicit;
        customNav.selectOnUp = GameObject.Find("RightButton").GetComponent<Button>();
        customNav.selectOnDown = GameObject.Find("MusicSlider").GetComponent<Slider>();
        GameObject.Find("VolumeSlider").GetComponent<Slider>().navigation = customNav;
    }

    public void playClick(){
        clickClip.Play();
    }
}
