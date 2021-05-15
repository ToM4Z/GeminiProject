using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
    public int normalGearCount = 0;
    public int bonusGearCount = 0;
    public int bombCount = 0;


    private bool invulnerability;
    [SerializeField] private float invulnerabilityTime = 3f;
    private float invulnerabilityTimer = 0f;

    private PlayerMaterialHandler materialHandler;

    private HUDManager hud;

    void Start()
    {
        materialHandler = GetComponent<PlayerMaterialHandler>();
        hud = transform.parent.gameObject.GetComponentInChildren<HUDManager>();
        hp = maxHP;
    }

    private void Reset()
    {
        hp = maxHP;
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
        hud.setGearCounter(normalGearCount);
        if (normalGearCount == 100){
            playerLives++;
            normalGearCount = 0;
        }
    }

    public void increaseBonusGear(){
        bonusGearCount++;
        hud.setGearBonusCounter(bonusGearCount);
    }

    public void increaseBomb(){
        bombCount++;
        hud.setBombCounter(bombCount);
    }

    public bool isDeath() { return hp == 0; }

    public int getHP() { return hp; }

    public void increaseHP(){
        hp++;
    }

    public void hurt(DeathEvent deathEvent, bool fatal = false)
    {
        if (!fatal)
        {
            if (invulnerability || isDeath())
                return;

            hp--;
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
        print("DEATH BY " + deathEvent.ToString());
        Messenger<DeathEvent>.Broadcast(GameEvent.DEATH, deathEvent);
    }

    private void OnDestroy()
    {
        Messenger.RemoveListener(GameEvent.RESET, Reset);
    }
}
