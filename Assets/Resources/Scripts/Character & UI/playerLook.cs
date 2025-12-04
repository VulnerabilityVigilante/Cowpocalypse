using UnityEngine;
using UnityEngine.SceneManagement;


public class PlayerLook : MonoBehaviour
{
    [Header("Settings")]
    public float sensitivity = 1f;
    public Transform playerBody;

    private float xRotation = 0f;
    private float mouseX;
    private float mouseY;

    void Start()
    {
        // Only lock if we're in the gameplay scene
        if (SceneManager.GetActiveScene().name == "TheRanch")
        {
            LockCursor(true);
        }
        else
        {
            LockCursor(false);
        }
    }



    void Update()
    {
        // BLOCK MOUSE LOOK WHEN PAUSED
        if (PauseMenu.GameIsPaused)
        {
            LockCursor(false);
            return;
        }

        // Handle resume-from-pause immediate lock
        if (!PauseMenu.GameIsPaused && Cursor.lockState != CursorLockMode.Locked)
        {
            LockCursor(true);
        }

        // Dialogue open blocks look
        if (DialogueUI.Instance != null && DialogueUI.Instance.dialoguePanel.activeSelf)
        {
            LockCursor(false);
            return;
        }

        // Shop open blocks look
        if (ShopUI.Instance != null && ShopUI.Instance.ShopIsOpen)
        {
            LockCursor(false);
            return;
        }

        LockCursor(true);

        mouseX = Input.GetAxisRaw("Mouse X") * sensitivity;
        mouseY = Input.GetAxisRaw("Mouse Y") * sensitivity;
    }

    void LateUpdate()
    {
        // BLOCK CAMERA ROTATION WHEN PAUSED
        if (PauseMenu.GameIsPaused)
            return;

        if (DialogueUI.Instance != null && DialogueUI.Instance.dialoguePanel.activeSelf)
            return;

        if (ShopUI.Instance != null && ShopUI.Instance.ShopIsOpen)
            return;

        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);

        transform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
        playerBody.Rotate(Vector3.up * mouseX);
    }

    void LockCursor(bool locked)
    {
        Cursor.lockState = locked ? CursorLockMode.Locked : CursorLockMode.None;
        Cursor.visible = !locked;
    }
}
