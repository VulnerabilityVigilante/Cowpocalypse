using UnityEngine;

public class ShopTrigger : MonoBehaviour
{
    private bool playerInRange = false;

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = true;
            InteractUI.Instance.ShowPrompt("Press E to open shop");
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = false;
            InteractUI.Instance.HidePrompt();
            
            // Close the shop if the player walks away
            ShopUI.Instance.Close();
        }
    }

    void Update()
    {
        if (playerInRange && Input.GetKeyDown(KeyCode.E))
        {
            InteractUI.Instance.HidePrompt();
            ShopUI.Instance.Open();
        }
    }
}
