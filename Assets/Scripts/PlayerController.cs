using UnityEngine;
using System.Collections;

public class PlayerController : MonoBehaviour
{
    // ───────────────────── UI Settings ─────────────────────
    [Header("UI")]
    public CanvasGroup damageFlashGroup;
    public float flashDuration = 0.2f;

    private Coroutine flashCoroutine;

    // ───────────────────── Health Settings ─────────────────────
    [Header("Health Settings")]
    public int maxHits = 3;
    public float recoveryDelay = 5f;
    public AudioSource hurtAudio;

    private int currentHits = 0;
    private float timeSinceLastHit = 0f;
    private bool isDead = false;

    // ───────────────────── Movement Settings ─────────────────────
    [Header("Movement Settings")]
    public float moveSpeed = 5f;

    [Header("Jump Settings")]
    public float jumpVelocity = 12f;
    public int maxJumps = 2;
    public float gravity = -20f;

    [Header("Air Control")]
    public float airAcceleration = 5f;
    public float airDeceleration = 8f;
    public float airControlResponsiveness = 1.5f;

    // ───────────────────── Camera Settings ─────────────────────
    [Header("Camera Settings")]
    public float mouseSensitivity = 2f;
    public Transform cameraTransform;

    // ───────────────────── Footstep Settings ─────────────────────
    [Header("Footstep Settings")]
    public float footstepInterval = 0.4f;

    // ───────────────────── Audio References ─────────────────────
    [Header("Audio Sources")]
    public AudioSource jumpAudio;
    public AudioSource airJumpAudio;
    public AudioSource landAudio;
    public AudioSource walkAudio;

    // Internal
    private Rigidbody rb;
    private float verticalLookRotation = 0f;

    private Vector2 moveInput;
    private Vector2 lookInput;
    private bool jumpPressed;

    private int jumpCount = 0;
    private float velocityY = 0f;
    private Vector3 velocityX = Vector3.zero;

    private PlayerInputActions inputActions;
    private bool IsGrounded = false;
    private bool wasGrounded = false;
    private float footstepTimer = 0f;

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

        // Handle jumping
        if (jumpPressed && jumpCount < maxJumps)
        {
            velocityY = jumpVelocity;
            jumpPressed = false;
            jumpCount++;
            IsGrounded = false;
            if (jumpCount == 1)
                jumpAudio.Play();
            else
                airJumpAudio.Play();
        }

        // Handle footsteps
        if (IsGrounded && moveInput.magnitude > 0.1f)
        {
            footstepTimer += Time.deltaTime;
            if (footstepTimer >= footstepInterval)
            {
                walkAudio.pitch = Random.Range(0.95f, 1.05f); // Optional variation
                walkAudio.Play();
                footstepTimer = 0f;
            }
        }
        else
        {
            walkAudio.Stop();
            footstepTimer = 0f;
        }

        if (!isDead && currentHits > 0)
        {
            timeSinceLastHit += Time.deltaTime;

            if (timeSinceLastHit >= recoveryDelay)
            {
                currentHits = 0;
                Debug.Log("Player recovered");
            }
        }
    }

    void FixedUpdate()
    {
        wasGrounded = IsGrounded;

        float delta = Time.fixedDeltaTime;
        float timeScale = GameManager.gameplaySpeed;

        // Apply gravity
        velocityY += gravity * delta * timeScale;

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
                float alignment = Vector3.Dot(velocityX.normalized, targetVelocity.normalized);
                alignment = (alignment + 1f) * 0.5f;

                float blendFactor = airAcceleration * delta * Mathf.Lerp(0.5f, 2f, alignment) * airControlResponsiveness;
                velocityX = Vector3.Lerp(velocityX, targetVelocity, blendFactor);
            }
            else
            {
                velocityX = Vector3.MoveTowards(velocityX, Vector3.zero, airDeceleration * delta);
            }

            if (velocityX.magnitude > moveSpeed)
                velocityX = velocityX.normalized * moveSpeed;
        }

        Vector3 horizontalMove = velocityX * delta * timeScale;
        Vector3 verticalMove = Vector3.up * velocityY * delta * timeScale;

        rb.MovePosition(rb.position + horizontalMove + verticalMove);
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

                if (!IsGrounded && !wasGrounded)
                {
                    landAudio.Play();
                }

                IsGrounded = true;
                break;
            }
        }
    }

    IEnumerator FlashRed()
    {
        if (damageFlashGroup == null) yield break;

        damageFlashGroup.alpha = 1f;
        float timer = 0f;

        while (timer < flashDuration)
        {
            timer += Time.deltaTime;
            damageFlashGroup.alpha = Mathf.Lerp(1f, 0f, timer / flashDuration);
            yield return null;
        }

        damageFlashGroup.alpha = 0f;
    }

    public void TakeLaserHit()
    {
        if (isDead) return;

        currentHits++;
        timeSinceLastHit = 0f;

        if (hurtAudio) hurtAudio.Play();

        Debug.Log($"Player hit! {currentHits}/{maxHits}");

        if (currentHits >= maxHits)
        {
            Die();
        }
    }

    void Die()
    {
        isDead = true;

        Debug.Log("Player died!");

        // Disable movement
        inputActions.Disable();
        rb.linearVelocity = Vector3.zero;
        rb.isKinematic = false; // Optional: fall to ground
        rb.constraints = RigidbodyConstraints.None;

        // Optional: play death animation, ragdoll, or reload scene
    }


}
