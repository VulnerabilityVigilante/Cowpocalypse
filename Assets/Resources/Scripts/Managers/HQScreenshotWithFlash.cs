using UnityEngine;
using UnityEngine.UI;
using System.IO;
using System.Collections;

public class HQScreenshotWithFlash : MonoBehaviour
{
    [Header("Screenshot Settings")]
    public Camera targetCamera;
    public KeyCode screenshotKey = KeyCode.F12;
    public int resolutionMultiplier = 1;

    [Header("Flash UI")]
    public Image flashImage;         // Assign a full-screen UI Image here
    public float flashDuration = 0.25f;

    [Header("Shutter Sound")]
    public AudioClip shutterSound;   // MP3/WAV/etc.
    private AudioSource audioSource;

    void Start()
    {
        // Create a self-contained AudioSource
        audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.playOnAwake = false;
        audioSource.volume = 0.3f;
    }

    void Update()
    {
        if (Input.GetKeyDown(screenshotKey))
            StartCoroutine(CaptureScreenshotRoutine());
    }

    IEnumerator CaptureScreenshotRoutine()
    {
        // Flash effect
        if (flashImage != null)
            StartCoroutine(FlashEffect());

        // Play shutter sound
        if (shutterSound != null)
            audioSource.PlayOneShot(shutterSound);

        // Wait one frame so flash & audio trigger immediately
        yield return new WaitForEndOfFrame();

        // Now take the HQ screenshot
        CaptureHQScreenshot();
    }

    void CaptureHQScreenshot()
    {
        RenderTexture rt = new RenderTexture(Screen.width * resolutionMultiplier, Screen.height * resolutionMultiplier, 24);
        targetCamera.targetTexture = rt;

        Texture2D screenshot = new Texture2D(rt.width, rt.height, TextureFormat.RGB24, false);

        targetCamera.Render();
        RenderTexture.active = rt;
        screenshot.ReadPixels(new Rect(0, 0, rt.width, rt.height), 0, 0);

        targetCamera.targetTexture = null;
        RenderTexture.active = null;
        Destroy(rt);

        byte[] bytes = screenshot.EncodeToPNG();

        // Save path: Pictures/Cowpocalypse Screenshots
        string folder = Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.MyPictures), "Cowpocalypse Screenshots");
        Directory.CreateDirectory(folder);

        string timestamp = System.DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss");
        string filePath = Path.Combine(folder, $"Cowpocalypse_{timestamp}.png");

        File.WriteAllBytes(filePath, bytes);

        Debug.Log($"HQ Screenshot saved to: {filePath}");
    }

    IEnumerator FlashEffect()
    {
        // Enable image so it actually renders
        flashImage.gameObject.SetActive(true);

        // Start fully visible
        flashImage.color = new Color(1, 1, 1, 1);

        float t = 0;
        while (t < flashDuration)
        {
            t += Time.deltaTime;
            float alpha = Mathf.Lerp(1f, 0f, t / flashDuration);
            flashImage.color = new Color(1, 1, 1, alpha);
            yield return null;
        }

        // Disable after fading out
        flashImage.gameObject.SetActive(false);
    }

}
