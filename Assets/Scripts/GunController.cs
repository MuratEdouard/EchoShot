using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class GunController : MonoBehaviour
{
    [Header("Arrow & Muzzle")]
    public GameObject arrowPrefab;
    public Transform muzzleTransform;

    [Header("Animator")]
    public Animator animator;

    [Header("Audio Sources")]
    public AudioSource shootAudio;
    public AudioSource reloadAudio;

    [Header("Freeze Settings")]
    public float freezeDuration = 2f;
    public float reloadDuration = 2f;
    public int maxArrowsPerFreeze = 3;

    private int arrowsPlaced = 0;
    private float freezeTimer = 0f;
    private float reloadTimer = 0f;

    private bool isFrozen = false;
    private bool isCharging = false;
    private bool isReloading = false;

    private bool shootButtonWasReleased = false;

    private PlayerInputActions inputActions;

    void Awake()
    {
        inputActions = new PlayerInputActions();

        inputActions.Player.Shoot.started += ctx =>
        {
            if (shootButtonWasReleased)
            {
                TryShoot();
                shootButtonWasReleased = false;
            }
        };

        inputActions.Player.Shoot.canceled += ctx =>
        {
            shootButtonWasReleased = true;
        };
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
        float delta = Time.deltaTime * GameManager.gameplaySpeed;

        if (isCharging)
        {
            AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(0);
            if (stateInfo.IsName("CrossbowCharge") && stateInfo.normalizedTime >= 1f)
            {
                isCharging = false;
            }
        }

        if (isReloading)
        {
            reloadTimer += delta;
            if (reloadTimer >= reloadDuration)
            {
                isReloading = false;
                reloadTimer = 0f;
            }
            return;
        }

        if (isFrozen)
        {
            freezeTimer += Time.deltaTime;
            if (arrowsPlaced >= maxArrowsPerFreeze || freezeTimer >= freezeDuration)
            {
                ResumeTime();
            }
        }
    }

    void TryShoot()
    {
        if (isReloading || isCharging || arrowsPlaced >= maxArrowsPerFreeze)
            return;

        Shoot();
        arrowsPlaced++;

        if (!isFrozen)
        {
            StartFreeze();
        }
    }

    void Shoot()
    {
        animator.Play("CrossbowShoot");
        shootAudio.Play();
        isCharging = true;
        Instantiate(arrowPrefab, muzzleTransform.position, muzzleTransform.rotation);
    }

    void StartFreeze()
    {
        GameManager.gameplaySpeed = 0.05f;
        isFrozen = true;
        freezeTimer = 0f;
    }

    void ResumeTime()
    {
        GameManager.gameplaySpeed = 1f;
        isFrozen = false;
        isReloading = true;
        reloadTimer = 0f;
        arrowsPlaced = 0;

        reloadAudio.Play();
    }
}
