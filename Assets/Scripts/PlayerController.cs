using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody))]
public class PlayerController : MonoBehaviour
{
    public float moveSpeed = 5f;
    public float jumpForce = 6f;
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

    void OnEnable()
    {
        inputActions.Enable();
        Cursor.lockState = CursorLockMode.Locked;
    }

    void OnDisable()
    {
        inputActions.Disable();
    }

    void Update()
    {
        // Read look input every frame for smooth rotation
        lookInput = inputActions.Player.Look.ReadValue<Vector2>();
        LookAround();

        if (jumpPressed && isGrounded)
        {
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
            jumpPressed = false;
        }
    }

    void FixedUpdate()
    {
        Move();
    }

    void Move()
    {
        Vector3 direction = transform.right * moveInput.x + transform.forward * moveInput.y;
        Vector3 move = direction.normalized * moveSpeed * Time.fixedDeltaTime;
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
