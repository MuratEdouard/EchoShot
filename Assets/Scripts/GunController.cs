using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class GunController : MonoBehaviour
{
    public GameObject bulletPrefab;
    public Transform muzzleTransform;
    public int maxBullets = 10;
    public float fireInterval = 0.2f;
    public float placementInterval = 0.15f;

    private readonly Queue<GameObject> bulletsQueue = new Queue<GameObject>();
    private float fireTimer = 0f;
    private float placeTimer = 0f;
    private bool isFiring = false;
    private bool placeBulletRequest = false;

    private PlayerInputActions inputActions;

    void Awake()
    {
        inputActions = new PlayerInputActions();

        inputActions.Player.PlaceBullet.performed += _ => placeBulletRequest = true;
        inputActions.Player.FireBullet.started += _ => isFiring = true;
        inputActions.Player.FireBullet.canceled += _ =>
        {
            isFiring = false;
            fireTimer = 0f;
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
        // Place bullet with interval
        placeTimer += Time.deltaTime;
        if (placeBulletRequest && placeTimer >= placementInterval)
        {
            SpawnBullet();
            placeTimer = 0f;
            placeBulletRequest = false;
        }

        // Fire bullets with interval
        if (isFiring && bulletsQueue.Count > 0)
        {
            fireTimer += Time.deltaTime;
            if (fireTimer >= fireInterval)
            {
                GameObject bullet = bulletsQueue.Dequeue();
                Bullet bulletScript = bullet.GetComponent<Bullet>();
                if (bulletScript != null)
                {
                    bulletScript.Launch();
                }
                fireTimer = 0f;
            }
        }
    }

    void SpawnBullet()
    {
        if (bulletsQueue.Count >= maxBullets)
        {
            GameObject oldBullet = bulletsQueue.Dequeue();
            Destroy(oldBullet);
        }

        GameObject bullet = Instantiate(bulletPrefab, muzzleTransform.position, muzzleTransform.rotation);
        bulletsQueue.Enqueue(bullet);
    }
}