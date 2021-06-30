using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

/*
 *  Class: DialogueUI
 *  
 *  Description:
 *  This script manages DIalogue System
 *  
 *  Author: Thomas Voce
*/

public class DialogueUI : MonoBehaviour
{

    #region Singleton

    public static DialogueUI instance;

    void Awake()
    {
        instance = this;
    }

    #endregion

    public GameObject DialogueBox, NextImg, ExitImg;
    public TextMeshProUGUI textComponent;
    public float textSpeed;
    private Coroutine coroutine;

    private int index, endIndex;

    // disable all objects on start
    void Start()
    {
        DialogueBox.SetActive(false);
        NextImg.SetActive(false);
        ExitImg.SetActive(false);
        textComponent.text = string.Empty;
    }

    // if a dialogue is open and I click on submit,
    // I stop text animation and with another click I go to the next string or I close dialogue
    // If I click Escape (optionally) I immediately close dialogue
    void Update()
    {
        if (!DialogueBox.activeSelf)
            return;

        if (Input.GetButtonDown("Submit"))
        {
            if(textComponent.text == GlobalVariables.Dialogues[index])
            {
                NextImg.SetActive(false);
                ExitImg.SetActive(false);
                NextLine();
            }
            else
            {
                if(coroutine != null)
                    StopCoroutine(coroutine);
                textComponent.text = GlobalVariables.Dialogues[index];
                ShowNextImg();
            }
        } else if (Input.GetButtonDown("Escape"))
        {
            if (coroutine != null)
                StopCoroutine(coroutine);
            StartCoroutine(DelayEnableInput()); // I delay enable input because player still receive input (is not in pause)
            DialogueBox.SetActive(false);
        }
    }

    // If this is the last string I show the exit image, otherwise I show the next image
    private void ShowNextImg()
    {
        if (index == endIndex)
            ExitImg.SetActive(true);
        else
            NextImg.SetActive(true);
    }

    // called by DialogueTrigger, it say me which strings I'd show to player
    public void StartDialogue(int startIndex, int endIndex)
    {
        Messenger<bool>.Broadcast(GlobalVariables.ENABLE_INPUT, false);
        DialogueBox.SetActive(true);
        textComponent.text = string.Empty;
        index = startIndex;
        this.endIndex = endIndex;

        coroutine = StartCoroutine(TypeLine());
    }

    // this coroutine apply a text animation
    IEnumerator TypeLine()
    {
        foreach (char c in GlobalVariables.Dialogues[index].ToCharArray())
        {
            textComponent.text += c;
            yield return new WaitForSeconds(textSpeed);
        }
        ShowNextImg();
    }

    // if this was the last string I close Dialogue, otherwise I'll show the next string
    void NextLine()
    {
        if (index < endIndex)
        {
            index++;
            textComponent.text = string.Empty;
            coroutine = StartCoroutine(TypeLine());
        }
        else
        {
            StartCoroutine(DelayEnableInput());
            DialogueBox.SetActive(false);
        }
    }

    IEnumerator DelayEnableInput()
    {
        yield return new WaitForSeconds(.5f);
        Messenger<bool>.Broadcast(GlobalVariables.ENABLE_INPUT, true);
    }
}
