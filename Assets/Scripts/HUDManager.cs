using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HUDManager : MonoBehaviour
{

    public static HUDManager instance = null;

    void Awake() {
        if (instance == null)
            instance = this;
        else if(instance != this)
            Destroy(gameObject);
    }


    Text gearCounter;
    Text gearBonusCounter;
    Text bombCounter;
    Text lifeCounter;

    Image hpBattery;

    Sprite greenHP, orangeHP, redHP;

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

        greenHP = Resources.Load<Sprite>("HUD/Green_HP");
        orangeHP = Resources.Load<Sprite>("HUD/Orange_HP");
        redHP = Resources.Load<Sprite>("HUD/Red_HP");

        this.setBombCounter(PlayerStatisticsController.instance.bombCount);
        this.setLifeCounter(PlayerStatisticsController.instance.playerLives);
        this.setGearBonusCounter(PlayerStatisticsController.instance.bonusGearCount);
        this.setGearCounter(PlayerStatisticsController.instance.normalGearCount);
        
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
        
    }
}
