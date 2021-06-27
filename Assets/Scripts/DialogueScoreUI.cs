using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class DialogueScoreUI : MonoBehaviour
{
    #region Singleton

    public static DialogueScoreUI instance;

    void Awake()
    {
        instance = this;
    }

    #endregion

    public GameObject DialogueBox, ExitImg, InteractImage;
    public TextMeshProUGUI textComponent;
    public float textSpeed;
    private Coroutine coroutine;

    private string text;
    private bool activable = true;

    void Start()
    {
        DialogueBox.SetActive(false);
        ExitImg.SetActive(false);
        textComponent.text = string.Empty;
    }

    void Update()
    {
        if (!DialogueBox.activeSelf)
            return;

        if (Input.GetButtonDown("Submit") || Input.GetButtonDown("Cancel"))
        {
            if (textComponent.text == text)
            {
                ExitImg.SetActive(false);
                DialogueBox.SetActive(false);
                StartCoroutine(DelayEnableInput());
                StartCoroutine(DelayEnableScoreUI());
            }
            else
            {
                if (coroutine != null)
                    StopCoroutine(coroutine);
                textComponent.text = text;
                ExitImg.SetActive(true);
            }
        }
    }

    public void ShowButton(bool active)
    {
        InteractImage.SetActive(active);
    }

    public void ShowScores(int level)
    {
        if (!activable)
            return;

        activable = false;
        Messenger<bool>.Broadcast(GlobalVariables.ENABLE_INPUT, false);
        DialogueBox.SetActive(true);
        textComponent.text = string.Empty;

        if (GlobalVariables.scores.TryGetValue(level, out int score))
            coroutine = StartCoroutine(TypeLine(text = GlobalVariables.Dialogues[0] + score));
        else
            coroutine = StartCoroutine(TypeLine(text = GlobalVariables.Dialogues[1]));
    }

    IEnumerator TypeLine(string x)
    {
        foreach (char c in x.ToCharArray())
        {
            textComponent.text += c;
            yield return new WaitForSeconds(textSpeed);
        }
        ExitImg.SetActive(true);
    }

    IEnumerator DelayEnableInput()
    {
        yield return new WaitForSeconds(.5f);
        Messenger<bool>.Broadcast(GlobalVariables.ENABLE_INPUT, true);
    }

    IEnumerator DelayEnableScoreUI()
    {
        yield return new WaitForSeconds(.5f);
        activable = true;
    }
}
