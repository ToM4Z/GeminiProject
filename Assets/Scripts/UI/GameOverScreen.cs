using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

/*
 *  Class: GameOverScreen
 *  
 *  Description:
 *  This script manages the Game Over Screen.
 *  
 *  Author: Andrea De Seta
*/

public class GameOverScreen : MonoBehaviour
{
    public static GameOverScreen instance = null;
    void Awake() {
        if (instance == null)
            instance = this;
        else if(instance != this)
            Destroy(gameObject);
    }

    [SerializeField] private AudioSource clickSfx;
    [SerializeField] private GameObject gameOverPanel;
    private bool actived = false;

    void Start()
    {
        this.gameObject.SetActive(false);
        gameOverPanel.SetActive(false);
    }

    //This method activates the Game Over Screen routine
    public void ActiveGameOverScreen(){        
        actived = true;
        this.gameObject.SetActive(true);
        Managers.Audio.PlayGameOver();
        StartCoroutine(WaitForThePianoSound());
    }

    //The effective panel that handle the Game Over screen.
    //It will appear after 7 seconds to make a better effect with the background music.
    private IEnumerator WaitForThePianoSound(){
        yield return new WaitForSeconds(7.0f);
        Time.timeScale = 0;
        Cursor.visible = true;
        gameOverPanel.SetActive(true);
        EventSystem.current.SetSelectedGameObject(null);
        EventSystem.current.SetSelectedGameObject(GameObject.Find("HubButton"));
    }

    public void PlayClickAudio(){
        clickSfx.Play();
    }

    //It tooks the player back to hub.
    public void BackToHub()
    {
        Time.timeScale = 1f;
        GlobalVariables.PlayerLives = GlobalVariables.PlayerLivesToReset;
        LevelLoader.instance.LoadLevel(GlobalVariables.HUB_SCENE);
    }

    //It tooks the player back to the main menu.
    public void RestartLevel()
    {
        Time.timeScale = 1f;
        GlobalVariables.PlayerLives = GlobalVariables.PlayerLivesToReset;
        LevelLoader.instance.ReloadLevel();
    }
    public bool getActived(){
        return actived;
    }
}
