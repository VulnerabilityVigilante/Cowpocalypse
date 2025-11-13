using UnityEngine;

public class ShopUI : MonoBehaviour
{
    public static ShopUI Instance;
    public GameObject shopPanel;
    public bool ShopIsOpen { get; private set; }


    void Awake()
    {
        Instance = this;
    }

    public void BuyRedbull()
    {
        int cost = 10;

        // Try spending coins
        bool purchased = CurrencyManager.Instance.SpendCoins(cost);

        if (purchased)
        {
            Debug.Log("You bought a Redbull!");
            // TODO: Add to inventory when ready
        }
        else
        {
            Debug.Log("Not enough Schmeckles!");
        }
    }

    public void BuyCigarettes()
    {
        int cost = 30;

        // Try spending coins
        bool purchased = CurrencyManager.Instance.SpendCoins(cost);

        if (purchased)
        {
            Debug.Log("You bought a pack of cigarettes!");
            // TODO: Add to inventory when ready
        }
        else
        {
            Debug.Log("Not enough Schmeckles!");
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
