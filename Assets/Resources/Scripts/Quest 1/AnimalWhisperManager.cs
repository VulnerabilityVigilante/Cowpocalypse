using UnityEngine;

public class AnimalWhisperManager : MonoBehaviour
{
    public static AnimalWhisperManager Instance;

    [Header("Audio Settings")]
    public AudioSource whisperSource;
    public AnimationCurve volumeCurve; // X = % of fences fixed, Y = volume (0–1)

    private int totalFences;
    private int fencesFixed;

    void Awake()
    {
        Instance = this;
    }

    public void Initialize(int totalFenceCount)
    {
        totalFences = totalFenceCount;
        fencesFixed = 0;

        if (whisperSource != null)
        {
            whisperSource.loop = true;
            whisperSource.Play();
        }
    }

    public void OnFenceFixed()
    {
        fencesFixed++;
        float progress = Mathf.Clamp01((float)fencesFixed / totalFences);

        if (whisperSource != null)
        {
            float targetVolume = volumeCurve != null ? volumeCurve.Evaluate(progress) : progress;
            StopAllCoroutines(); // stop any existing fade
            StartCoroutine(SmoothIncrease(targetVolume, 1.5f)); // smooth over 1.5 seconds
        }

        if (fencesFixed >= totalFences)
        {
            FadeOutWhispers();
        }
    }

    private System.Collections.IEnumerator SmoothIncrease(float targetVolume, float duration)
    {
        float startVolume = whisperSource.volume;
        float time = 0f;

        while (time < duration)
        {
            time += Time.deltaTime;
            whisperSource.volume = Mathf.Lerp(startVolume, targetVolume, time / duration)
                        + Random.Range(-0.01f, 0.01f);

            yield return null;
        }

        whisperSource.volume = targetVolume;
    }


    void FadeOutWhispers()
    {
        if (whisperSource == null) return;

        // 👻 Sudden cutoff — the air goes dead
        whisperSource.Stop();
        whisperSource.volume = 0f;
        Debug.Log("🔇 Whispers abruptly stopped — the world falls silent.");
    }


    private System.Collections.IEnumerator FadeOutRoutine()
    {
        float startVol = whisperSource.volume;
        float t = 0f;

        while (t < 1f)
        {
            t += Time.deltaTime / 2f; // fade out over 2 seconds
            whisperSource.volume = Mathf.Lerp(startVol, 0f, t);
            yield return null;
        }

        whisperSource.Stop();
    }
}
