using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
/*
 *  Class: GameOverScreen
 *  
 *  Description:
 *  This script manages the HUD.
 *  
 *  Author: Andrea De Seta
*/
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

    //Method found on internet. It allows us to search a Child Component by its name.
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
        this.setLifeCounter(GlobalVariables.PlayerLives);
        this.setGearBonusCounter(PlayerStatistics.instance.bonusGearCount);
        this.setGearCounter(PlayerStatistics.instance.normalGearCount);
        
    }

    //Set the Gear Counter placed in the HUD with a new number.
    public void setGearCounter(int newCount){
        gearCounter.text = "x" + newCount;
    }

    //Set the Gear Bonus Counter placed in the HUD with a new number.
    public void setGearBonusCounter(int newCount){
        gearBonusCounter.text = "x" + newCount;
    }

    //Set the Bomb Counter placed in the HUD with a new number.
    public void setBombCounter(int newCount){
        bombCounter.text = "x" + newCount;
    }

    //Set the Life Counter placed in the HUD with a new number.
    public void setLifeCounter(int newCount){
        lifeCounter.text = "x" + newCount;
    }

    //Update the Battery placed in the HUD.
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

    //Active a Checkpoint image in the middle of the screen. It stays on the screen for 4 seconds.
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
