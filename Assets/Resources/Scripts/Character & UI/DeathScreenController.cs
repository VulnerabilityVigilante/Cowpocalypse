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

        ReacquirePlayerReferences();
    }

    void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        ReacquirePlayerReferences();

        // Reset mouse lock after scene reload
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    private void ReacquirePlayerReferences()
    {
        move = FindAnyObjectByType<PlayerController>();
        look = FindAnyObjectByType<PlayerLook>();
        gun = FindAnyObjectByType<Quest3_PlayerGun>();
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
        QuestGiver questGiver = FindAnyObjectByType<QuestGiver>();

        // Save everything
        if (questGiver != null)
            QuestFileSaveSystem.SaveAll(questGiver);

        // Lock before loading
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        // Reload scene fresh
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}
