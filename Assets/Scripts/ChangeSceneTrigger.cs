using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChangeSceneTrigger : MonoBehaviour
{
    public int SceneIndex;

    void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Player"))
            LevelLoader.instance.LoadLevel(SceneIndex);
    }
}
