using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 *  Class: EnemiesManager
 *  
 *  Description:
 *  This manager will respawn the enemies when the player dies.
 *  
 *  Author: Thomas Voce
*/
public class EnemiesManager : MonoBehaviour, IGameManager
{
    public ManagerStatus status { get; private set; }

    private List<GameObject> enemies = new List<GameObject>();

    private List<GameObject> enemiesDead = new List<GameObject>();

    public void Startup()
    {
        enemies.AddRange(GameObject.FindGameObjectsWithTag("Enemy"));

        status = ManagerStatus.Started;
    }

    public void EnemyDie(GameObject enemy)
    {
        if (!enemies.Remove(enemy))
            throw new System.Exception("Removing enemy that not existing");

        enemiesDead.Add(enemy);
    }

    public void ClearEnemyDeadList()
    {
        foreach (GameObject enemy in enemiesDead)
        {
            Destroy(enemy);
        }
        enemiesDead.Clear();
    }

    public void RespawnEnemies()
    {
        foreach (GameObject enemy in enemies)
            enemy.GetComponent<AIEnemy>().ResetEnemy();

        foreach (GameObject enemy in enemiesDead)
        {
            enemy.SetActive(true);
            enemy.GetComponent<AIEnemy>().ResetEnemy();
            enemies.Add(enemy);
        }
        enemiesDead.Clear();
    }

    private void Awake()
    {
        Messenger.AddListener(GameEvent.RESET, RespawnEnemies);
    }

    private void OnDestroy()
    {
        Messenger.RemoveListener(GameEvent.RESET, RespawnEnemies);
    }
}
