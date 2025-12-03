using UnityEngine;

public class Quest3_PlayerGun : MonoBehaviour
{
    public Camera playerCamera;
    public float range = 100f;
    public int damage = 10;

    private GunAudioController audioController;
    private GunMuzzleFlash muzzleFX;

    void Start()
    {
        audioController = GetComponent<GunAudioController>();
        muzzleFX = GetComponent<GunMuzzleFlash>();
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
        // Block firing if mouse is unlocked
        if (Cursor.lockState != CursorLockMode.Locked)
            return;

        // Play muzzle flash
        if (muzzleFX != null)
            muzzleFX.Play();

        RaycastHit hit;
        bool didHitEnemy = false;

        if (Physics.Raycast(playerCamera.transform.position,
                            playerCamera.transform.forward,
                            out hit, range))
        {
            // =============================
            // UNIVERSAL ENEMY TAG DETECTION
            // Works for:
            //  - Collider tagged Enemy
            //  - Child tagged Enemy
            //  - Root tagged Enemy
            // =============================
            bool isEnemy =
                hit.collider.CompareTag("Enemy") ||
                hit.collider.transform.CompareTag("Enemy") ||
                (hit.collider.transform.parent != null && hit.collider.transform.parent.CompareTag("Enemy")) ||
                hit.collider.transform.root.CompareTag("Enemy");

            if (isEnemy)
            {
                didHitEnemy = true;

                // 1. Try CowHealth
                Quest3_CowHealth cow = hit.collider.GetComponentInParent<Quest3_CowHealth>();
                if (cow != null)
                {
                    cow.TakeDamage(damage);
                    goto PLAY_SOUND;
                }

                // 2. Try GenericAnimalHealth (chickens, pigs, goats, etc.)
                GenericAnimalHealth animal = hit.collider.GetComponentInParent<GenericAnimalHealth>();
                if (animal != null)
                {
                    animal.TakeDamage(damage);
                    goto PLAY_SOUND;
                }
            }
        }

    PLAY_SOUND:
        if (audioController != null)
            audioController.PlayShotSound(didHitEnemy);
    }

}
