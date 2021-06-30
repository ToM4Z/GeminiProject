using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

/*
 *  Class: Pause Menu
 *  
 *  Description:
 *  This script manages the Pause Menu GUI.
 *  
 *  Author: Andrea De Seta
*/

public class PauseMenu : MonoBehaviour
{
    [SerializeField] private GameObject pausePanel;
    [SerializeField] private GameObject optionPanel;
    [SerializeField] private GameObject controlsPanel;
    [SerializeField] private AudioSource clickClip;
    [SerializeField] private AudioSource pauseClip;
    [SerializeField] private AudioSource unpauseClip;
    [SerializeField] private GameObject keyboardMappingImage;
    [SerializeField] private GameObject gamepadMappingImage;
    [SerializeField] private Slider soundSlider, musicSlider;

    void Start()
    {
        pausePanel.SetActive(false);

        soundSlider.value = GlobalVariables.SoundVolume;
        musicSlider.value = GlobalVariables.MusicVolume;
    }


    void Update()
    {
        //When I press "ESC", the Pause appears or diseappers
        if(Input.GetButtonDown("Escape") && !GlobalVariables.Win && !GlobalVariables.GameOver){
            if (GlobalVariables.isPaused)
                Unpause();
            else
                Pause();
        }
   
    }

    //Pausing the menu, the time will be setted to 0 and the player will not able to move.
    void Pause(){
        pauseClip.Play();
        pausePanel.gameObject.SetActive(true);
        Time.timeScale = 0f;
        Messenger<bool>.Broadcast(GlobalVariables.ENABLE_INPUT, false);
        Messenger<bool>.Broadcast(GlobalVariables.TOGGLE_AUDIO_ON_OFF, false, MessengerMode.DONT_REQUIRE_LISTENER);
        GlobalVariables.isPaused = true;
        EventSystem.current.SetSelectedGameObject(null);
        EventSystem.current.SetSelectedGameObject(GameObject.Find("ResumeButton"));
        Cursor.visible = true;
    }

    //Pausing the menu, the time will be setted to 1 and the player can move again.
    public void Unpause(){
        unpauseClip.Play();
        pausePanel.gameObject.SetActive(false);
        optionPanel.gameObject.SetActive(false);
        controlsPanel.gameObject.SetActive(false);
        Time.timeScale = 1f;
        Messenger<bool>.Broadcast(GlobalVariables.ENABLE_INPUT, true);
        Messenger<bool>.Broadcast(GlobalVariables.TOGGLE_AUDIO_ON_OFF, true, MessengerMode.DONT_REQUIRE_LISTENER);
        GlobalVariables.isPaused = false;
        ConfigSaveSystem.Save();        // save configs because the player could modify volumes
        Cursor.visible = false;
    }

    public void QuitGame(){
        Application.Quit();
    }

    //It tooks us from Pause to Option panel.
    public void PauseToOption(){
        pausePanel.SetActive(false);
        optionPanel.SetActive(true);
        EventSystem.current.SetSelectedGameObject(null);
        EventSystem.current.SetSelectedGameObject(GameObject.Find("Back Button"));
    }

    //It tooks us from Option to Pause panel.
    public void OptionToPause(){
        optionPanel.SetActive(false);
        pausePanel.SetActive(true);
        EventSystem.current.SetSelectedGameObject(null);
        EventSystem.current.SetSelectedGameObject(GameObject.Find("ResumeButton"));
        ConfigSaveSystem.Save();
    }

    //It tooks us from Option to Controls panel.
    public void OptionToControls(){
        optionPanel.SetActive(false);
        controlsPanel.SetActive(true);
        EventSystem.current.SetSelectedGameObject(null);
        EventSystem.current.SetSelectedGameObject(GameObject.Find("Back Button"));
    }

    //It tooks us from Controls to Option panel.
    public void ControlsToOption(){
        controlsPanel.SetActive(false);
        optionPanel.SetActive(true);
        EventSystem.current.SetSelectedGameObject(null);
        EventSystem.current.SetSelectedGameObject(GameObject.Find("Controls Button"));
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

    public void BackToHub()
    {
        unpauseClip.Play();
        Time.timeScale = 1f;
        LevelLoader.instance.LoadLevel(GlobalVariables.HUB_SCENE);
    }

    public void BackToMainMenu()
    {
        unpauseClip.Play();
        Time.timeScale = 1f;
        LevelLoader.instance.LoadLevel(GlobalVariables.MAIN_MENU_SCENE);
    }

    public void goToController(){
        keyboardMappingImage.SetActive(false);
        gamepadMappingImage.SetActive(true);
        EventSystem.current.SetSelectedGameObject(null);
        EventSystem.current.SetSelectedGameObject(GameObject.Find("LeftButton"));
        //I have to re edit the navigation of this button because he will disabled when I change mapping screen.
        Navigation customNav = new Navigation();
        customNav.mode = Navigation.Mode.Explicit;
        customNav.selectOnLeft = GameObject.Find("LeftButton").GetComponent<Button>();
        GameObject.Find("Back Button").GetComponent<Button>().navigation = customNav;
    }

    public void goToKeyboard(){
        gamepadMappingImage.SetActive(false);
        keyboardMappingImage.SetActive(true);
        EventSystem.current.SetSelectedGameObject(null);
        EventSystem.current.SetSelectedGameObject(GameObject.Find("RightButton"));
        //Same as above.
        Navigation customNav = new Navigation();
        customNav.mode = Navigation.Mode.Explicit;
        customNav.selectOnRight = GameObject.Find("RightButton").GetComponent<Button>();
        GameObject.Find("Back Button").GetComponent<Button>().navigation = customNav;
    }

    public void playClick(){
        clickClip.Play();
    }
}
