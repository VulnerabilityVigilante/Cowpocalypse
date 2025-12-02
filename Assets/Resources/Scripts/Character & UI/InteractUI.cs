using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class InteractUI : MonoBehaviour
{
    public static InteractUI Instance;

    [Header("UI Elements")]
    public GameObject promptPanel; // Assign a UI panel in Canvas
    public TMP_Text promptText;        // Assign the Text component

    void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);

        HidePrompt();
    }

    public void ShowPrompt(string message)
    {
        promptPanel.SetActive(true);
        promptText.text = message;
    }

    public void HidePrompt()
    {
        promptPanel.SetActive(false);
    }
}
