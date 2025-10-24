using UnityEngine;

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
        LockCursor(true);
    }

    void Update()
    {
        // If dialogue is open, pause look input and unlock cursor
        if (DialogueUI.Instance != null && DialogueUI.Instance.dialoguePanel.activeSelf)
        {
            LockCursor(false);
            return; // Stop looking while dialogue is active
        }
        else
        {
            LockCursor(true);
        }

        // Raw mouse delta
        mouseX = Input.GetAxisRaw("Mouse X") * sensitivity;
        mouseY = Input.GetAxisRaw("Mouse Y") * sensitivity;
    }

    void LateUpdate()
    {
        // Only rotate camera if dialogue isn't open
        if (DialogueUI.Instance != null && DialogueUI.Instance.dialoguePanel.activeSelf)
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
