using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    [SerializeField] private GameObject menuPanel;
    [SerializeField] private GameObject optionPanel;
    [SerializeField] private AudioSource clickClip;

    [SerializeField] private GameObject keyboardMappingImage;
    [SerializeField] private GameObject gamepadMappingImage;

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void Play(){
       // SceneManager.LoadScene("Hub"); 
    }

    public void MenuToOption(){
        menuPanel.SetActive(false);
        optionPanel.SetActive(true);
    }

    public void OptionToMenu(){
        optionPanel.SetActive(false);
        menuPanel.SetActive(true);
    }

    public void QuitGame(){
        Application.Quit();
    }

    public void updateVolume(float v){
        Managers.Audio.soundVolume = v;
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
