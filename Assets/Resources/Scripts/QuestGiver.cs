using UnityEngine;
using System.Collections.Generic;

public class QuestGiver : MonoBehaviour
{
    [Header("Parent Quests (in order)")]
    public List<string> parentQuestIDs = new List<string> {
        "RepairFenceQuest",
        "SpendAtShopQuest"
    };

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
            // Trim in case there are invisible spaces in quest IDs
            string trimmedID = id.Trim();

            // Try to find the quest in QuestManager
            Quest quest = QuestManager.Instance.quests.Find(q => q.questID.Trim() == trimmedID);

            // If not found, log a clear warning (helps detect typos)
            if (quest == null)
            {
                Debug.LogWarning($"[QuestGiver] Quest '{trimmedID}' not found in QuestManager!");
                continue;
            }

            QuestState state = quest.state;

            // Log state info for debugging visibility
            Debug.Log($"[QuestGiver] Checking quest '{trimmedID}' | state = {state} | rewarded = {rewardedQuests.Contains(trimmedID)}");

            // ✅ Skip any quests that were already rewarded
            if (rewardedQuests.Contains(trimmedID))
                continue;

            // ✅ Return the first not-started quest
            if (state == QuestState.NotStarted)
            {
                Debug.Log($"[QuestGiver] Next available quest: {trimmedID}");
                return trimmedID;
            }
        }

        Debug.Log("[QuestGiver] No next quest found — either all rewarded or in progress.");
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
        Debug.Log("---- QUEST HANDOFF DEBUG ----");
        foreach (string id in parentQuestIDs)
        {
            Debug.Log($"[QuestGiver] '{id}' state = {QuestManager.Instance.GetQuestState(id)}");
        }

        string nextQuestID = GetNextQuestID();
        Debug.Log($"[QuestGiver] Next quest candidate: {nextQuestID}");

        if (!string.IsNullOrEmpty(nextQuestID))
        {
            Quest quest = QuestManager.Instance.quests.Find(q => q.questID == nextQuestID);
            if (quest == null)
            {
                Debug.LogWarning($"[QuestGiver] Quest '{nextQuestID}' NOT FOUND in QuestManager. Check the QuestManager list!");
                return;
            }

            QuestManager.Instance.StartQuest(nextQuestID);
            Debug.Log($"[QuestGiver] ✅ Started parent quest: {nextQuestID}");

            // Auto-start subtasks if defined on the parent
            if (quest.requiredSubtaskIDs != null)
            {
                foreach (string sub in quest.requiredSubtaskIDs)
                {
                    QuestManager.Instance.StartQuest(sub);
                    Debug.Log($"[QuestGiver] ✅ Auto-started subquest: {sub}");
                }
            }
            return;
        }

        if (AllQuestsComplete())
        {
            Debug.Log("[QuestGiver] 🎯 All parent quests complete.");
            return;
        }

        Debug.Log("[QuestGiver] ⚠️ No new quest to give.");
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


    public bool HasBeenRewarded(string questID)
    {
        return rewardedQuests.Contains(questID);
    }

}
