using UnityEngine;

public class HealingItemUse : MonoBehaviour
{
    [Header("Healing Sounds (assign MP3s)")]
    public AudioClip redbullSound;     // drinking sound
    public AudioClip cigaretteSound;   // puff sound

    private AudioSource audioSource;

    void Awake()
    {
        // Create a self-contained AudioSource on this object
        audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.playOnAwake = false;
        audioSource.spatialBlend = 0f; // UI sound (2D)
        audioSource.volume = 0.8f;
    }

    void Update()
    {
        // Use Redbull (1)
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            if (Inventory.Instance.UseRedbull())
            {
                PlayHealingSound(redbullSound);
            }
            else
            {
                Debug.Log("No Redbull!");
            }
        }

        // Use Cigarette (2)
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            if (Inventory.Instance.UseCigarette())
            {
                PlayHealingSound(cigaretteSound);
            }
            else
            {
                Debug.Log("No Cigarettes!");
            }
        }
    }

    private void PlayHealingSound(AudioClip clip)
    {
        if (clip != null && audioSource != null)
        {
            // Extreme pitch variance
            audioSource.pitch = Random.Range(0.7f, 1.4f);

            audioSource.PlayOneShot(clip);
        }
        else
        {
            Debug.LogWarning("Healing sound not assigned in Inspector!");
        }
    }

}
