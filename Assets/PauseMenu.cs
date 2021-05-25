using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PauseMenu : MonoBehaviour
{
    public bool isPaused = false;
    public GameObject pausePanel;
    void Start()
    {
        pausePanel.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetButtonDown("Cancel")){
            if (isPaused){
                Unpause();
            }
            else{
                Pause();
            }
        }
    }

    void Pause(){
        pausePanel.gameObject.SetActive(true);
        Time.timeScale = 0f;
        Messenger<bool>.Broadcast(GameEvent.ENABLE_INPUT, false);
        isPaused = true;
    }

    public void Unpause(){
        pausePanel.gameObject.SetActive(false);
        Time.timeScale = 1f;
        Messenger<bool>.Broadcast(GameEvent.ENABLE_INPUT, true);
        isPaused = false;
    }

    public void QuitGame(){
        Application.Quit();
    }
}
