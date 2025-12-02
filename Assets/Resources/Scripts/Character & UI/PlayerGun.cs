using UnityEngine;

public class Quest3_PlayerGun : MonoBehaviour
{
    public Camera playerCamera;
    public float range = 100f;
    public int damage = 10;

    private GunAudioController audioController;

    void Start()
    {
        audioController = GetComponent<GunAudioController>();
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
        RaycastHit hit;
        bool didHitEnemy = false;

        if (Cursor.lockState != CursorLockMode.Locked)
            return;


        if (Physics.Raycast(playerCamera.transform.position,
                            playerCamera.transform.forward,
                            out hit, range))
        {
            Transform root = hit.collider.transform.root;

            // Tag lives on ROOT
            if (root.CompareTag("Enemy"))
            {
                didHitEnemy = true;

                // CowHealth lives on the CHILD we hit, not on root
                Quest3_CowHealth cow = hit.collider.GetComponent<Quest3_CowHealth>();

                if (cow == null)
                {
                    // If raycast hits a child of the child, climb upward
                    cow = hit.collider.GetComponentInParent<Quest3_CowHealth>();
                }

                if (cow != null)
                {
                    cow.TakeDamage(damage);
                }
            }
        }

        // Play audio based on real Enemy hit only
        if (audioController != null)
            audioController.PlayShotSound(didHitEnemy);
    }

}
