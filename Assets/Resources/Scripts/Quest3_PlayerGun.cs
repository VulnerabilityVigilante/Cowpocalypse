using UnityEngine;

public class Quest3_PlayerGun : MonoBehaviour
{
    public Camera playerCamera;
    public float range = 100f;
    public int damage = 10;

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

        if (Physics.Raycast(playerCamera.transform.position, playerCamera.transform.forward, out hit, range))
        {
            // Check if we hit a quest cow
            Quest3_CowHealth cow = hit.transform.GetComponent<Quest3_CowHealth>();

            if (cow != null)
            {
                cow.TakeDamage(damage);
            }
        }
    }
}
