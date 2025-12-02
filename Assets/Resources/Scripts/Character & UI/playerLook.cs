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
        // Lock only if no UI is open
        if (DialogueUI.Instance == null && ShopUI.Instance == null)
            LockCursor(true);
    }


    void Update()
    {
        // If dialogue is open, pause look input and unlock cursor
        if (DialogueUI.Instance != null && DialogueUI.Instance.dialoguePanel.activeSelf)
        {
            LockCursor(false);
            return;
        }

        // If shop is open, pause look input and unlock cursor
        if (ShopUI.Instance != null && ShopUI.Instance.ShopIsOpen)
        {
            LockCursor(false);
            return;
        }

        // Otherwise, lock cursor and allow look movement
        LockCursor(true);

        mouseX = Input.GetAxisRaw("Mouse X") * sensitivity;
        mouseY = Input.GetAxisRaw("Mouse Y") * sensitivity;
    }


    void LateUpdate()
    {
        // Stop rotating camera while dialogue is active
        if (DialogueUI.Instance != null && DialogueUI.Instance.dialoguePanel.activeSelf)
            return;

        // Stop rotating camera while shop is open
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
