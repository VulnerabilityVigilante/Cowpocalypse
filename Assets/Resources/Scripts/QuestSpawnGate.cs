using UnityEngine;

public class QuestSpawnGate : MonoBehaviour
{
    [Tooltip("Quest ID that must be started before this object activates.")]
    public string requiredQuestID = "GatherWood";

    void Start()
    {
        // Hide object at start if quest isn't started
        if (QuestManager.Instance.GetQuestState(requiredQuestID) == QuestState.NotStarted)
        {
            gameObject.SetActive(false);
        }

        // Subscribe to quest start event (we'll add it below)
        QuestEvents.OnQuestStarted += HandleQuestStarted;
    }

    void OnDestroy()
    {
        QuestEvents.OnQuestStarted -= HandleQuestStarted;
    }

    void HandleQuestStarted(string questID)
    {
        if (questID == requiredQuestID)
        {
            gameObject.SetActive(true);
        }
    }
}
