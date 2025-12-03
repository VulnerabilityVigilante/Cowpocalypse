using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    
    void Start()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    
    public void PlayGame()
    {
        SceneManager.LoadScene("TheRanch");
    }

    public void QuitGame()
    {
        Debug.Log("QUIT pressed — quitting application.");
        Application.Quit();
    }
}
