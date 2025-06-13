using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody))]
public class PlayerController : MonoBehaviour
{
    public float moveSpeed = 5f;
    public float jumpForce = 10f;
    public float mouseSensitivity = 2f;
    public Transform cameraTransform;

    private Rigidbody rb;
    private float verticalLookRotation = 0f;
    private bool isGrounded;

    private Vector2 moveInput;
    private Vector2 lookInput;
    private bool jumpPressed;

    private PlayerInputActions inputActions;

    void Awake()
    {
        inputActions = new PlayerInputActions();

        inputActions.Player.Move.performed += ctx => moveInput = ctx.ReadValue<Vector2>();
        inputActions.Player.Move.canceled += ctx => moveInput = Vector2.zero;

        inputActions.Player.Jump.performed += ctx => jumpPressed = true;

        rb = GetComponent<Rigidbody>();
        rb.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;
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
        // Slow down sight a bit
        float speedFactor = Mathf.Lerp(1f, 0.5f, 1f - GameManager.gameplaySpeed);
        lookInput = inputActions.Player.Look.ReadValue<Vector2>() * speedFactor;
        LookAround();

        // Delay jump based on gameplay speed
        if (jumpPressed && isGrounded)
        {
            rb.AddForce(Vector3.up * jumpForce * GameManager.gameplaySpeed, ForceMode.Impulse);
            jumpPressed = false;
        }
    }

    void FixedUpdate()
    {
        Move();
        
        // Custom gravity scaled by gameplaySpeed
        Vector3 gravity = Physics.gravity * GameManager.gameplaySpeed;
        rb.AddForce(gravity, ForceMode.Acceleration);
    }

    void Move()
    {
        float delta = Time.unscaledDeltaTime * GameManager.gameplaySpeed;

        Vector3 direction = transform.right * moveInput.x + transform.forward * moveInput.y;
        Vector3 move = direction.normalized * moveSpeed * delta;
        Vector3 newPos = rb.position + move;
        rb.MovePosition(newPos);
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
        isGrounded = true;
    }

    void OnCollisionExit(Collision collision)
    {
        isGrounded = false;
    }
}
