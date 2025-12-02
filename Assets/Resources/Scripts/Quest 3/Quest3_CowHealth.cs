using UnityEngine;

public class Quest3_CowHealth : MonoBehaviour
{
    public int maxHealth = 30;
    private int currentHealth;

    [Header("FX")]
    public GameObject deathExplosionPrefab;  

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

        // Spawn explosion FX
        if (deathExplosionPrefab != null)
        {
            Instantiate(
                deathExplosionPrefab,
                transform.position,
                Quaternion.identity
            );
        }

        // Count cow toward quest progress
        if (QuestManager.Instance != null)
        {
            QuestManager.Instance.AddProgress("ShootCowsQuest", 1);
        }

        // Destroy entire cow root
        Destroy(transform.root.gameObject);
    }

    public void TriggerDeathExplosionOnly()
    {
        if (deathExplosionPrefab != null)
        {
            Instantiate(
                deathExplosionPrefab,
                transform.position,
                Quaternion.identity
            );
        }
    }

}
