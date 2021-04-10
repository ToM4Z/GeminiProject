using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public int playerLives = 3;
    public int hp = 5;
    public int normalGearCount = 0;
    public int bonusGearCount = 0;
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void increaseNormalGear(){
        normalGearCount++;
        if (normalGearCount == 100){
            playerLives++;
            normalGearCount = 0;
        }
    }

    public void increaseBonusGear(){
        bonusGearCount++;
    }

    public void increaseHP(){
        hp++;
    }

    public void Damage(int dam){

    }
}
