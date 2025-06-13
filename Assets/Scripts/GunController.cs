using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class GunController : MonoBehaviour
{
    public GameObject bulletPrefab;
    public Transform muzzleTransform;

    [Header("Freeze Settings")]
    public float freezeDuration = 2f;
    public float reloadDuration = 2f;
    public int maxBulletsPerFreeze = 3;
    public float freezeDelay = 0.5f;

    private int bulletsPlaced = 0;
    private float freezeTimer = 0f;
    private float reloadTimer = 0f;

    private bool isFrozen = false;
    private bool isReloading = false;

    private PlayerInputActions inputActions;

    void Awake()
    {
        inputActions = new PlayerInputActions();
        inputActions.Player.PlaceBullet.performed += ctx => TryPlaceBullet();
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
            if (bulletsPlaced >= maxBulletsPerFreeze || freezeTimer >= freezeDuration)
            {
                ResumeTime();
            }
        }
    }

    void TryPlaceBullet()
    {
        if (isReloading || bulletsPlaced >= maxBulletsPerFreeze)
            return;

        PlaceBullet();
        bulletsPlaced++;

        if (!isFrozen)
        {
            StartFreeze();
        }
    }

    void PlaceBullet()
    {
        Instantiate(bulletPrefab, muzzleTransform.position, muzzleTransform.rotation);
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
        bulletsPlaced = 0;
    }
}
