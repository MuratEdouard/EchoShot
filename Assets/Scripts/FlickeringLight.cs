using UnityEngine;

[RequireComponent(typeof(Light))]
public class FlickeringLight : MonoBehaviour
{
    public float minIntensity = 0.8f;
    public float maxIntensity = 1.2f;
    public float flickerSpeed = 0.05f;  // time between flickers

    private Light _light;
    private float _timer;

    void Start()
    {
        _light = GetComponent<Light>();
        _timer = flickerSpeed;
    }

    void Update()
    {
        _timer -= Time.deltaTime;
        if (_timer <= 0f)
        {
            _light.intensity = Random.Range(minIntensity, maxIntensity);
            _timer = flickerSpeed + Random.Range(0f, 0.1f); // adds slight irregularity
        }
    }
}
