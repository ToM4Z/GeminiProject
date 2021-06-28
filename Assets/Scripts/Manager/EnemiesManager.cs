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

    // this list contains enemies who actually are alive
    private List<GameObject> enemies = new List<GameObject>();

    // this list contains enemies who are dead and, if player reach a checkpoint, they cannot respawn more
    private List<GameObject> enemiesDead = new List<GameObject>();

    // object that all enemies have to spawn
    [SerializeField] public GameObject DropItem;

    public void Startup()
    {
        enemies.AddRange(GameObject.FindGameObjectsWithTag("Enemy")); // collect all enemies at runtime

        status = ManagerStatus.Started;
    }

    // when an enemy die, i move it from a list to another, but..
    public void EnemyDie(GameObject enemy)
    {
        if (enemies.Remove(enemy))  // if I'm not able to remove an enemy means that enemy was spawned at runtime and cannot be respawned
        {
            enemiesDead.Add(enemy);
        }
    }

    // when player trigger a checkpoint, i delete all enemies who are dead 
    public void ClearEnemyDeadList()
    {
        foreach (GameObject enemy in enemiesDead)
        {
            Destroy(enemy);
        }
        enemiesDead.Clear();
    }

    // when player respawn, I have to respawn all enemies, even enemies dead,
    // so I reset alive and dead enemies and move dead enemies to alive list
    public void RespawnEnemies()
    {
        foreach (GameObject enemy in enemies)
            enemy.GetComponent<IResettable>().Reset();

        foreach (GameObject enemy in enemiesDead)
        {
            enemy.SetActive(true);
            enemy.GetComponent<IResettable>().Reset();
            enemies.Add(enemy);
        }
        enemiesDead.Clear();
    }

    private void Awake()
    {
        Messenger.AddListener(GlobalVariables.RESET, RespawnEnemies);
    }

    private void OnDestroy()
    {
        Messenger.RemoveListener(GlobalVariables.RESET, RespawnEnemies);
    }
}
