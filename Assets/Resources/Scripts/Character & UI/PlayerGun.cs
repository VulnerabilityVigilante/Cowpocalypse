using UnityEngine;

public class Quest3_PlayerGun : MonoBehaviour
{
    public Camera playerCamera;
    public float range = 100f;
    public int damage = 10;

    private GunAudioController audioController;
    private GunMuzzleFlash muzzleFX;   // <-- ADD THIS

    void Start()
    {
        audioController = GetComponent<GunAudioController>();
        muzzleFX = GetComponent<GunMuzzleFlash>();   // <-- ADD THIS
    }

    void Update()
    {
        if (Input.GetButtonDown("Fire1"))
        {
            Shoot();
        }
    }

    void Shoot()
    {
        // NEW: Play smoke regardless of hit
        if (muzzleFX != null)
            muzzleFX.Play();

        RaycastHit hit;
        bool didHitEnemy = false;

        if (Cursor.lockState != CursorLockMode.Locked)
            return;

        if (Physics.Raycast(playerCamera.transform.position,
                            playerCamera.transform.forward,
                            out hit, range))
        {
            Transform root = hit.collider.transform.root;

            if (root.CompareTag("Enemy"))
            {
                didHitEnemy = true;

                Quest3_CowHealth cow = hit.collider.GetComponent<Quest3_CowHealth>();
                if (cow == null)
                    cow = hit.collider.GetComponentInParent<Quest3_CowHealth>();

                if (cow != null)
                    cow.TakeDamage(damage);
            }
        }

        if (audioController != null)
            audioController.PlayShotSound(didHitEnemy);
    }
}