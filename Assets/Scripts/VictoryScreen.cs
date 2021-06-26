using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

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

    //// Update is called once per frame
    //void Update()
    //{
    //    if (Input.GetKeyDown(KeyCode.Joystick1Button0) || Input.GetKeyDown(KeyCode.Return))
    //    {
    //        Button button = EventSystem.current.currentSelectedGameObject.GetComponent<Button>();

    //        if (button != null)
    //            button.onClick.Invoke();
    //    }
    //}

    public void ActiveVictoryScreen(){

        this.gameObject.SetActive(true);
        Cursor.visible = true;
        actived = true;
        Managers.Audio.PlayVictory();
        EventSystem.current.SetSelectedGameObject(null);
        EventSystem.current.SetSelectedGameObject(GameObject.Find("HubButton"));
        StartCoroutine(CountNormalGear());
        StartCoroutine(CountBonusGear());

        int score = (PlayerStatistics.instance.normalGearCountToCalculateScore * GlobalVariables.GearScore) 
            + (PlayerStatistics.instance.bonusGearCount * GlobalVariables.GearBonusScore);

        if (!GlobalVariables.scores.ContainsKey(GlobalVariables.ACTUAL_SCENE) || GlobalVariables.scores[GlobalVariables.ACTUAL_SCENE] < score)
        {
            newRecord.SetActive(true);
            GlobalVariables.scores.Add(GlobalVariables.ACTUAL_SCENE, score);
        }
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
            yield return new WaitForSeconds(0.05f);
        }
    }

    private IEnumerator CountBonusGear(){
        for(int i = 1; i <= PlayerStatistics.instance.bonusGearCount; i++){
            scoreToShow += GlobalVariables.GearBonusScore;
            score.text = scoreToShow + "";
            scoreSfx.Play();
            yield return new WaitForSeconds(0.05f);
        }
    }


}
