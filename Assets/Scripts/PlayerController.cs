using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float moveSpeed = 5f;
    public float jumpVelocity = 12f; // Initial upward velocity
    public float gravity = -20f;    // Custom gravity (stronger for snappy feel)
    public float mouseSensitivity = 2f;
    public Transform cameraTransform;

    private Rigidbody rb;
    private float verticalLookRotation = 0f;

    private Vector2 moveInput;
    private Vector2 lookInput;
    private bool jumpPressed;

    private int jumpCount = 0;
    private int maxJumps = 2;

    private float velocityY = 0f; // Custom vertical velocity

    private PlayerInputActions inputActions;

    void Awake()
    {
        inputActions = new PlayerInputActions();

        inputActions.Player.Move.performed += ctx => moveInput = ctx.ReadValue<Vector2>();
        inputActions.Player.Move.canceled += ctx => moveInput = Vector2.zero;

        inputActions.Player.Jump.performed += ctx => jumpPressed = true;

        rb = GetComponent<Rigidbody>();
        rb.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;
        rb.interpolation = RigidbodyInterpolation.None;
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
        }
    }

    void FixedUpdate()
    {
        float delta = Time.fixedDeltaTime;
        float timeScale = GameManager.gameplaySpeed;

        // Gravity is always applied with real-world delta
        velocityY += gravity * delta * timeScale;

        // Horizontal move is scaled by game time
        Vector3 direction = transform.right * moveInput.x + transform.forward * moveInput.y;
        Vector3 horizontalMove = direction.normalized * moveSpeed * delta * timeScale;

        // Vertical move ALSO scaled by game time
        Vector3 verticalMove = Vector3.up * velocityY * delta * timeScale;

        // Final movement
        rb.MovePosition(rb.position + horizontalMove + verticalMove);
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
        // Simple grounding logic
        if (velocityY < 0f)
        {
            velocityY = 0f;
            jumpCount = 0;
        }
    }
}
