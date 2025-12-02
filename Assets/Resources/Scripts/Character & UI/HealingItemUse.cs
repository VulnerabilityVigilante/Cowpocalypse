using UnityEngine;

public class HealingItemUse : MonoBehaviour
{
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))   // press 1 to use Redbull
        {
            if (!Inventory.Instance.UseRedbull())
                Debug.Log("No Redbull!");
        }

        if (Input.GetKeyDown(KeyCode.Alpha2))   // press 2 to use Cigarettes
        {
            if (!Inventory.Instance.UseCigarette())
                Debug.Log("No Cigarettes!");
        }
    }
}
