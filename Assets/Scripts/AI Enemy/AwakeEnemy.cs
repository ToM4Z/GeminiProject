using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AwakeEnemy : MonoBehaviour
{
    [SerializeField] List<AIEnemy> enemies;

    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Player"))
            foreach (AIEnemy e in enemies)
                e.AwakeEnemy();
    }
}
