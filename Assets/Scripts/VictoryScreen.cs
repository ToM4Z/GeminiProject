using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class VictoryScreen : MonoBehaviour
{
    
    [SerializeField] private Text score;
    [SerializeField] private AudioSource scoreSfx;
    [SerializeField] private AudioSource clickSfx;
    private int scoreToShow = 0;
    private bool actived = false;
    void Start()
    {
        this.gameObject.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void ActiveVictoryScreen(){

        this.gameObject.SetActive(true);
        actived = true;
        Managers.Audio.PlayVictory();
        StartCoroutine(CountNormalGear());
        StartCoroutine(CountBonusGear());

        int score = (PlayerStatistics.instance.normalGearCountToCalculateScore * GlobalVariables.GearScore) 
            + (PlayerStatistics.instance.bonusGearCount * GlobalVariables.GearBonusScore);

        if (!GlobalVariables.scores.ContainsKey(GlobalVariables.ACTUAL_SCENE))
        {
            GlobalVariables.scores.Add(GlobalVariables.ACTUAL_SCENE, score);
        }
        else
            if (GlobalVariables.scores[GlobalVariables.ACTUAL_SCENE] < score)
        {
            // new record
            GlobalVariables.scores.Add(GlobalVariables.ACTUAL_SCENE, score);
        }

        string scoress = "";
        foreach(KeyValuePair<int, int> item in GlobalVariables.scores) {
            scoress += item.Key + "(" + item.Value + ") ";
        }
        print(scoress);

    }

    public void PlayClickAudio(){
        clickSfx.Play();
    }

    public void BackToHub()
    {
        Time.timeScale = 1f;
        LevelLoader.instance.LoadLevel(GlobalVariables.HUB_SCENE);
    }

    public void RestartLevel()
    {
        Time.timeScale = 1f;
        LevelLoader.instance.ReloadLevel();
    }

    public bool getActived(){
        return actived;
    }

    private IEnumerator CountNormalGear(){
        for(int i = 1; i <= PlayerStatistics.instance.normalGearCountToCalculateScore; i++){
            scoreToShow += GlobalVariables.GearScore;
            score.text = scoreToShow + "";
            scoreSfx.Play();
            yield return new WaitForSeconds(0.3f);
        }
    }

    private IEnumerator CountBonusGear(){
        for(int i = 1; i <= PlayerStatistics.instance.bonusGearCount; i++){
            scoreToShow += GlobalVariables.GearBonusScore;
            score.text = scoreToShow + "";
            scoreSfx.Play();
            yield return new WaitForSeconds(0.3f);
        }
    }


}
