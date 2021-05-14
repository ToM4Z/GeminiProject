using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HUDManager : MonoBehaviour
{
    Text gearCounter;
    Text gearBonusCounter;
    Text bombCounter;

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
        gearBonusCounter = this.GetChildComponentByName<Text>("GearBonusCounter");
        bombCounter = this.GetChildComponentByName<Text>("BombCounter");
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
}
