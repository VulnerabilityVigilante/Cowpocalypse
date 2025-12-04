using UnityEngine;

public class GenericAnimalHealth : MonoBehaviour
{
    [Header("Health Settings")]
    public int maxHealth = 30;
    private int currentHealth;

    [Header("Death FX")]
    public GameObject deathExplosionPrefab;

    void Start()
    {
        currentHealth = maxHealth;
    }

    // Apply damage to animal.
    public void TakeDamage(int amount)
    {
        currentHealth -= amount;

        if (currentHealth <= 0)
            Die();
    }

    // Handles full death: explosion FX + destroying the root object.
    void Die()
    {
        Debug.Log($"[Animal] '{name}' died.");

        // Spawn explosion FX if assigned
        if (deathExplosionPrefab != null)
        {
            Instantiate(
                deathExplosionPrefab,
                transform.position,
                Quaternion.identity
            );
        }

        // Destroy the entire prefab root
        Destroy(transform.root.gameObject);
    }

    // For situations where only the explosion should be triggered (mass kills, despawn, etc.)
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
