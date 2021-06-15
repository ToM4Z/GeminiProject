using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{
    [SerializeField] private GameObject pausePanel;
    [SerializeField] private GameObject optionPanel;
    [SerializeField] private AudioSource clickClip;
    [SerializeField] private AudioSource pauseClip;
    [SerializeField] private AudioSource unpauseClip;
    void Start()
    {
        //Cursor.visible = false;
        pausePanel.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetButtonDown("Cancel")){
            if (GlobalVariables.isPaused)
                Unpause();
            else
                Pause();
        }
    }

    void Pause(){
        pauseClip.Play();
        pausePanel.gameObject.SetActive(true);
        Time.timeScale = 0f;
        Messenger<bool>.Broadcast(GlobalVariables.ENABLE_INPUT, false);
        Messenger<bool>.Broadcast(GlobalVariables.TOGGLE_AUDIO_ON_OFF, false, MessengerMode.DONT_REQUIRE_LISTENER);
        GlobalVariables.isPaused = true;
        //Cursor.visible = true;
    }

    public void Unpause(){
        unpauseClip.Play();
        pausePanel.gameObject.SetActive(false);
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
    }

    public void OptionToPause(){
        optionPanel.SetActive(false);
        pausePanel.SetActive(true);
    }

    public void updateVolume(float v){
        GlobalVariables.Volume = v;
        //Debug.Log(GlobalVariables.Volume);
    }

    public void BackToHub(){
       // SceneManager.LoadScene("Hub"); 
    }

    public void playClick(){
        clickClip.Play();
    }
}
