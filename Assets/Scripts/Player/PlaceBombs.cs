using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 *  Class: PlaceBombs
 *  
 *  Description:
 *  This script allow player to place bombs, if he have almost one.
 *  
 *  Author: Thomas Voce
*/
public class PlaceBombs : MonoBehaviour
{
    PlayerStatistics statistics;
    PlayerController player;

    [SerializeField] GameObject bomb;
    GameObject lastBomb;

    void Start()
    {
        player = GetComponent<PlayerController>();
        statistics = PlayerStatistics.instance;
    }

    void Update()
    {
        if (canPlaceBomb() && player.GetButtonDown("UseObject") && statistics.bombCount > 0 && lastBomb == null)
        {
            statistics.decreaseBomb();

            Vector3 pos = transform.position;
            pos += 1 * transform.forward;
            pos += 1 * Vector3.up;
            lastBomb = Instantiate(bomb, pos, Quaternion.identity);
        }
    }

    private bool canPlaceBomb()
    {
        return player.status == PlayerController.Status.IDLE
            || player.status == PlayerController.Status.CROUCH
            || player.status == PlayerController.Status.FALLING;
    }
}
