using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

    public void Startup()
    {
        collectables.AddRange(GameObject.FindGameObjectsWithTag("Collectable"));
        status = ManagerStatus.Started;
        Debug.Log("gear count: "+collectables.Count);
    }

    public void CollectedItem(GameObject item)
    {
        if (collectables.Remove(item) && !item.name.Contains("SpawnerController"))  // if I'm not able to remove an enemy means that enemy was spawned at runtime and cannot be respawned
        {
            collectedItems.Add(item);
        }
    }

    public void ClearCollectedList()
    {
        collectedItems.Clear();
        this.gearCount = PlayerStatistics.instance.normalGearCount;
        this.bonusGearCount = PlayerStatistics.instance.bonusGearCount;
        this.bombCount = PlayerStatistics.instance.bombCount;
    }

    public void RespawnCollectables()
    {
        foreach (GameObject gear in gearDropped)
            Destroy(gear);
        gearDropped.Clear();

        foreach (GameObject collectable in collectables) {
            if(collectable.name.Contains("SpawnerController") && collectable != null) {
                collectable.GetComponent<BonusGearActivator>().firstTime = true;
            }
        }
        foreach (GameObject collectable in collectedItems)
        {
            collectable.SetActive(true);
            collectables.Add(collectable);
        }
        collectedItems.Clear();
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
