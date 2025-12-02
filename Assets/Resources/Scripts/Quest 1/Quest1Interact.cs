using UnityEngine;

public class Quest1Interact : MonoBehaviour
{
    public string itemName = "Fence Part";
    bool playerInRange = false;

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = true;
            if (InteractUI.Instance != null)
            {
                InteractUI.Instance.ShowPrompt("Press E to pick up " + itemName);
            }
        }
    }
    
    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = false;
            if (InteractUI.Instance != null)
            {
                InteractUI.Instance.HidePrompt();
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (playerInRange && Input.GetKeyDown(KeyCode.E))
        {
            PickUp();
        }
    }

    void PickUp()
    {
        if (InteractUI.Instance != null)
        {
            InteractUI.Instance.HidePrompt();
        }
        
        Inventory.Instance.AddFencePart();

        if (AnimalSpawner.Instance != null)
        {
            AnimalSpawner.Instance.StartSpawning();
        }
        
        Destroy(gameObject);
    }
}
