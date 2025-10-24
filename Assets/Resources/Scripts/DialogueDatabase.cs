using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "DialogueDatabase", menuName = "Dialogue/Database", order = 1)]
public class DialogueDatabase : ScriptableObject
{
    [System.Serializable]
    public class QuestDialogue
    {
        public string questID;

        [Header("When the player first talks (quest offer)")]
        [TextArea(3, 5)] public string[] introLines;

        [Header("When the quest is already active")]
        [TextArea(3, 5)] public string[] activeLines;

        [Header("When the quest is completed")]
        [TextArea(3, 5)] public string[] completeLines;
    }

    [Header("All quest dialogue for this NPC")]
    public List<QuestDialogue> dialogues = new List<QuestDialogue>();

    [Header("Final dialogue when all parent quests are complete (optional)")]
    [TextArea(3, 5)]
    public string[] allQuestsCompleteLines;  // ✅ now defined at the root level

    public QuestDialogue GetDialogue(string questID)
    {
        return dialogues.Find(d => d.questID == questID);
    }
}
