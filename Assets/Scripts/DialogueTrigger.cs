using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogueTrigger : MonoBehaviour
{
    public int DialogueStart, DialogueEnd;
    public int condition = 0;

    private bool IsConditionTrue()
    {
        switch (condition)
        {
            case 1: return GlobalVariables.scores.Keys.Count == 3;
            default: return true;
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && IsConditionTrue())
        {
            DialogueUI.instance.StartDialogue(DialogueStart, DialogueEnd);
            Destroy(this);
        }
    }
}
