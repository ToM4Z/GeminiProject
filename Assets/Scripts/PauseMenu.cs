using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PauseMenu : MonoBehaviour
{
    public GameObject pausePanel;
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
        pausePanel.gameObject.SetActive(true);
        Time.timeScale = 0f;
        Messenger<bool>.Broadcast(GlobalVariables.ENABLE_INPUT, false);
        GlobalVariables.isPaused = true;
        //Cursor.visible = true;
    }

    public void Unpause(){
        pausePanel.gameObject.SetActive(false);
        Time.timeScale = 1f;
        Messenger<bool>.Broadcast(GlobalVariables.ENABLE_INPUT, true);
        GlobalVariables.isPaused = false;
        //Cursor.visible = false;
    }

    public void QuitGame(){
        Application.Quit();
    }
}
