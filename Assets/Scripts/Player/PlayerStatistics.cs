using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 *  Class: PlayerStatisticsController
 *  
 *  Description:
 *  This script contains the player's statistics, the number of objects
 *  All variables contained here, are showed by HUD
 *  This script is also used to access quickly with singleton to player
 *  
 *  Author: Thomas Voce (hp, lives, hurt and death method) 
 *          Andrea De Seta (collectables managing)
*/
public class PlayerStatistics : MonoBehaviour
{

    #region Singleton

    public static PlayerStatistics instance;

    void Awake()
    {
        instance = this;
        Messenger.AddListener(GlobalVariables.RESET, Reset);
    }

    #endregion

    // actual hp
    private int hp;

    // counter of gears
    public int normalGearCount;

    //This variable is used to calculate the score, because the normal count will be setted to 0 when we have
    //100 of them and so we will avoid to calculate the final score with 0.
    public int normalGearCountToCalculateScore;

    // counter of gear bonus
    public int bonusGearCount;

    // counter of bombs
    public int bombCount;

    private PlayerController playerController;

    void Start()
    {
        hp = GlobalVariables.PlayerHPToReset;
        normalGearCount = 0;
        normalGearCountToCalculateScore = 0;
        bonusGearCount = 0;
        bombCount = 0;
        playerController = GetComponent<PlayerController>();
    }

    // when player respawn, I reset HP and update HUD (T)
    private void Reset()
    {
        hp = GlobalVariables.PlayerHPToReset;
        UIManager.instance.GetHUD().updateHpBattery(hp);
    }

    public void increaseNormalGear(){
        normalGearCountToCalculateScore++;
        normalGearCount++;
        UIManager.instance.GetHUD().setGearCounter(normalGearCount);
        if (normalGearCount == 100){
            GlobalVariables.PlayerLives++;
            normalGearCount = 0;
        }
    }

    public void increaseBonusGear(){
        bonusGearCount++;
        UIManager.instance.GetHUD().setGearBonusCounter(bonusGearCount);
    }

    public void increaseBomb(){
        bombCount++;
        UIManager.instance.GetHUD().setBombCounter(bombCount);
    }
    public void decreaseBomb()
    {
        bombCount--;
        UIManager.instance.GetHUD().setBombCounter(bombCount);
    }

    public void increaseLives(){
        GlobalVariables.PlayerLives++;
        UIManager.instance.GetHUD().setLifeCounter(GlobalVariables.PlayerLives);
    }
    public void decreaseLives(){
        GlobalVariables.PlayerLives--;
        UIManager.instance.GetHUD().setLifeCounter(GlobalVariables.PlayerLives);
    }

    public bool isDeath() { return hp == 0; }

    public int getHP() { return hp; }

    public void increaseHP(){
        if(hp + 1 <= GlobalVariables.PlayerHPToReset)
        {
            hp++;
            UIManager.instance.GetHUD().updateHpBattery(hp);
        }
    }

    // if somebody calls this method with fatal = true, kill player instantly (T)
    // otherwise, remove one HP and if HP = 0, player die
    // before this, If I'm changing scene, I cannot receive damage
    public void hurt(DeathEvent deathEvent, bool fatal = false)
    {
        if (LevelLoader.instance.isChangingScene)
            return;

        if (!fatal)
        {
            if (playerController.Invulnerability || isDeath())
                return;

            hp--;
            UIManager.instance.GetHUD().updateHpBattery(hp);
            if (hp == 0) 
                death(deathEvent);
            else
            {
                //print("HP: " + hp);
                playerController.Hurt(deathEvent);
            }

        }
        else if(hp > 0)
            death(deathEvent);
    }

    // when player die (T)
    private void death(DeathEvent deathEvent)
    {
        hp = 0;
        UIManager.instance.GetHUD().updateHpBattery(hp);
        if (GlobalVariables.PlayerLives > 0)        // if Lives > 0, decrease one Life and update HUD (T)
            decreaseLives();
        else
            GlobalVariables.PlayerLives--;          // otherwise, this was the last life, so I decrease one life and send DEATH message, so the respawn manager can activate gameover screen (T)
        //print("DEATH BY " + deathEvent.ToString());
        Messenger<DeathEvent>.Broadcast(GlobalVariables.DEATH, deathEvent);
        
    }

    private void OnDestroy()
    {
        Messenger.RemoveListener(GlobalVariables.RESET, Reset);
    }
}
