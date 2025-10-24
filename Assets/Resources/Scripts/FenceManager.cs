using UnityEngine;

public class FenceManager : MonoBehaviour
{
    public static FenceManager Instance;

    [Header("Fence Settings")]
    public int logsRequired = 4;
    public FenceSlot[] slots; // assign in Inspector

    private void Awake()
    {
        Instance = this;

        if (AnimalWhisperManager.Instance != null)
            AnimalWhisperManager.Instance.Initialize(logsRequired);
    }

    private void Update()
    {
        // Watch inventory for GatherWood completion
        if (QuestManager.Instance.GetQuestState("GatherWood") == QuestState.InProgress &&
            Inventory.Instance.fenceParts >= logsRequired)
        {
            QuestManager.Instance.CompleteQuest("GatherWood");
            Debug.Log("✅ GatherWood quest complete!");
        }
    }

    public void CheckFenceCompletion()
    {
        if (QuestManager.Instance.GetQuestState("FixFence") != QuestState.InProgress)
            return;

        foreach (FenceSlot slot in slots)
        {
            if (!slot.IsFilled()) return; // not done yet
        }

        // All slots are filled
        QuestManager.Instance.CompleteQuest("FixFence");
        Debug.Log("✅ FixFence quest complete!");

        if (AnimalSpawner.Instance != null)
        {
            AnimalSpawner.Instance.OnFenceFixed();
        }
    }

    public void NotifyFencePlaced()
    {
        if (AnimalWhisperManager.Instance != null)
        {
            AnimalWhisperManager.Instance.OnFenceFixed();
        }
    }

}
