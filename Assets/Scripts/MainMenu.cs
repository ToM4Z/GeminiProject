using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    [SerializeField] private GameObject menuPanel;
    [SerializeField] private GameObject optionPanel;
    [SerializeField] private GameObject videoSettingsPanel;
    [SerializeField] private AudioSource clickClip;

    [SerializeField] private GameObject keyboardMappingImage;
    [SerializeField] private GameObject gamepadMappingImage;

    private void Update() {

        if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.Joystick1Button0))
        {
            Button button = EventSystem.current.currentSelectedGameObject.GetComponent<Button>();

            if (button != null)
                button.onClick.Invoke();
        }

    }

    public void Play(){
        LevelLoader.instance.LoadLevel(GlobalVariables.HUB_SCENE);
    }

    public void MenuToOption(){

        menuPanel.SetActive(false);
        optionPanel.SetActive(true);
        EventSystem.current.SetSelectedGameObject(null);
        EventSystem.current.SetSelectedGameObject(GameObject.Find("Back Button"));
        
    }

    public void OptionToMenu(){
        optionPanel.SetActive(false);
        menuPanel.SetActive(true);
        EventSystem.current.SetSelectedGameObject(null);
        EventSystem.current.SetSelectedGameObject(GameObject.Find("OptionButton"));
    }

    public void OptionToVideoSettings(){
        optionPanel.SetActive(false);
        videoSettingsPanel.SetActive(true);
        EventSystem.current.SetSelectedGameObject(null);
        EventSystem.current.SetSelectedGameObject(GameObject.Find("Back Button"));
    }

    public void VideoSettingsToOption(){
        videoSettingsPanel.SetActive(false);
        optionPanel.SetActive(true);
        EventSystem.current.SetSelectedGameObject(null);
        EventSystem.current.SetSelectedGameObject(GameObject.Find("VideoSettingsButton"));
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
        EventSystem.current.SetSelectedGameObject(null);
        EventSystem.current.SetSelectedGameObject(GameObject.Find("LeftButton"));

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
