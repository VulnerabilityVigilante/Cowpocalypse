using UnityEngine;

public class Inventory : MonoBehaviour
{
    public static Inventory Instance;   // Singleton for quick access
    public int fenceParts = 0;

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    public int totalLogsCollected = 0;

    public void AddFencePart()
        {
            fenceParts++;
            totalLogsCollected++;
            Debug.Log("Fence parts: " + fenceParts);

            // Check GatherWood quest progress
            if (QuestManager.Instance.GetQuestState("GatherWood") == QuestState.InProgress &&
                totalLogsCollected >= 4)
            {
                QuestManager.Instance.CompleteQuest("GatherWood");
                Debug.Log("✅ GatherWood quest complete (total logs collected).");
            }
        }
}
