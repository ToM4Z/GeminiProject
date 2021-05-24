using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/*
 *  Class: Managers
 *  
 *  Description:
 *  This manager starts all other managers.
 *  
 *  Author: Prof
*/

[RequireComponent(typeof(EnemiesManager))]
[RequireComponent(typeof(RespawnManager))]
//[RequireComponent(typeof())]
public class Managers : MonoBehaviour
{
    public static EnemiesManager Enemies { get; private set; }
    public static RespawnManager Respawn { get; private set; }

    private List<IGameManager> _startSequence;


    void Awake()
    {
        Enemies = GetComponent<EnemiesManager>();
        Respawn = GetComponent<RespawnManager>();

        _startSequence = new List<IGameManager>();
        _startSequence.Add(Enemies);
        _startSequence.Add(Respawn);

        StartCoroutine(StartupManagers());
    }

    private IEnumerator StartupManagers()
    {
        foreach( IGameManager manager in _startSequence)
        {
            manager.Startup();
        }

        yield return null;
        int numModules = _startSequence.Count;
        int numReady = 0;

        while(numReady < numModules)
        {
            int lastReady = numReady;
            numReady = 0;
             foreach(IGameManager manager in _startSequence)
            {
                if(manager.status == ManagerStatus.Started)
                {
                    numReady++;
                }
            }
            if (numReady > lastReady)
                print("Progress: " + numReady + "/" + numModules);
            yield return null;
        }
        print("All managers started up");
    }
}