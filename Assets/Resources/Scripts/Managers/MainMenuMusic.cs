using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class MainMenuMusic : MonoBehaviour
{
    [Header("Music")]
    public AudioClip menuMusic;

    [Header("Settings")]
    public bool loop = true;
    public float volume = 0.1f;

    private AudioSource audioSource;

    void Awake()
    {
        // Get or add AudioSource
        audioSource = GetComponent<AudioSource>();

        // Configure audio source
        audioSource.clip = menuMusic;
        audioSource.loop = loop;
        audioSource.volume = volume;
        audioSource.playOnAwake = false;

        // Start the music
        if (menuMusic != null)
            audioSource.Play();
        else
            Debug.LogWarning("MainMenuMusic: No audio clip assigned!");
    }
}
