using UnityEngine;
using System.Collections.Generic;

public enum QuestState { NotStarted, InProgress, Completed }

// ✅ Move the event class INSIDE the same file but OUTSIDE of any MonoBehaviour
public static class QuestEvents
{
    public static System.Action<string> OnQuestStarted;
    public static System.Action<string> OnQuestCompleted;
}

[System.Serializable]
public class Quest
{
    public string questID;
    public string description;
    public QuestState state = QuestState.NotStarted;

    [Header("Optional: Subtasks this quest depends on")]
    public string[] requiredSubtaskIDs;

    [Header("Rewards")]
    public int coinReward = 0;
}

public class QuestManager : MonoBehaviour
{
    public static QuestManager Instance;

    public List<Quest> quests = new List<Quest>();

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void StartQuest(string id)
    {
        Quest quest = quests.Find(q => q.questID == id);
        if (quest != null && quest.state == QuestState.NotStarted)
        {
            quest.state = QuestState.InProgress;
            Debug.Log($"Quest started: {quest.questID}");
            QuestEvents.OnQuestStarted?.Invoke(id); // ✅ trigger event safely
        }
        else if (quest == null)
        {
            Debug.LogWarning($"Quest '{id}' not found in QuestManager.");
        }
    }

    public void CompleteQuest(string id)
    {
        Quest quest = quests.Find(q => q.questID == id);
        if (quest != null && quest.state == QuestState.InProgress)
        {
            quest.state = QuestState.Completed;
            Debug.Log($"Quest completed: {quest.questID}");
            QuestEvents.OnQuestCompleted?.Invoke(id);

            // stop spawner after FixFence quest
            if (id == "FixFence" && AnimalSpawner.Instance != null)
            {
                AnimalSpawner.Instance.StopSpawning();
            }

            RecheckAllParentQuests();
        }
    }


    private void RecheckAllParentQuests()
    {
        foreach (Quest q in quests)
        {
            if (q.state == QuestState.InProgress && q.requiredSubtaskIDs.Length > 0)
            {
                bool allDone = true;
                foreach (string sub in q.requiredSubtaskIDs)
                {
                    if (GetQuestState(sub) != QuestState.Completed)
                    {
                        allDone = false;
                        break;
                    }
                }

                if (allDone)
                {
                    CompleteQuest(q.questID);
                }
            }
        }
    }

    public QuestState GetQuestState(string id)
    {
        Quest quest = quests.Find(q => q.questID == id);
        return quest != null ? quest.state : QuestState.NotStarted;
    }
}
