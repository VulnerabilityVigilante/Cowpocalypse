using UnityEngine;

public class GunMuzzleFlash : MonoBehaviour
{
    public ParticleSystem muzzleSmoke;

    public void Play()
    {
        if (muzzleSmoke != null)
            muzzleSmoke.Play();
    }
}
