using UnityEngine;

public class HologramBeam : MonoBehaviour
{
    public Transform spider;         // The spider
    public Transform robot;     // The robot

    void Update()
    {
        if (!spider || !robot) return;

        Vector3 start = robot.position;
        Vector3 end = spider.position;
        Vector3 direction = end - start;
        float distance = direction.magnitude;

        // Position on the drone
        transform.position = start;

        // Look from drone to spider
        transform.rotation = Quaternion.LookRotation(-direction);

        // Adjust length of beam via its scale
        Vector3 localScale = transform.localScale;
        localScale.z = distance * 100;
        transform.localScale = localScale;
    }
}

