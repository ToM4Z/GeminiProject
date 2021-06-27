using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 *  Class: UI Manager
 *  
 *  Description:
 *  I created this script to manage all the GUI components in easy way.
 *  
 *  Author: Andrea De Seta
*/

public class UIManager : MonoBehaviour
{
    public static UIManager instance = null;

    void Awake() {
        if (instance == null)
            instance = this;
        else if(instance != this)
            Destroy(gameObject);
    }

    [SerializeField] private GameObject hudPanel;
    private HUDScript hud;

    [SerializeField] private GameObject blackScreenPanel;
    private BlackFadeScreen blackFadeScreen;

    [SerializeField] private GameObject victoryScreenPanel;
    private VictoryScreen victoryScreen;
    [SerializeField] private GameObject gameOverPanel;
    private GameOverScreen gameOverScreen;

    [SerializeField] private GameObject pausePanel;
    private PauseMenu pauseMenu;

    void Start()
    {
        hud = hudPanel.GetComponent<HUDScript>();
        blackFadeScreen = blackScreenPanel.GetComponent<BlackFadeScreen>();
        victoryScreen = victoryScreenPanel.GetComponent<VictoryScreen>();
        gameOverScreen = gameOverPanel.GetComponent<GameOverScreen>();
        pauseMenu = pausePanel.GetComponent<PauseMenu>();

    }

    void Update()
    {
        
    }

    public HUDScript GetHUD(){
        return hud;
    }

    public BlackFadeScreen GetBlackFadeScreen(){
        return blackFadeScreen;
    }

    public VictoryScreen GetVictoryScreen(){
        return victoryScreen;
    }

    public GameOverScreen GetGameOverScreen(){
        return gameOverScreen;
    }

    public PauseMenu GetPauseMenu(){
        return pauseMenu;
    }
}
