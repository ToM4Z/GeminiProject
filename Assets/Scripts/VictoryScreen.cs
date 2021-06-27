using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

/*
 *  Class: Victory Screen
 *  
 *  Description:
 *  This script manages the Victory Screen.
 *  
 *  Author: Andrea De Seta, Thomas Voce
*/

public class VictoryScreen : MonoBehaviour
{
    
    [SerializeField] private Text score;
    [SerializeField] private AudioSource scoreSfx;
    [SerializeField] private AudioSource clickSfx;
    [SerializeField] private GameObject newRecord;
    private int scoreToShow = 0;
    private bool actived = false;
    void Start()
    {
        this.gameObject.SetActive(false);
    }

    public void ActiveVictoryScreen(){

        this.gameObject.SetActive(true);
        Cursor.visible = true;
        actived = true;
        Managers.Audio.PlayVictory();
        //First button selected is the HUB Button. (Andrea)
        EventSystem.current.SetSelectedGameObject(null);
        EventSystem.current.SetSelectedGameObject(GameObject.Find("HubButton"));
        StartCoroutine(CountNormalGear());
        StartCoroutine(CountBonusGear());


        //Calculation a priori of the final score (Thomas)
        int score = (PlayerStatistics.instance.normalGearCountToCalculateScore * GlobalVariables.GearScore) 
            + (PlayerStatistics.instance.bonusGearCount * GlobalVariables.GearBonusScore);

        //If the actual final score is better than last saved final score, a NEW RECORD text will appear. (Thomas)
        if (!GlobalVariables.scores.ContainsKey(GlobalVariables.ACTUAL_SCENE) || GlobalVariables.scores[GlobalVariables.ACTUAL_SCENE] < score)
        {
            newRecord.SetActive(true);
            GlobalVariables.scores.Add(GlobalVariables.ACTUAL_SCENE, score);
        }
    }

    public void PlayClickAudio(){
        clickSfx.Play();
    }

    //It tooks the player back to hub.
    public void BackToHub()
    {
        Time.timeScale = 1f;
        LevelLoader.instance.LoadLevel(GlobalVariables.HUB_SCENE);
    }

    //It tooks the player back to the main menu.
    public void RestartLevel()
    {
        Time.timeScale = 1f;
        LevelLoader.instance.ReloadLevel();
    }

    public bool getActived(){
        return actived;
    }

    //I did a for loop in order to make a paus of 0.05 secs between an iteration and the other in order to
    //make a nice effect when the final score is being calculated in the game. (Andrea)
    private IEnumerator CountNormalGear(){
        for(int i = 1; i <= PlayerStatistics.instance.normalGearCountToCalculateScore; i++){
            scoreToShow += GlobalVariables.GearScore;
            score.text = scoreToShow + "";
            scoreSfx.Play();
            yield return new WaitForSeconds(0.05f);
        }
    }
    
    //Same of above
    private IEnumerator CountBonusGear(){
        for(int i = 1; i <= PlayerStatistics.instance.bonusGearCount; i++){
            scoreToShow += GlobalVariables.GearBonusScore;
            score.text = scoreToShow + "";
            scoreSfx.Play();
            yield return new WaitForSeconds(0.05f);
        }
    }


}
