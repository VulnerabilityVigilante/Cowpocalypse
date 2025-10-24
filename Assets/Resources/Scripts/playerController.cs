using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(CharacterController))]
public class PlayerController : MonoBehaviour
{
    [Header("Movement")]
    public float moveSpeed = 100f;
    public float sprintSpeed = 200f;
    public float airControl = 0.8f;

    [Header("Jumping")]
    public float jumpHeight = 80f;
    public float gravity = -180f;

    private CharacterController controller;
    private Vector3 velocity;
    private bool grounded;

    private Vector2 moveInput;

    void Start()
    {
        controller = GetComponent<CharacterController>();
    }


    void Update()
    {
        // Ground check
        grounded = controller.isGrounded;
        if (grounded && velocity.y < 0)
            velocity.y = -2f;

        // ✅ Old Input System (works out of the box with WASD/arrow keys)
        float x = Input.GetAxisRaw("Horizontal");
        float z = Input.GetAxisRaw("Vertical");

        Vector3 move = transform.right * x + transform.forward * z;
        float speed = Input.GetKey(KeyCode.LeftShift) ? sprintSpeed : moveSpeed;


        if (grounded)
            controller.Move(move.normalized * speed * Time.deltaTime);
        else
            controller.Move(move.normalized * (speed * airControl) * Time.deltaTime);

        // Jump
        if (Input.GetButtonDown("Jump") && grounded)
            velocity.y = jumpHeight;


        // Gravity
        velocity.y += gravity * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);
    }
}

   