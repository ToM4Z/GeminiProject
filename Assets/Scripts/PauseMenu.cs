using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

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
        //Cursor.visible = false;
        pausePanel.SetActive(false);

        soundSlider.value = GlobalVariables.SoundVolume;
        musicSlider.value = GlobalVariables.MusicVolume;
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetButtonDown("Escape") && !GlobalVariables.Win && !GlobalVariables.GameOver){
            if (GlobalVariables.isPaused)
                Unpause();
            else
                Pause();
        }

        if (GlobalVariables.isPaused && (Input.GetButtonDown("Submit") || Input.GetKeyDown(KeyCode.Return)))
        {
            Button button = EventSystem.current.currentSelectedGameObject.GetComponent<Button>();

            if (button != null)
                button.onClick.Invoke();

        }
    }

    void Pause(){
        pauseClip.Play();
        pausePanel.gameObject.SetActive(true);
        Time.timeScale = 0f;
        Messenger<bool>.Broadcast(GlobalVariables.ENABLE_INPUT, false);
        Messenger<bool>.Broadcast(GlobalVariables.TOGGLE_AUDIO_ON_OFF, false, MessengerMode.DONT_REQUIRE_LISTENER);
        GlobalVariables.isPaused = true;
        EventSystem.current.SetSelectedGameObject(null);
        EventSystem.current.SetSelectedGameObject(GameObject.Find("ResumeButton"));
        //Cursor.visible = true;
    }

    public void Unpause(){
        unpauseClip.Play();
        pausePanel.gameObject.SetActive(false);
        optionPanel.gameObject.SetActive(false);
        controlsPanel.gameObject.SetActive(false);
        Time.timeScale = 1f;
        Messenger<bool>.Broadcast(GlobalVariables.ENABLE_INPUT, true);
        Messenger<bool>.Broadcast(GlobalVariables.TOGGLE_AUDIO_ON_OFF, true, MessengerMode.DONT_REQUIRE_LISTENER);
        GlobalVariables.isPaused = false;
        //Cursor.visible = false;
    }

    public void QuitGame(){
        Application.Quit();
    }

    public void PauseToOption(){
        pausePanel.SetActive(false);
        optionPanel.SetActive(true);
        EventSystem.current.SetSelectedGameObject(null);
        EventSystem.current.SetSelectedGameObject(GameObject.Find("Back Button"));
    }

    public void OptionToPause(){
        optionPanel.SetActive(false);
        pausePanel.SetActive(true);
        EventSystem.current.SetSelectedGameObject(null);
        EventSystem.current.SetSelectedGameObject(GameObject.Find("ResumeButton"));
    }
    public void OptionToControls(){
        optionPanel.SetActive(false);
        controlsPanel.SetActive(true);
        EventSystem.current.SetSelectedGameObject(null);
        EventSystem.current.SetSelectedGameObject(GameObject.Find("Back Button"));
    }
    public void ControlsToOption(){
        controlsPanel.SetActive(false);
        optionPanel.SetActive(true);
        EventSystem.current.SetSelectedGameObject(null);
        EventSystem.current.SetSelectedGameObject(GameObject.Find("Controls Button"));
    }

    public void updateVolume(float v){
        GlobalVariables.SoundVolume = v;
    }

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

        Navigation customNav = new Navigation();
        customNav.mode = Navigation.Mode.Explicit;
        customNav.selectOnRight = GameObject.Find("RightButton").GetComponent<Button>();
        GameObject.Find("Back Button").GetComponent<Button>().navigation = customNav;
    }

    public void playClick(){
        clickClip.Play();
    }
}
