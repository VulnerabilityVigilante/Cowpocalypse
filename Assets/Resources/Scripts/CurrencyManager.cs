using UnityEngine;
using TMPro;

public class CurrencyManager : MonoBehaviour
{
    public static CurrencyManager Instance;

    [Header("Settings")]
    public int startingCoins = 0;

    [Header("UI")]
    public TextMeshProUGUI currencyText;

    private int currentCoins;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else Destroy(gameObject);
    }

    void Start()
    {
        currentCoins = startingCoins;
        UpdateUI();
    }

    public void AddCoins(int amount)
    {
        currentCoins += amount;
        UpdateUI();
    }

    public bool SpendCoins(int amount)
    {
        if (currentCoins >= amount)
        {
            currentCoins -= amount;
            UpdateUI();
            return true;
        }
        return false;
    }

    public int GetCoins()
    {
        return currentCoins;
    }

    private void UpdateUI()
    {
        if (currencyText != null)
            currencyText.text = currentCoins.ToString();
    }
}
