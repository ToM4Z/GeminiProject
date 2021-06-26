using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

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

    void Start()
    {
        DialogueBox.SetActive(false);
        NextImg.SetActive(false);
        ExitImg.SetActive(false);
        textComponent.text = string.Empty;
    }

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
        }
    }

    private void ShowNextImg()
    {
        if (index == endIndex)
            ExitImg.SetActive(true);
        else
            NextImg.SetActive(true);
    }

    public void StartDialogue(int startIndex, int endIndex)
    {
        Messenger<bool>.Broadcast(GlobalVariables.ENABLE_INPUT, false);
        DialogueBox.SetActive(true);
        textComponent.text = string.Empty;
        index = startIndex;
        this.endIndex = endIndex;
        coroutine = StartCoroutine(TypeLine());
    }

    IEnumerator TypeLine()
    {
        return TypeLine(GlobalVariables.Dialogues[index]);
    }

    IEnumerator TypeLine(string x)
    {
        foreach (char c in x.ToCharArray())
        {
            textComponent.text += c;
            yield return new WaitForSeconds(textSpeed);
        }
        ShowNextImg();
    }

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
