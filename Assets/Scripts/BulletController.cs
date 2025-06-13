using UnityEngine;

public class Bullet : MonoBehaviour
{
    public float speed = 100f;
    public float lifeTime = 2f;
    public GameObject explosionEffectPrefab;

    private float lifeTimer = 0f;
    private float fullSpeedTimeElapsed = 0f;
    private float fullSpeedDuration = 0.05f; // Bullet travels at full speed for a short time at first

    void Update()
    {
        fullSpeedTimeElapsed += Time.deltaTime;

        // Choose speed factor: normal (1.0) for first 0.5s, then use gameplaySpeed
        float speedFactor = fullSpeedTimeElapsed < fullSpeedDuration ? 1f : GameManager.gameplaySpeed;

        float delta = Time.deltaTime * speedFactor;
        transform.position += transform.forward * speed * delta;

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
