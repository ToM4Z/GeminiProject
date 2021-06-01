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
        Messenger.AddListener(GameEvent.RESET, Reset);
    }

    #endregion


    public int playerLives = 3;
    [SerializeField] private int maxHP;
    private int hp;
    public int normalGearCount {get; private set;}
    public int bonusGearCount { get; private set; }
    public int bombCount { get; private set; }


    private bool invulnerability;
    [SerializeField] private float invulnerabilityTime = 3f;
    private float invulnerabilityTimer = 0f;

    private PlayerMaterialHandler materialHandler;


    void Start()
    {
        materialHandler = GetComponent<PlayerMaterialHandler>();
        hp = maxHP;
        normalGearCount = 0;
        bonusGearCount = 0;
        bombCount = 0;
    }

    private void Reset()
    {
        hp = maxHP;
        HUDManager.instance.updateHpBattery(hp);
    }

    void Update()
    {
        if (invulnerability)
        {
            invulnerabilityTimer -= Time.deltaTime;
            materialHandler.setTransparencyAlpha(Mathf.PingPong(Time.time, 0.5f) / 0.5f);

            if (invulnerabilityTimer < 0f)
            {
                invulnerability = false;
                invulnerabilityTimer = 0f;
                materialHandler.resetMaterials();
            }
        }
    }

    public void increaseNormalGear(){
        normalGearCount++;
        HUDManager.instance.setGearCounter(normalGearCount);
        if (normalGearCount == 100){
            playerLives++;
            normalGearCount = 0;
        }
    }

    public void increaseBonusGear(){
        bonusGearCount++;
        HUDManager.instance.setGearBonusCounter(bonusGearCount);
    }

    public void increaseBomb(){
        bombCount++;
        HUDManager.instance.setBombCounter(bombCount);
    }
    public void decreaseBomb()
    {
        bombCount--;
        HUDManager.instance.setBombCounter(bombCount);
    }

    public void increaseLives(){
        playerLives++;
        HUDManager.instance.setLifeCounter(playerLives);
    }
    public void decreaseLives(){
        playerLives--;
        if(playerLives < 0){
            //TODO GAME OVER
        }
        HUDManager.instance.setLifeCounter(playerLives);
    }

    public bool isDeath() { return hp == 0; }

    public int getHP() { return hp; }

    public void increaseHP(){
        hp++;
        HUDManager.instance.updateHpBattery(hp);
    }

    public void hurt(DeathEvent deathEvent, bool fatal = false)
    {
        if (!fatal)
        {
            if (invulnerability || isDeath())
                return;

            hp--;
            HUDManager.instance.updateHpBattery(hp);
            if (hp == 0) 
                death(deathEvent);
            else
            {
                print("HP: " + hp);
                invulnerability = true;
                invulnerabilityTimer = invulnerabilityTime;

                materialHandler.ToFadeMode();
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
        Messenger<DeathEvent>.Broadcast(GameEvent.DEATH, deathEvent);
        
    }

    private void OnDestroy()
    {
        Messenger.RemoveListener(GameEvent.RESET, Reset);
    }
}
