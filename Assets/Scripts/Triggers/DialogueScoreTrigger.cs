using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 *  Class: DialogueScoreTrigger
 *  
 *  Description:
 *  This script is very similar to DialogueTrigger, 
 *  but this is specifically used in HUB scene where
 *  player would to see score of one level
 *  
 *  Author: Thomas Voce
*/

public class DialogueScoreTrigger : MonoBehaviour
{
    public int level;
    bool isInside = false;

    // if player is inside this trigger and press submit, it will trigger DialogueScoreUI
    private void Update()
    {
        if(isInside && Input.GetButtonDown("Submit"))
            DialogueScoreUI.instance.ShowScores(level);
    }

    // when player enter in trigger, DialogueScoreUI show button
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isInside = true;
            DialogueScoreUI.instance.ShowButton(true);
        }
    }

    // when player exit from trigger, DialogueScoreUI don't show button
    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isInside = false;
            DialogueScoreUI.instance.ShowButton(false);
        }
    }
}
