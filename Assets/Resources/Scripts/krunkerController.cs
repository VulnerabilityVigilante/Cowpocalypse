using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class krunkerController : MonoBehaviour
{
    [Header("Movement Settings")]
    public float accelGround = 500f;     // acceleration on ground
    public float accelAir = 300f;        // acceleration in air
    public float friction = 8f;          // ground friction
    public float maxGroundSpeed = 12f;   // normal running speed
    public float maxAirSpeed = 20f;      // allow faster bhop speeds
    
    [Header("Jumping")]
    public float jumpForce = 8f;
    public float gravity = -30f;

    private CharacterController controller;
    private Vector3 velocity;
    private bool grounded;

    void Start()
    {
        controller = GetComponent<CharacterController>();
    }

    void Update()
    {
        grounded = controller.isGrounded;

        // Input
        float inputX = Input.GetAxisRaw("Horizontal");
        float inputZ = Input.GetAxisRaw("Vertical");
        Vector3 wishDir = (transform.right * inputX + transform.forward * inputZ).normalized;

        // Apply friction when grounded and no input
        if (grounded && wishDir.magnitude < 0.1f)
        {
            velocity.x = Mathf.Lerp(velocity.x, 0, friction * Time.deltaTime);
            velocity.z = Mathf.Lerp(velocity.z, 0, friction * Time.deltaTime);
        }

        // Accelerate toward input direction
        if (wishDir.magnitude > 0.1f)
        {
            float accel = grounded ? accelGround : accelAir;
            float maxSpeed = grounded ? maxGroundSpeed : maxAirSpeed;
            Accelerate(wishDir, accel, maxSpeed);
        }

        // Jump (auto-bhop if you want — use GetButton instead of GetButtonDown)
        if (Input.GetButtonDown("Jump") && grounded)
        {
            velocity.y = jumpForce;
        }

        // Apply gravity
        velocity.y += gravity * Time.deltaTime;

        // Move
        controller.Move(velocity * Time.deltaTime);

        // Debug speed
        Vector3 flatVel = new Vector3(velocity.x, 0, velocity.z);
        Debug.Log("Speed: " + flatVel.magnitude.ToString("F2"));
    }

    void Accelerate(Vector3 wishDir, float accel, float maxSpeed)
    {
        // Project current velocity onto desired direction
        float projVel = Vector3.Dot(velocity, wishDir);
        float addSpeed = maxSpeed - projVel;

        if (addSpeed <= 0)
            return;

        float accelSpeed = accel * Time.deltaTime;
        if (accelSpeed > addSpeed)
            accelSpeed = addSpeed;

        velocity += wishDir * accelSpeed;
    }
}
