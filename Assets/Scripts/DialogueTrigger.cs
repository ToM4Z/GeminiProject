using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogueTrigger : MonoBehaviour
{
    public int DialogueStart, DialogueEnd;
    public int condition = 0;

    private void Start()
    {
        if (!IsConditionTrue(0))
            Destroy(this);
    }

    private bool IsConditionTrue() { return IsConditionTrue(condition); }
    private bool IsConditionTrue(int x)
    {
        switch (x)
        {
            case 1: return GlobalVariables.scores.Keys.Count == 3;
            default: return !GlobalVariables.isDialogueDone(DialogueStart);
        }
    }

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
