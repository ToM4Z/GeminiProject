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

    // position and rotation where to respawn player
    private Vector3 respawnPos;
    private Quaternion respawnRot;

    public ManagerStatus status { get; private set; }

    public void Startup()
    {
        do
        {
            player = GameObject.FindGameObjectWithTag("Player");
        } while (player == null); // wait until player is created
        
        respawnPos = transform.position;
        respawnRot = transform.rotation;
        RespawnTime = 3f;

        status = ManagerStatus.Started;
    }

    // when player is dead, if he lose a life, and
    // if he have lifes >=0 yet, he can respawn
    // otherwise, gameover occurs
    private void OnPlayerDeath(DeathEvent deathEvent) {
        Messenger<bool>.Broadcast(GlobalVariables.ENABLE_INPUT, false);

        if (GlobalVariables.PlayerLives >= 0)   
        {
            UIManager.instance.GetBlackFadeScreen().startFade();
            StartCoroutine(Respawn());
        }
        else
        {
            GlobalVariables.GameOver = true;
            GameOverScreen.instance.ActiveGameOverScreen();
        }
    }

    // every time player arrive on a checkpoint, this method is called
    // here respawn position change and enemies and collectables until now are deleted
    public void setRespawn(Vector3 pos, Quaternion rot)
    {
        respawnPos = pos;
        respawnRot = rot;
        Managers.Enemies.ClearEnemyDeadList();
        Managers.Collectables.ClearCollectedList();
    }

    // when respawn starts, wait until screen is black and then
    // reset player position and broadcast message of reset to all entities
    // and enable input for player
    private IEnumerator Respawn()
    {
        yield return new WaitForSeconds(RespawnTime);

        player.transform.position = respawnPos;
        player.transform.rotation = respawnRot;
        print("RESPAWN");
        yield return new WaitForEndOfFrame();

        Messenger.Broadcast(GlobalVariables.RESET, MessengerMode.DONT_REQUIRE_LISTENER);
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
