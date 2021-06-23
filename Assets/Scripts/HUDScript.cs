using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HUDScript : MonoBehaviour
{

    Text gearCounter;
    Text gearBonusCounter;
    Text bombCounter;
    Text lifeCounter;

    Image hpBattery;
    [SerializeField] private GameObject Checkpoint;

    AudioSource checkpointSFX;

    Sprite greenHP, orangeHP, redHP, emptyHP;

    private T GetChildComponentByName<T>(string name) where T : Component {
        foreach (T component in GetComponentsInChildren<T>(true)) {
            if (component.gameObject.name == name) {
                return component;
            }
        }
        return null;
    }
    void Start()
    {
        gearCounter = this.GetChildComponentByName<Text>("GearCounter");
        lifeCounter = this.GetChildComponentByName<Text>("LifeCounter");
        gearBonusCounter = this.GetChildComponentByName<Text>("GearBonusCounter");
        bombCounter = this.GetChildComponentByName<Text>("BombCounter");
        hpBattery = this.GetChildComponentByName<Image>("HP");
        checkpointSFX = this.GetChildComponentByName<AudioSource>("Checkpoint Audio");

        Checkpoint.SetActive(false);

        greenHP = Resources.Load<Sprite>("HUD/Green_HP");
        orangeHP = Resources.Load<Sprite>("HUD/Orange_HP");
        redHP = Resources.Load<Sprite>("HUD/Red_HP");
        emptyHP = Resources.Load<Sprite>("HUD/Empty_HP");

        this.setBombCounter(PlayerStatistics.instance.bombCount);
        this.setLifeCounter(PlayerStatistics.instance.lives);
        this.setGearBonusCounter(PlayerStatistics.instance.bonusGearCount);
        this.setGearCounter(PlayerStatistics.instance.normalGearCount);
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void setGearCounter(int newCount){
        gearCounter.text = "x" + newCount;
    }

    public void setGearBonusCounter(int newCount){
        gearBonusCounter.text = "x" + newCount;
    }

    public void setBombCounter(int newCount){
        bombCounter.text = "x" + newCount;
    }

    public void setLifeCounter(int newCount){
        lifeCounter.text = "x" + newCount;
    }

    public void updateHpBattery(int hp){
        
        if(hp == 3){
            hpBattery.sprite = greenHP;
        }
        else if(hp == 2){
            hpBattery.sprite = orangeHP;
        }
        else if(hp == 1){
            hpBattery.sprite = redHP;
        }
        else if(hp == 0){
            hpBattery.sprite = emptyHP;
        }
        
    }

    public void ActivateCheckpointImage(){
        Checkpoint.SetActive(true);
        checkpointSFX.Play();
        StartCoroutine(CheckpointOnScreen());
    }

    private IEnumerator CheckpointOnScreen(){
        yield return new WaitForSeconds(4.0f);
        Checkpoint.SetActive(false);
    }
}
