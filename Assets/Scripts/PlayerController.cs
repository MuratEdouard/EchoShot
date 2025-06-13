using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float moveSpeed = 5f;
    public float jumpVelocity = 12f; // Initial upward velocity
    public float gravity = -20f;    // Custom gravity (stronger for snappy feel)
    public float mouseSensitivity = 2f;
    public Transform cameraTransform;

    public float airAcceleration = 5f;   // How fast to reach target velocity in air
    public float airDeceleration = 8f;   // How fast to slow down in air
    public float airControlResponsiveness = 1.5f; // Air control multiplier (more = better control)

    private Rigidbody rb;
    private float verticalLookRotation = 0f;

    private Vector2 moveInput;
    private Vector2 lookInput;
    private bool jumpPressed;

    private int jumpCount = 0;
    private int maxJumps = 2;

    private float velocityY = 0f; // Custom vertical velocity
    private Vector3 velocityX = Vector3.zero; // Horizontal velocity

    private PlayerInputActions inputActions;

    private bool IsGrounded = false; // Track grounded state

    void Awake()
    {
        inputActions = new PlayerInputActions();

        inputActions.Player.Move.performed += ctx => moveInput = ctx.ReadValue<Vector2>();
        inputActions.Player.Move.canceled += ctx => moveInput = Vector2.zero;

        inputActions.Player.Jump.performed += ctx => jumpPressed = true;

        rb = GetComponent<Rigidbody>();
        rb.constraints = RigidbodyConstraints.FreezeRotation;
        rb.interpolation = RigidbodyInterpolation.None;
        rb.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;
    }

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void OnEnable()
    {
        inputActions.Enable();
    }

    void OnDisable()
    {
        inputActions.Disable();
    }

    void Update()
    {
        float speedFactor = Mathf.Lerp(1f, 0.5f, 1f - GameManager.gameplaySpeed);
        lookInput = inputActions.Player.Look.ReadValue<Vector2>() * speedFactor;
        LookAround();

        if (jumpPressed && jumpCount < maxJumps)
        {
            velocityY = jumpVelocity;
            jumpPressed = false;
            jumpCount++;
            IsGrounded = false; // Left ground on jump
        }
    }

    void FixedUpdate()
    {
        float delta = Time.fixedDeltaTime;
        float timeScale = GameManager.gameplaySpeed;

        // Apply gravity (vertical velocity)
        velocityY += gravity * delta * timeScale;

        // Calculate target horizontal velocity (XZ plane)
        Vector3 inputDirection = transform.right * moveInput.x + transform.forward * moveInput.y;
        Vector3 targetVelocity = inputDirection.normalized * moveSpeed;

        if (IsGrounded)
        {
            velocityX = targetVelocity;
        }
        else
        {
            if (moveInput.magnitude > 0.1f)
            {
                // Alignment-based air control
                float alignment = Vector3.Dot(velocityX.normalized, targetVelocity.normalized);
                alignment = (alignment + 1f) * 0.5f; // [-1,1] → [0,1]

                float blendFactor = airAcceleration * delta * Mathf.Lerp(0.5f, 2f, alignment) * airControlResponsiveness;
                velocityX = Vector3.Lerp(velocityX, targetVelocity, blendFactor);
            }
            else
            {
                velocityX = Vector3.MoveTowards(velocityX, Vector3.zero, airDeceleration * delta);
            }

            // Optional: Clamp to max speed
            if (velocityX.magnitude > moveSpeed)
                velocityX = velocityX.normalized * moveSpeed;
        }

        // Movement vectors
        Vector3 horizontalMove = velocityX * delta * timeScale;
        Vector3 verticalMove = Vector3.up * velocityY * delta * timeScale;

        // Apply movement
        rb.MovePosition(rb.position + horizontalMove + verticalMove);

        // Prevent physics drift
        rb.linearVelocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
    }

    void LookAround()
    {
        float mouseX = lookInput.x * mouseSensitivity;
        float mouseY = lookInput.y * mouseSensitivity;

        transform.Rotate(Vector3.up * mouseX);

        verticalLookRotation -= mouseY;
        verticalLookRotation = Mathf.Clamp(verticalLookRotation, -90f, 90f);
        cameraTransform.localEulerAngles = new Vector3(verticalLookRotation, 0f, 0f);
    }

    void OnCollisionStay(Collision collision)
    {
        foreach (ContactPoint contact in collision.contacts)
        {
            if (Vector3.Dot(contact.normal, Vector3.up) > 0.5f)
            {
                velocityY = 0f;
                jumpCount = 0;
                IsGrounded = true;
                break;
            }
        }
    }
}
