using System.Collections;
using UnityEngine;
/*
 *  Class: RespawnManager
 *  
 *  Description:
 *  This manager handles the respawn point of the player.
 *  
 *  Author: Thomas Voce
*/
public class RespawnManager : MonoBehaviour, IGameManager
{
    private float RespawnTime;
    private GameObject player;
    private Vector3 respawnPos;
    private Quaternion respawnRot;

    public ManagerStatus status { get; private set; }

    public void Startup()
    {
        do
        {
            player = GameObject.FindGameObjectWithTag("Player");
            //player = PlayerStatisticsController.instance.gameObject;
        } while (player == null); // wait until player is created
        
        respawnPos = transform.position;
        respawnRot = transform.rotation;
        RespawnTime = 3f;

        status = ManagerStatus.Started;
    }

    private void OnPlayerDeath(DeathEvent deathEvent) {
        Messenger<bool>.Broadcast(GlobalVariables.ENABLE_INPUT, false);
        StartCoroutine(Respawn());
    }

    public void setRespawn(Vector3 pos, Quaternion rot)
    {
        respawnPos = pos;
        respawnRot = rot;
        Managers.Enemies.ClearEnemyDeadList();
    }

    private IEnumerator Respawn()
    {
        yield return new WaitForSeconds(RespawnTime);
        //yield return new WaitForEndOfFrame();

        player.transform.position = respawnPos;
        player.transform.rotation = respawnRot;
        print("RESPAWN");

        Messenger.Broadcast(GlobalVariables.RESET);
        Messenger<bool>.Broadcast(GlobalVariables.ENABLE_INPUT, true);
    }

    private void Awake()
    {
        Messenger<DeathEvent>.AddListener(GlobalVariables.DEATH, OnPlayerDeath);
    }

    private void OnDestroy()
    {
        Messenger<DeathEvent>.RemoveListener(GlobalVariables.DEATH, OnPlayerDeath);
    }

}
