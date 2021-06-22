using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AmbientalObjectsManager : MonoBehaviour, IGameManager
{
    public ManagerStatus status { get; private set; }

    private List<GameObject> resettables = new List<GameObject>();

    public void Startup()
    {
        resettables.AddRange(GameObject.FindGameObjectsWithTag("Ambiental Object to Reset"));

        status = ManagerStatus.Started;
    }

    public void ResetObjects()
    {
        foreach(GameObject o in resettables )
        {
            o.GetComponent<IResettable>().Reset();
        }
    }

    private void Awake()
    {
        Messenger.AddListener(GlobalVariables.RESET, ResetObjects);
    }

    private void OnDestroy()
    {
        Messenger.RemoveListener(GlobalVariables.RESET, ResetObjects);
    }
}
