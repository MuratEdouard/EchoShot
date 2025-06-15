using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class ArrowController : MonoBehaviour
{
    public float speed = 100f;
    public float lifeTime = 2f;
    public GameObject explosionEffectPrefab;

    private float lifeTimer = 0f;
    private float fullSpeedTimeElapsed = 0f;
    private float fullSpeedDuration = 0.05f;

    private Rigidbody rb;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        rb.isKinematic = true; // Necessary for MovePosition to behave predictably
        rb.collisionDetectionMode = CollisionDetectionMode.Continuous; // Avoid tunneling
    }

    void Update()
    {
        fullSpeedTimeElapsed += Time.deltaTime;

        float speedFactor = fullSpeedTimeElapsed < fullSpeedDuration ? 1f : GameManager.gameplaySpeed;
        float delta = Time.deltaTime * speedFactor;

        Vector3 nextPosition = transform.position + transform.forward * speed * delta;
        rb.MovePosition(nextPosition); // Physics-friendly movement

        lifeTimer += delta;
        if (lifeTimer >= lifeTime)
        {
            Destroy(gameObject);
        }
    }

    void OnTriggerEnter(Collider other)
    {
        Explode();
    }

    void Explode()
    {
        if (explosionEffectPrefab != null)
        {
            Instantiate(explosionEffectPrefab, transform.position, Quaternion.identity);
        }
        Destroy(gameObject);
    }
}
