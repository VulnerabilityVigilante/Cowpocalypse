using UnityEngine;
using TMPro;

public class HealingItemUI : MonoBehaviour
{
    public static HealingItemUI Instance;

    [Header("Text Displays")]
    public TextMeshProUGUI redbullText;
    public TextMeshProUGUI cigaretteText;

    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        RefreshAll();
    }

    public void RefreshAll()
    {
        RefreshRedbull();
        RefreshCigarettes();
    }

    public void RefreshRedbull()
    {
        redbullText.text = Inventory.Instance.redbullCount.ToString();
    }

    public void RefreshCigarettes()
    {
        cigaretteText.text = Inventory.Instance.cigaretteCount.ToString();
    }
}
