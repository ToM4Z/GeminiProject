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
public class PlayerStatisticsController : MonoBehaviour
{

    #region Singleton

    public static PlayerStatisticsController instance;

    void Awake()
    {
        instance = this;
        Messenger.AddListener(GlobalVariables.RESET, Reset);
    }

    #endregion


    private int hp;
    public int playerLives = 3;
    [SerializeField] private int maxHP;

    public int normalGearCount { get; private set; }
    public int bonusGearCount { get; private set; }
    public int bombCount { get; private set; }

    private PlayerInputModelController playerController;

    void Start()
    {
        hp = maxHP;
        normalGearCount = 0;
        bonusGearCount = 0;
        bombCount = 0;
        playerController = GetComponent<PlayerInputModelController>();
    }

    private void Reset()
    {
        hp = maxHP;
        HUDScript.instance.updateHpBattery(hp);
    }

    public void increaseNormalGear(){
        normalGearCount++;
        HUDScript.instance.setGearCounter(normalGearCount);
        if (normalGearCount == 100){
            playerLives++;
            normalGearCount = 0;
        }
    }

    public void increaseBonusGear(){
        bonusGearCount++;
        HUDScript.instance.setGearBonusCounter(bonusGearCount);
    }

    public void increaseBomb(){
        bombCount++;
        HUDScript.instance.setBombCounter(bombCount);
    }
    public void decreaseBomb()
    {
        bombCount--;
        HUDScript.instance.setBombCounter(bombCount);
    }

    public void increaseLives(){
        playerLives++;
        HUDScript.instance.setLifeCounter(playerLives);
    }
    public void decreaseLives(){
        playerLives--;
        if(playerLives < 0){
            //TODO GAME OVER
        }
        HUDScript.instance.setLifeCounter(playerLives);
    }

    public bool isDeath() { return hp == 0; }

    public int getHP() { return hp; }

    public void increaseHP(){
        hp++;
        HUDScript.instance.updateHpBattery(hp);
    }

    public void hurt(DeathEvent deathEvent, bool fatal = false)
    {
        if (!fatal)
        {
            if (playerController.Invulnerability || isDeath())
                return;

            hp--;
            HUDScript.instance.updateHpBattery(hp);
            if (hp == 0) 
                death(deathEvent);
            else
            {
                print("HP: " + hp);
                playerController.Hurt(deathEvent);
            }

        }
        else
            death(deathEvent);
    }

    private void death(DeathEvent deathEvent)
    {
        hp = 0;
        this.decreaseLives();
        BlackFadeScreen.instance.startFade();
        print("DEATH BY " + deathEvent.ToString());
        Messenger<DeathEvent>.Broadcast(GlobalVariables.DEATH, deathEvent);
        
    }

    private void OnDestroy()
    {
        Messenger.RemoveListener(GlobalVariables.RESET, Reset);
    }
}
