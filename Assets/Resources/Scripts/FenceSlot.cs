using UnityEngine;

public class FenceSlot : MonoBehaviour
{
    public GameObject fencePiecePrefab; // prefab to spawn
    private bool inRange = false;
    private bool isFilled = false;

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && !isFilled)
        {
            inRange = true;

            if (Inventory.Instance.fenceParts > 0)
            {
                InteractUI.Instance.ShowPrompt("Press E to place a fence part.");
            }
            else
            {
                InteractUI.Instance.ShowPrompt("This fence needs repairing, but you don’t have any parts yet.");
            }
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
            inRange = false;
        InteractUI.Instance.HidePrompt();
    }

    void Update()
    {
        if (inRange && !isFilled)
        {
            // Only show prompt if player has parts
            if (Inventory.Instance.fenceParts > 0)
            {
                Debug.Log("Press E to place a fence part.");

                if (Input.GetKeyDown(KeyCode.E))
                {
                    PlaceFencePart();
                }
            }
            else
            {
                Debug.Log("This fence needs repairing, but you don’t have any parts yet.");
            }
        }
    }

    void PlaceFencePart()
    {
        Inventory.Instance.fenceParts--;
        Instantiate(fencePiecePrefab, transform.position, transform.rotation);
        isFilled = true;
        Debug.Log("Fence part placed!");
        InteractUI.Instance.HidePrompt();

        // 👇 NEW: Notify the whisper system each time a fence is placed
        if (FenceManager.Instance != null)
            FenceManager.Instance.NotifyFencePlaced();

        Debug.Log("FenceSlot notifying FenceManager");
        FenceManager.Instance.CheckFenceCompletion();
    }



    public bool IsFilled() => isFilled;
}
