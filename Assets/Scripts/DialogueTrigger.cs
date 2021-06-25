using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogueTrigger : MonoBehaviour
{
    public int DialogueStart, DialogueEnd;

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            DialogueUI.instance.StartDialogue(DialogueStart, DialogueEnd);
            gameObject.SetActive(false);
        }
    }
}
