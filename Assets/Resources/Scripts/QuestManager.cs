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
    Dictionary<string, int> questProgress = new Dictionary<string, int>();
    Dictionary<string, int> questProgressGoals = new Dictionary<string, int>();


    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }
    }

    void Start()
    {
        // ✅ now everything in the scene exists
        QuestFileSaveSystem.LoadAll();
    }




    public void StartQuest(string id)
    {
        Quest quest = quests.Find(q => q.questID == id);
        if (quest != null && quest.state == QuestState.NotStarted)
        {
            quest.state = QuestState.InProgress;
            Debug.Log($"Quest started: {quest.questID}");
            QuestEvents.OnQuestStarted?.Invoke(id); // event for QuestSpawnGate
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
            // Save immediately when quest completes
            QuestGiver questGiver = GameObject.FindObjectOfType<QuestGiver>();
            if (questGiver != null)
                QuestFileSaveSystem.SaveAll(questGiver);

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


    void RecheckAllParentQuests()
    {
        foreach (Quest q in quests)
        {
            // ✅ Only check quests that actually have subtasks
            if (q.state == QuestState.InProgress && q.requiredSubtaskIDs != null && q.requiredSubtaskIDs.Length > 0)
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
                    Debug.Log($"[QuestManager] Auto-completing parent quest '{q.questID}' because all subtasks are done.");
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

    public void SetQuestGoal(string id, int goal)
    {
        if (!questProgressGoals.ContainsKey(id))
            questProgressGoals.Add(id, goal);
        else
            questProgressGoals[id] = goal;

        if (!questProgress.ContainsKey(id))
            questProgress.Add(id, 0);
    }

    public void AddProgress(string id, int amount)
    {
        if (!questProgress.ContainsKey(id))
            questProgress.Add(id, 0);

        questProgress[id] += amount;

        Debug.Log($"[Quest Progress] {id}: {questProgress[id]}/{questProgressGoals[id]}");

        if (questProgressGoals.ContainsKey(id))
        {
            if (questProgress[id] >= questProgressGoals[id])
            {
                CompleteQuest(id);
            }
        }
    }

}
