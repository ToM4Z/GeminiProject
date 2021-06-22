using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollectablesManager : MonoBehaviour, IGameManager
{
    public ManagerStatus status { get; private set; }
    private List<GameObject> collectables = new List<GameObject>();
    private List<GameObject> collectedItems = new List<GameObject>();
    [SerializeField] private GameObject playerStatistics;
    private int gearCount;
    private int bonusGearCount;

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

    private void UpdatePlayerStats() {
        
    }

    public void ClearCollectedList()
    {
        collectedItems.Clear();
        this.gearCount = playerStatistics.GetComponent<PlayerStatistics>().normalGearCount;
        this.bonusGearCount = playerStatistics.GetComponent<PlayerStatistics>().bonusGearCount;

    }

    public void RespawnCollectables()
    {
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
        playerStatistics.GetComponent<PlayerStatistics>().normalGearCount = gearCount;
        playerStatistics.GetComponent<PlayerStatistics>().bonusGearCount = this.bonusGearCount;
        UIManager.instance.GetHUD().setGearCounter(gearCount);
        UIManager.instance.GetHUD().setGearBonusCounter(bonusGearCount);
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
