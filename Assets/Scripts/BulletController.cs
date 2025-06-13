using UnityEngine;

public class Bullet : MonoBehaviour
{
    public float speed = 50f;
    public float lifeTime = 5f;
    public GameObject explosionEffectPrefab;

    private float lifeTimer = 0f;

    void Update()
    {
        // Move bullet respecting gameplaySpeed multiplier
        float delta = Time.unscaledDeltaTime * GameManager.gameplaySpeed;
        transform.position += transform.forward * speed * delta;

        lifeTimer += delta;
        if (lifeTimer >= lifeTime)
        {
            Explode();
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
