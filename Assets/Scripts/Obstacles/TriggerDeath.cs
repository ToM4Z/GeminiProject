using UnityEngine;
/*
 *  Class: TriggerDeath
 *  
 *  Description:
 *  This script allow to kill player when he fall in the vacuum.
 *  
 *  Author: Thomas Voce
*/
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
