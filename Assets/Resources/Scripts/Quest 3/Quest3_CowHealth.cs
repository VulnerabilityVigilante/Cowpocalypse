using UnityEngine;

public class Quest3_CowHealth : MonoBehaviour
{
    public int maxHealth = 30;
    private int currentHealth;

    void Start()
    {
        currentHealth = maxHealth;
    }

    public void TakeDamage(int amount)
    {
        currentHealth -= amount;

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    void Die()
    {
        Debug.Log($"[Quest3] Cow '{name}' died.");

        // Count cow toward quest progress
        if (QuestManager.Instance != null)
        {
            QuestManager.Instance.AddProgress("ShootCowsQuest", 1);
        }

        Destroy(transform.root.gameObject);
    }
}
