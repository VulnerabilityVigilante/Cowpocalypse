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
        else
        {
            Destroy(gameObject);
            return;
        }
    }

    void Start()
    {
        // Load coins first from save file
        LoadCoinsFromFile();

        // If no coin entry found in save, start with default
        if (currentCoins == 0 && startingCoins > 0)
            currentCoins = startingCoins;

        UpdateUI();
    }


    private bool SaveFileHasCoins()
    {
        // QuestFileSaveSystem will call SetCoinsFromLoad() if coins exist.
        // If we reach here and currentCoins is still 0, AND startingCoins is 0,
        // then we assume no save file loaded coins.
        return false; 
    }

    public void AddCoins(int amount)
    {
        currentCoins += amount;

        // No PlayerPrefs — saving handled by QuestFileSaveSystem.SaveAll()
        UpdateUI();
    }

    public bool SpendCoins(int amount)
    {
        if (currentCoins >= amount)
        {
            currentCoins -= amount;

            // No PlayerPrefs — saving handled by QuestFileSaveSystem.SaveAll()
            UpdateUI();
            return true;
        }
        return false;
    }

    public void SetCoinsFromLoad(int amount)
    {
        currentCoins = amount;
        UpdateUI();
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

    public void LoadCoinsFromFile()
    {
        string folder = System.IO.Path.Combine(
            System.Environment.GetFolderPath(System.Environment.SpecialFolder.MyDocuments),
            "CowpocalypseSave"
        );

        string path = System.IO.Path.Combine(folder, "save.txt");

        if (!System.IO.File.Exists(path))
            return;

        string[] lines = System.IO.File.ReadAllLines(path);

        foreach (string line in lines)
        {
            if (line.StartsWith("Coins"))
            {
                int amount = int.Parse(line.Split(':')[1]);
                currentCoins = amount;
                UpdateUI();
                return;
            }
        }
    }


}
