using System.Collections;
using UnityEngine;

public class GameLevelManager : MonoBehaviour
{
    private GameObject player;

    private void Start()
    {
        player = PlayerStatisticsController.instance.gameObject;
    }

    private void OnPlayerDeath(DeathEvent deathEvent) {
        Messenger<bool>.Broadcast(GameEvent.ENABLE_INPUT_CAMERA, false);
        StartCoroutine(Respawn());
    }

    private IEnumerator Respawn()
    {
        yield return new WaitForSeconds(5f);
        player.transform.SetPositionAndRotation()
        Messenger.Broadcast(GameEvent.RESET);
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
