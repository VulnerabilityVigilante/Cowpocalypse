using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class PlayerMovement : MonoBehaviour
{
    [Header("Movement")]
    public float maxGroundSpeed = 10f;
    public float groundAcceleration = 50f;
    public float airAcceleration = 20f;

    [Header("Jumping")]
    public float jumpForce = 5f;
    public float playerHeight = 2f;
    public LayerMask groundMask;

    [Header("References")]
    public Transform orientation;

    private Rigidbody rb;
    private bool grounded;

    private float horizontal;
    private float vertical;
    private Vector3 wishDir;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;
    }

    void Update()
    {
        // Input
        horizontal = Input.GetAxisRaw("Horizontal");
        vertical = Input.GetAxisRaw("Vertical");

        // Ground check
        grounded = Physics.Raycast(transform.position, Vector3.down, playerHeight * 0.5f + 0.3f, groundMask);

        // Jump
        if (Input.GetButtonDown("Jump") && grounded)
        {
            Vector3 vel = rb.linearVelocity;
            vel.y = 0f; // reset vertical velocity
            rb.linearVelocity = vel;
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
        }
    }

    void FixedUpdate()
    {
        // Movement direction
        wishDir = (orientation.forward * vertical + orientation.right * horizontal).normalized;

        if (wishDir.magnitude > 0.1f)
        {
            if (grounded)
                Accelerate(wishDir, groundAcceleration, maxGroundSpeed);
            else
                Accelerate(wishDir, airAcceleration, maxGroundSpeed * 2f); // let air be faster if you want bhop
        }
    }

    void Accelerate(Vector3 wishDir, float accel, float maxVel)
    {
        Vector3 currentVel = rb.linearVelocity;
        float projVel = Vector3.Dot(currentVel, wishDir); // current speed in wishDir
        float addSpeed = maxVel - projVel;

        if (addSpeed <= 0f)
            return;

        float accelSpeed = accel * Time.fixedDeltaTime;
        if (accelSpeed > addSpeed)
            accelSpeed = addSpeed;

        rb.AddForce(wishDir * accelSpeed, ForceMode.VelocityChange);
    }
}
