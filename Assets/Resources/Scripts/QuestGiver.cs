using UnityEngine;
using System.Collections.Generic;

public class QuestGiver : MonoBehaviour
{
    [Header("Parent Quests (in order)")]
    public List<string> parentQuestIDs = new List<string> { "RepairFenceQuest" };

    [Header("Dialogue Database")]
    public DialogueDatabase dialogueDatabase; // 👈 Added this line

    // This keeps track of the last quest that was given or completed
    private string currentQuestID = null;
    
    private HashSet<string> rewardedQuests = new HashSet<string>();


    public string GetActiveQuestID()
    {
        foreach (string id in parentQuestIDs)
        {
            QuestState state = QuestManager.Instance.GetQuestState(id);
            if (state == QuestState.InProgress)
                return id;
        }
        return null;
    }

    public string GetNextQuestID()
    {
        foreach (string id in parentQuestIDs)
        {
            QuestState state = QuestManager.Instance.GetQuestState(id);
            if (state == QuestState.NotStarted)
                return id;
        }
        return null;
    }

    public bool AllQuestsComplete()
    {
        foreach (string id in parentQuestIDs)
        {
            if (QuestManager.Instance.GetQuestState(id) != QuestState.Completed)
                return false;
        }
        return true;
    }

    public void GiveNextParentQuest()
    {
        if (AllQuestsComplete())
        {
            Debug.Log("All quests are complete!");
            return;
        }

        string nextQuestID = GetNextQuestID();

        if (string.IsNullOrEmpty(nextQuestID))
        {
            Debug.Log("No new quest to give (active or all complete).");
            return;
        }

        QuestManager.Instance.StartQuest(nextQuestID);
        currentQuestID = nextQuestID;

        // Auto-start subtasks
        Quest parent = QuestManager.Instance.quests.Find(q => q.questID == nextQuestID);
        if (parent != null && parent.requiredSubtaskIDs != null)
        {
            foreach (string sub in parent.requiredSubtaskIDs)
            {
                QuestManager.Instance.StartQuest(sub);
                Debug.Log($"Subquest started: {sub}");
            }
        }

        Debug.Log($"Quest started: {nextQuestID}");
    }
    public void TryGiveQuestReward()
    {
        // Find the last parent quest that was completed but not yet rewarded
        foreach (string id in parentQuestIDs)
        {
            Quest quest = QuestManager.Instance.quests.Find(q => q.questID == id);
            if (quest != null && quest.state == QuestState.Completed)
            {
                // Reward only once (mark with a simple internal flag)
                if (!rewardedQuests.Contains(id))
                {
                    if (CurrencyManager.Instance != null && quest.coinReward > 0)
                    {
                        CurrencyManager.Instance.AddCoins(quest.coinReward);
                        Debug.Log($"Rewarded player {quest.coinReward} coins for completing '{quest.questID}'");
                    }

                    rewardedQuests.Add(id);
                    return;
                }
            }
        }
    }

}
