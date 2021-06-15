using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    [SerializeField] private GameObject menuPanel;
    [SerializeField] private GameObject optionPanel;
    [SerializeField] private AudioSource clickClip;

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
        GlobalVariables.Volume = v;
       // Debug.Log(GlobalVariables.Volume);
    }

        public void playClick(){
        clickClip.Play();
    }
}
