using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 *  Class: CollectablesManager
 *  
 *  Description:
 *  this manager handle collectable object reset such as: gear, bonus gear count and bomb count
 *  
 *  Author: Gianfranco Sapia
*/
public class CollectablesManager : MonoBehaviour, IGameManager
{
    public ManagerStatus status { get; private set; }
    private List<GameObject> collectables = new List<GameObject>();
    private List<GameObject> collectedItems = new List<GameObject>();
    private List<GameObject> gearDropped = new List<GameObject>();
    private int gearCount;
    private int bonusGearCount;
    private int bombCount;

    public GameObject eventFX;

    //This list stores every item Collectable at start
    public void Startup()
    {
        collectables.AddRange(GameObject.FindGameObjectsWithTag("Collectable"));
        status = ManagerStatus.Started;
    }

    //Get track of every object collected, expect for bonus block spawner
    public void CollectedItem(GameObject item)
    {
        if (collectables.Remove(item) && !item.name.Contains("SpawnerController"))
        {
            collectedItems.Add(item);
        }
    }

    //When a checkpoint is reached the list of collected item is resetted and the stat updated at that moment  
    public void ClearCollectedList()
    {
        collectedItems.Clear();
        this.gearCount = PlayerStatistics.instance.normalGearCount;
        this.bonusGearCount = PlayerStatistics.instance.bonusGearCount;
        this.bombCount = PlayerStatistics.instance.bombCount;
    }

    //This function is called every time the player die
    public void RespawnCollectables()
    {
        //gear dropped from enemy and not collected are destroyed
        foreach (GameObject gear in gearDropped)
            Destroy(gear);
        gearDropped.Clear();

        //bonus gear block are resetted
        foreach (GameObject collectable in collectables) {
            if(collectable.name.Contains("SpawnerController") && collectable != null) {
                collectable.GetComponent<BonusGearActivator>().firstTime = true;
            }
        }

        //object collected can now be collected again
        foreach (GameObject collectable in collectedItems)
        {
            collectable.SetActive(true);
            collectables.Add(collectable);
        }

        collectedItems.Clear();

        //stats reset in playerstatistics and in hud 
        PlayerStatistics.instance.normalGearCount = gearCount;
        PlayerStatistics.instance.bonusGearCount = bonusGearCount;
        PlayerStatistics.instance.bombCount = bombCount;
        UIManager.instance.GetHUD().setGearCounter(gearCount);
        UIManager.instance.GetHUD().setGearBonusCounter(bonusGearCount);
        UIManager.instance.GetHUD().setBombCounter(bombCount);
    }

    public void AddGearDropped(GameObject go)
    {
        gearDropped.Add(go);
    }

    private void Awake()
    {
        Messenger.AddListener(GlobalVariables.RESET, RespawnCollectables);
    }

    private void OnDestroy()
    {
        Messenger.RemoveListener(GlobalVariables.RESET, RespawnCollectables);
    }
}
