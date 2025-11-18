using UnityEngine;

public class ShopUI : MonoBehaviour
{
    public static ShopUI Instance;
    public GameObject shopPanel;
    public bool ShopIsOpen { get; private set; }

    // 👉 Track what the player has bought during this quest
    private bool boughtRedbull = false;
    private bool boughtCigarettes = false;

    void Awake()
    {
        Instance = this;
    }

    public void BuyRedbull()
    {
        int cost = 10;

        bool purchased = CurrencyManager.Instance.SpendCoins(cost);

        if (purchased)
        {
            Debug.Log("You bought a Redbull!");
            // TODO: Add to inventory when ready

            boughtRedbull = true;
            CheckSpendAtShopQuestProgress();
        }
        else
        {
            Debug.Log("Not enough Schmeckles!");
        }
    }

    public void BuyCigarettes()
    {
        int cost = 30;

        bool purchased = CurrencyManager.Instance.SpendCoins(cost);

        if (purchased)
        {
            Debug.Log("You bought a pack of cigarettes!");
            // TODO: Add to inventory when ready

            boughtCigarettes = true;
            CheckSpendAtShopQuestProgress();
        }
        else
        {
            Debug.Log("Not enough Schmeckles!");
        }
    }

    // 👉 Call this after each successful purchase
    private void CheckSpendAtShopQuestProgress()
    {
        // Make sure the ID string EXACTLY matches the one in QuestManager/QuestGiver
        string questId = "SpendAtShopQuest";

        if (QuestManager.Instance == null) return;

        // Only care if the quest is active
        if (QuestManager.Instance.GetQuestState(questId) == QuestState.InProgress)
        {
            if (boughtRedbull && boughtCigarettes)
            {
                Debug.Log("[ShopUI] Both items bought, completing SpendAtShopQuest.");
                QuestManager.Instance.CompleteQuest(questId);
            }
        }
    }

    public void Open()
    {
        ShopIsOpen = true;

        shopPanel.SetActive(true);
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
    }

    public void Close()
    {
        ShopIsOpen = false;

        shopPanel.SetActive(false);
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }
}
