using UnityEngine;

public class Bullet : MonoBehaviour
{
    public float speed = 50f;
    public GameObject explosionEffectPrefab;

    private bool launched = false;

    public void Launch()
    {
        launched = true;
    }

    void Update()
    {
        if (launched)
        {
            transform.position += transform.forward * speed * Time.deltaTime;
        }
    }

    void OnCollisionEnter(Collision collision)
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
