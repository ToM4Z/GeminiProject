using UnityEngine;

public class TriggerDeath : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerStatisticsController.instance.hurt(DeathEvent.FALLED_IN_VACUUM, true);
        }
    }
}
