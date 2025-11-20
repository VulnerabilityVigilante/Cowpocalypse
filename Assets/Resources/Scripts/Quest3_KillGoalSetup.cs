using UnityEngine;

public class Quest3_KillGoalSetup : MonoBehaviour
{
    [Header("Quest 3 Settings")]
    public string questID = "ShootCowsQuest";
    public int killGoal = 10;

    void Start()
    {
        // Ensure the quest goal is registered
        if (QuestManager.Instance != null)
        {
            QuestManager.Instance.SetQuestGoal(questID, killGoal);
            Debug.Log($"[Quest3] Kill goal for '{questID}' set to {killGoal} kills.");
        }
        else
        {
            Debug.LogError("[Quest3] QuestManager.Instance is NULL. Cannot set kill goal.");
        }
    }
}
