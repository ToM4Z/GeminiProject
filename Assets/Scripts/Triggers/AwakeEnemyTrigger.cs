using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 *  Class: AwakeEnemyTrigger
 *  
 *  Description:
 *  This script is used to awake enemies which are in INACTIVE status
 *  
 *  Author: Thomas Voce
*/

public class AwakeEnemyTrigger : MonoBehaviour
{
    [SerializeField] List<AIEnemy> enemies;

    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Player"))
            foreach (AIEnemy e in enemies)
                e.AwakeEnemy();
    }
}
