using UnityEngine;

public class Inventory : MonoBehaviour
{
    public static Inventory Instance;   // Singleton for quick access
    public int fenceParts = 0;

    public int redbullCount = 0;
    public int cigaretteCount = 0;
    public int cigaretteCharges = 0; // total uses available



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

    public void AddRedbull()
    {
        redbullCount++;
        Debug.Log("Redbull added. Total: " + redbullCount);
        HealingItemUI.Instance.RefreshRedbull();
    }

    public void AddCigarettes()
    {
        cigaretteCount++;     // number of packs owned
        cigaretteCharges += 3; // each pack = 3 uses
        Debug.Log("Added 1 pack of cigarettes. Packs: " + cigaretteCount + " | Charges: " + cigaretteCharges);

        HealingItemUI.Instance.RefreshCigarettes();
    }


    public bool UseRedbull()
    {
        if (redbullCount <= 0) return false;

        redbullCount--;
        Debug.Log("Used Redbull! Remaining: " + redbullCount);

        PlayerHealth.Instance.Heal(40);   // or whatever amount you want
        
        HealingItemUI.Instance.RefreshRedbull();

        return true;

    }

    public bool UseCigarette()
    {
        if (cigaretteCharges <= 0)
            return false;

        cigaretteCharges--; // use 1 charge

        // If a pack has been completely used up, decrease pack count
        if (cigaretteCharges % 3 == 0)
            cigaretteCount--;

        PlayerHealth.Instance.Heal(20);

        HealingItemUI.Instance.RefreshCigarettes();

        Debug.Log("Used a cigarette. Packs left: " + cigaretteCount + " | Charges left: " + cigaretteCharges);
        return true;
    }


}
