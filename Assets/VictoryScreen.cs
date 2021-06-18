using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class VictoryScreen : MonoBehaviour
{
    public static VictoryScreen instance = null;
    void Awake() {
        if (instance == null)
            instance = this;
        else if(instance != this)
            Destroy(gameObject);
    }
    [SerializeField] private Text score;
    [SerializeField] private AudioSource scoreSfx;
    [SerializeField] private AudioSource victorySfx;
    private int scoreToShow = 0;
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
        
        victorySfx.Play();
        StartCoroutine(CountNormalGear());
        StartCoroutine(CountBonusGear());
        
    }

    private IEnumerator CountNormalGear(){
        for(int i = 1; i <= PlayerStatisticsController.instance.normalGearCount; i++){
            scoreToShow += 10;
            score.text = scoreToShow + "";
            scoreSfx.Play();
            yield return new WaitForSeconds(0.3f);
        }
    }

    private IEnumerator CountBonusGear(){
        for(int i = 1; i <= PlayerStatisticsController.instance.bonusGearCount; i++){
            scoreToShow += 50;
            score.text = scoreToShow + "";
            scoreSfx.Play();
            yield return new WaitForSeconds(0.3f);
        }
    }


}
