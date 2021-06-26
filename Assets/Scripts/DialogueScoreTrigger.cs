using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogueScoreTrigger : MonoBehaviour
{
    bool isInside = false;

    private void Update()
    {
        if(isInside && Input.GetButtonDown("Submit"))
            DialogueScoreUI.instance.ShowScores();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isInside = true;
            DialogueScoreUI.instance.ShowButton(true);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isInside = false;
            DialogueScoreUI.instance.ShowButton(false);
        }
    }
}
