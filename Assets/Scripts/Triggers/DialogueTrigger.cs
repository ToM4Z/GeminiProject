using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 *  Class: DialogueTrigger
 *  
 *  Description:
 *  Activate DialogueUI and pass to him the dialogue identifier to show
 *  
 *  Author: Thomas Voce
*/

public class DialogueTrigger : MonoBehaviour
{
    // I send to DialogueUI the start and end strings to display
    public int DialogueStart, DialogueEnd;
    public int condition = 0;

    // If this dialogue is already shown, delete this go
    private void Start()
    {
        if (!IsConditionTrue(0))
            Destroy(this);
    }

    // some dialogues can have some type of condition to be triggered,
    // at this moment, only one dialogue must be triggered when player finish all 3 levels
    private bool IsConditionTrue() { return IsConditionTrue(condition); }
    private bool IsConditionTrue(int x)
    {
        switch (x)
        {
            case 1: return GlobalVariables.scores.Keys.Count == 3;
            default: return !GlobalVariables.isDialogueDone(DialogueStart); // I place this check here because player can trigger this go even before start (or awake) method is runned (strange but true)
        }
    }

    // when player trigger this go, and if condition is true, I say to dialogueUI to start dialogue
    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && IsConditionTrue())
        {
            GlobalVariables.addDialogueDone(DialogueStart);
            DialogueUI.instance.StartDialogue(DialogueStart, DialogueEnd);
            Destroy(this);
        }
    }
}
