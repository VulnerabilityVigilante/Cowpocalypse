using UnityEngine;
using UnityEngine.UI;

public class PlayerHealth : MonoBehaviour
{
    [Header("Health Settings")]
    public int maxHealth = 100;
    public int currentHealth;

    [Header("UI References")]
    public Slider healthBar;

    public static PlayerHealth Instance;


    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        currentHealth = maxHealth;

        if (healthBar != null)
            healthBar.value = 1f; // full HP
    }

    public void TakeDamage(int amount)
    {
        currentHealth -= amount;

        if (currentHealth < 0)
            currentHealth = 0;

        if (healthBar != null)
            healthBar.value = (float)currentHealth / maxHealth;

        if (currentHealth <= 0)
            Die();
    }

    void Die()
    {
        Debug.Log("PLAYER DIED!");

        // RESTORE HEALING ITEMS BEFORE SAVING
        Inventory.Instance.RestoreHealingItemsAfterDeath();

        QuestGiver questGiver = FindAnyObjectByType<QuestGiver>();

        // Reset active quest so it restarts properly
        if (questGiver != null)
        {
            string active = questGiver.GetActiveQuestID();
            if (!string.IsNullOrEmpty(active))
            {
                Quest q = QuestManager.Instance.quests.Find(x => x.questID == active);
                if (q != null)
                    q.state = QuestState.NotStarted;
            }

            // NOW save with restored consumables
            QuestFileSaveSystem.SaveAll(questGiver);
        }

        // Stop spawner
        if (Quest3_CowWaveSpawner.Instance != null)
            Quest3_CowWaveSpawner.Instance.StopAllCowSpawning();

        // Show death UI
        DeathScreenController dsc = FindAnyObjectByType<DeathScreenController>();
        if (dsc != null)
            dsc.ShowDeathScreen();
    }


    public void Heal(int amount)
    {
        currentHealth += amount;

        if (currentHealth > maxHealth)
            currentHealth = maxHealth;

        if (healthBar != null)
            healthBar.value = (float)currentHealth / maxHealth;

        Debug.Log("Healed " + amount + " HP. Current HP: " + currentHealth);
    }






}
