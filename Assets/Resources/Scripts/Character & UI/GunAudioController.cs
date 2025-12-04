using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class GunAudioController : MonoBehaviour
{
    [Header("Audio Clips")]
    public AudioClip fireSound; // MISS sound
    public AudioClip hitSound;  // HIT sound

    [Header("Pitch Variation")]
    [Range(0.5f, 2f)] public float minPitch = 0.9f;
    [Range(0.5f, 2f)] public float maxPitch = 1.1f;

    private AudioSource audioSource;

    void Awake()
    {
        audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.spatialBlend = 0f;
        audioSource.playOnAwake = false;
        audioSource.loop = false;
        audioSource.volume = 0.1f;
    }

    void OnEnable()
    {
        if (audioSource != null)
            audioSource.pitch = 1f;
    }

    // Called by PlayerGun. TRUE = hit, FALSE = miss.
    public void PlayShotSound(bool hit)
    {
        if (!gameObject.activeInHierarchy)
            return;

        audioSource.pitch = Random.Range(minPitch, maxPitch);

        if (hit)
        {
            if (hitSound != null)
                audioSource.PlayOneShot(hitSound);
        }
        else
        {
            if (fireSound != null)
                audioSource.PlayOneShot(fireSound);
        }
    }
}
