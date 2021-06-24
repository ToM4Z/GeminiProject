using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 *  Class: PlayerStatisticsController
 *  
 *  Description:
 *  This script contains the player'statistics, the number of objects, and manage the invulnerability time.
 *  
 *  Author: Thomas Voce, Andrea De Seta
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


    private int hp;
    [SerializeField] private int maxHP;

    public int normalGearCount;

    //This variable is used to calculate the score, because the normal count will be setted to 0 when we have
    //100 of them and so we will avoid to calculate the final score with 0.
    public int normalGearCountToCalculateScore { get; private set; }
    public int bonusGearCount;
    public int bombCount { get; private set; }

    private PlayerController playerController;

    void Start()
    {
        hp = maxHP;
        normalGearCount = 0;
        normalGearCountToCalculateScore = 0;
        bonusGearCount = 0;
        bombCount = 0;
        playerController = GetComponent<PlayerController>();
    }

    private void Reset()
    {
        hp = maxHP;
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
        if(hp + 1 <= maxHP)
        {
            hp++;
            UIManager.instance.GetHUD().updateHpBattery(hp);
        }
    }

    public void hurt(DeathEvent deathEvent, bool fatal = false)
    {
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
                print("HP: " + hp);
                playerController.Hurt(deathEvent);
            }

        }
        else if(hp > 0)
            death(deathEvent);
    }

    private void death(DeathEvent deathEvent)
    {
        hp = 0;
        UIManager.instance.GetHUD().updateHpBattery(hp);
        if (GlobalVariables.PlayerLives > 0)
            decreaseLives();
        else
            GlobalVariables.PlayerLives--;
        print("DEATH BY " + deathEvent.ToString());
        Messenger<DeathEvent>.Broadcast(GlobalVariables.DEATH, deathEvent);
        
    }

    private void OnDestroy()
    {
        Messenger.RemoveListener(GlobalVariables.RESET, Reset);
    }
}
