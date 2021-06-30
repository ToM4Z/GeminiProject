using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 *  Class: ChangeSceneTrigger
 *  
 *  Description:
 *  Trigger used in HUB scene to change scene simply by entering in this trigger
 *  
 *  Author: Thomas Voce
*/

public class ChangeSceneTrigger : MonoBehaviour
{
    public int SceneIndex;

    void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Player"))
            LevelLoader.instance.LoadLevel(SceneIndex);
    }
}
