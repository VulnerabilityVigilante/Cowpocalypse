using UnityEngine;
using UnityEngine.SceneManagement;

public class Inventory : MonoBehaviour
{
    public static Inventory Instance;   // Singleton for quick access
    public int fenceParts = 0;

    public int redbullCount = 0;
    public int cigaretteCount = 0;
    public int cigaretteCharges = 0; // total uses available


    // Temporary backup for combat quests
    private int saved_redbulls = 0;
    private int saved_cigPacks = 0;
    private int saved_cigCharges = 0;




    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }

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
            Debug.Log("GatherWood quest complete (total logs collected).");
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

    // Call when combat quest starts
    public void SaveHealingItemsForQuest()
    {
        saved_redbulls = redbullCount;
        saved_cigPacks = cigaretteCount;
        saved_cigCharges = cigaretteCharges;

        Debug.Log("Saved healing items for combat quest.");
    }

    // Call when player dies and presses Restart
    public void RestoreHealingItemsAfterDeath()
    {
        redbullCount = saved_redbulls;
        cigaretteCount = saved_cigPacks;
        cigaretteCharges = saved_cigCharges;

        HealingItemUI.Instance.RefreshAll();
        Debug.Log("Restored healing items after quest death.");
    }

    // Call when quest completes successfully
    public void ClearQuestHealingBackup()
    {
        saved_redbulls = 0;
        saved_cigPacks = 0;
        saved_cigCharges = 0;

        Debug.Log("Cleared combat quest healing backup.");
    }


    void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // Load consumables from save file into the new Inventory instance
        QuestFileSaveSystem.LoadAll();

        // Then refresh UI
        if (HealingItemUI.Instance != null)
            HealingItemUI.Instance.RefreshAll();
    }

    public void LoadHealingItemsForQuest()
    {
        // If no backup was created (quest wasn’t started this session), do nothing
        if (saved_redbulls == 0 &&
            saved_cigPacks == 0 &&
            saved_cigCharges == 0)
        {
            Debug.Log("No saved healing items for quest. Leaving current values as-is.");
            return;
        }

        redbullCount = saved_redbulls;
        cigaretteCount = saved_cigPacks;
        cigaretteCharges = saved_cigCharges;

        HealingItemUI.Instance.RefreshAll();

        Debug.Log("Restored quest healing items after loading into active combat quest.");
    }

    public void RebuildHealingBackupFromCurrentValues()
    {
        saved_redbulls = redbullCount;
        saved_cigPacks = cigaretteCount;
        saved_cigCharges = cigaretteCharges;

        Debug.Log("Rebuilt healing backup from current save file because quest is already active.");
    }

    public bool HasNoQuestBackup()
    {
        return saved_redbulls == 0 && saved_cigPacks == 0 && saved_cigCharges == 0;
    }


}
