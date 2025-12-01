using UnityEngine;
using UnityEngine.SceneManagement;

public class DeathScreenController : MonoBehaviour
{
    [Header("UI")]
    public GameObject deathScreen;

    private PlayerController move;
    private PlayerLook look;
    private Quest3_PlayerGun gun;

    void Start()
    {
        if (deathScreen != null)
            deathScreen.SetActive(false);

        // Automatically find player scripts
        move = FindObjectOfType<PlayerController>();
        look = FindObjectOfType<PlayerLook>();
        gun = FindObjectOfType<Quest3_PlayerGun>();
    }

    public void ShowDeathScreen()
    {
        if (deathScreen != null)
            deathScreen.SetActive(true);

        // Unlock mouse for UI
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        // Disable scripts if they exist
        if (move != null) move.enabled = false;
        if (look != null) look.enabled = false;
        if (gun != null) gun.enabled = false;
    }

    // Button Hook
    public void RestartScene()
    {
        QuestGiver questGiver = FindObjectOfType<QuestGiver>();

        // Save everything to save.txt
        if (questGiver != null)
            QuestFileSaveSystem.SaveAll(questGiver);

        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }



}
