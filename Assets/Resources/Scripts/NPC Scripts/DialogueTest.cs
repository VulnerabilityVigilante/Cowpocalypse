using UnityEngine;

public class DialogueTest : MonoBehaviour
{
    void Start()
    {
        string[] lines = new string[]
        {
            "Hey there, welcome to the ranch!",
            "Something strange has been happening around here...",
            "The animals don’t seem... normal.",
            "Could you help me fix the fence?"
        };

        DialogueUI.Instance.ShowDialogue(
            lines,
            () => Debug.Log("Quest Accepted"),
            () => Debug.Log("Quest Declined")
        );
    }
}
