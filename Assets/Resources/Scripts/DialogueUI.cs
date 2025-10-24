using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System;
using System.Collections;

public class DialogueUI : MonoBehaviour
{
    public static DialogueUI Instance;

    [Header("UI References")]
    public GameObject dialoguePanel;
    public TMP_Text dialogueText;
    public Button nextButton;
    public Button acceptButton;
    public Button declineButton;

    private string[] currentLines;
    private int currentIndex;
    private Action onAccept;
    private Action onDecline;

    void Awake()
    {
        Instance = this;
        dialoguePanel.SetActive(false);
    }

    public void ShowDialogue(string[] lines, Action acceptCallback = null, Action declineCallback = null)
    {
        currentLines = lines;
        currentIndex = 0;
        onAccept = acceptCallback;
        onDecline = declineCallback;

        dialoguePanel.SetActive(true);
        SetCursorVisible(true);
        acceptButton.gameObject.SetActive(false);
        declineButton.gameObject.SetActive(false);
        nextButton.gameObject.SetActive(true);

        DisplayCurrentLine();
    }

    void DisplayCurrentLine()
    {
        StopAllCoroutines();
        StartCoroutine(TypeText(currentLines[currentIndex]));
    }

    IEnumerator TypeText(string line)
    {
        dialogueText.text = "";
        foreach (char c in line)
        {
            dialogueText.text += c;
            yield return new WaitForSeconds(0.02f);
        }
    }

    public void OnNextClicked()
    {
        currentIndex++;
        if (currentIndex < currentLines.Length)
        {
            DisplayCurrentLine();
        }
        else
        {
            // End of dialogue lines
            nextButton.gameObject.SetActive(false);

            if (onAccept != null)
            {
                acceptButton.gameObject.SetActive(true);
                declineButton.gameObject.SetActive(true);
            }
            else
            {
                Close();
            }
        }
    }

    public void OnAcceptClicked()
    {
        if (onAccept != null)
        {
            // Let the callback decide whether to close or show new dialogue
            var temp = onAccept;
            onAccept = null;
            temp.Invoke();
        }
        else
        {
            Close();
        }
    }

    public void OnDeclineClicked()
    {
        if (onDecline != null)
        {
            // Let the callback decide whether to close or show new dialogue
            var temp = onDecline;
            onDecline = null;
            temp.Invoke();
        }
        else
        {
            Close();
        }
    }


    public void Close()
    {
        dialoguePanel.SetActive(false);
        currentLines = null;
        currentIndex = 0;
        SetCursorVisible(false);
    }

    void SetCursorVisible(bool visible)
    {
        Cursor.visible = visible;
        Cursor.lockState = visible ? CursorLockMode.None : CursorLockMode.Locked;
    }


}
