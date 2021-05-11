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
    public int maxHP = 5;
    public int hp = 5;
    public int normalGearCount = 0;
    public int bonusGearCount = 0;

    [SerializeField]
    private GameObject followTarget;

    private bool invulnerability;
    private float invulnerabilityTime = 3f;
    private float invulnerabilityTimer = 0f;

    private PlayerMaterialHandler materialHandler;

    void Start()
    {
        materialHandler = GetComponent<PlayerMaterialHandler>();
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

    public void hurt(DeathEvent deathEvent, bool fatal = false)
    {
        if (hp == 0)
            return;

        if (!fatal)
        {
            if (invulnerability)
                return;

            hp--;
            if (hp == 0)
            {
                Messenger<DeathEvent>.Broadcast(GameEvent.DEATH, deathEvent);

                switch (deathEvent)
                {
                    case DeathEvent.BURNED:
                        {
                            materialHandler.burnMaterials();
                            break;
                        }
                    case DeathEvent.FROZEN:
                        {
                            materialHandler.frozenMaterials();
                            break;
                        }
                }
            }
            else
            {
                print("HP: " + hp);
                invulnerability = true;
                invulnerabilityTimer = invulnerabilityTime;

                materialHandler.ToFadeMode();
            }

        }
        else
        {
            hp = 0;
            if (deathEvent.Equals(DeathEvent.FALLED_IN_VACUUM))
                followTarget.transform.SetParent(null);
            Messenger<DeathEvent>.Broadcast(GameEvent.DEATH, deathEvent);
        }
    }

    private void OnDestroy()
    {
        Messenger.RemoveListener(GameEvent.RESET, Reset);
    }
}
