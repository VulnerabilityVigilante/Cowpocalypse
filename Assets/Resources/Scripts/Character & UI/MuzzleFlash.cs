using UnityEngine;

public class GunMuzzleFlash : MonoBehaviour
{
    [Header("FX")]
    public ParticleSystem muzzleSmoke;
    public Light muzzleFlashLight;

    [Header("Flash Mesh")]
    public GameObject muzzleFlashQuad;
    public float quadDuration = 0.04f;

    [Header("Light Flash")]
    public float flashDuration = 0.04f;
    public float flashIntensity = 8f;

    private float quadTimer = 0f;
    private float lightTimer = 0f;

    void Awake()
    {
        if (muzzleFlashQuad != null)
            muzzleFlashQuad.SetActive(false);

        if (muzzleFlashLight != null)
            muzzleFlashLight.enabled = false;
    }

    void Update()
    {
        // Disable quad after timer expires
        if (muzzleFlashQuad != null && muzzleFlashQuad.activeSelf)
        {
            quadTimer -= Time.deltaTime;
            if (quadTimer <= 0f)
                muzzleFlashQuad.SetActive(false);
        }

        // Disable light after timer
        if (muzzleFlashLight != null && muzzleFlashLight.enabled)
        {
            lightTimer -= Time.deltaTime;
            if (lightTimer <= 0f)
                muzzleFlashLight.enabled = false;
        }
    }

    public void Play()
    {
        // 1. Smoke FX
        if (muzzleSmoke != null)
            muzzleSmoke.Play();

        // 2. Visible flame quad
        if (muzzleFlashQuad != null)
        {
            muzzleFlashQuad.SetActive(true);
            quadTimer = quadDuration;
        }

        // 3. Light flash
        if (muzzleFlashLight != null)
        {
            muzzleFlashLight.enabled = true;
            muzzleFlashLight.intensity = flashIntensity;
            lightTimer = flashDuration;
        }
    }
}
