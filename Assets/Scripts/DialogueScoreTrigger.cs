using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogueScoreTrigger : MonoBehaviour
{
    public int level;
    bool isInside = false;

    private void Update()
    {
        if(isInside && Input.GetButtonDown("Submit"))
            DialogueScoreUI.instance.ShowScores(level);
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
