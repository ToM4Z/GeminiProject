using System.Collections;
using UnityEngine;
/*
 *  Class: GameLevelManager
 *  
 *  Description:
 *  This script is the manager of the level. 
 *  It handles the respawn point of the player.
 *  
 *  Author: Thomas Voce
*/
public class GameLevelManager : MonoBehaviour
{
    [SerializeField] private float RespawnTime;
    private GameObject player;
    private Vector3 respawnPos;
    private Quaternion respawnRot;

    private void Start()
    {
        player = PlayerStatisticsController.instance.gameObject;
        respawnPos = transform.position;
        respawnRot = transform.rotation; 
    }

    private void OnPlayerDeath(DeathEvent deathEvent) {
        Messenger<bool>.Broadcast(GameEvent.ENABLE_INPUT, false);
        StartCoroutine(Respawn());
    }

    public void setRespawn(Vector3 pos, Quaternion rot)
    {
        respawnPos = pos;
        respawnRot = rot;
    }

    private IEnumerator Respawn()
    {
        yield return new WaitForSeconds(RespawnTime);
        yield return new WaitForEndOfFrame();

        player.transform.position = respawnPos;
        player.transform.rotation = respawnRot;
        print("RESPAWN");

        Messenger.Broadcast(GameEvent.RESET);
        Messenger<bool>.Broadcast(GameEvent.ENABLE_INPUT, true);
    }

    private void Awake()
    {
        Messenger<DeathEvent>.AddListener(GameEvent.DEATH, OnPlayerDeath);
    }

    private void OnDestroy()
    {
        Messenger<DeathEvent>.RemoveListener(GameEvent.DEATH, OnPlayerDeath);
    }
}
