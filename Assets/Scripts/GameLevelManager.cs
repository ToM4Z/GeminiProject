using System.Collections;
using UnityEngine;

public class GameLevelManager : MonoBehaviour
{
    [SerializeField] private float RespawnTime;
    private GameObject player;
    private Vector3 respawnPos;
    private Quaternion respawnRot;

    private void Start()
    {
        player = PlayerStatisticsController.instance.gameObject;
        respawnPos = player.transform.position;
        respawnRot = player.transform.rotation; 
    }

    private void OnPlayerDeath(DeathEvent deathEvent) {
        Messenger<bool>.Broadcast(GameEvent.ENABLE_INPUT, false);
        StartCoroutine(Respawn());
    }

    private IEnumerator Respawn()
    {
        yield return new WaitForSeconds(RespawnTime);
        yield return new WaitForEndOfFrame();

        player.transform.position = player.transform.parent.position;
        player.transform.rotation = player.transform.parent.rotation;
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
