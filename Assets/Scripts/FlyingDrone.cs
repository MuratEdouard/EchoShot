using UnityEngine;
using UnityEngine.AI;

public class FlyingDrone : MonoBehaviour
{
    public NavMeshAgent agent;
    public float normalSpeed = 1f;
    public float patrolRadius = 30f;     // Radius to pick random patrol points
    public float detectionRadius = 15f;  // Distance to start chasing player
    public float waypointTolerance = 1f; // How close to get to patrol point

    [Header("Explosion Settings")]
    public GameObject explosionEffectPrefab;

    [Header("Laser Settings")]
    public GameObject laserPrefab;
    public Transform laserSpawnPoint;
    public float laserCooldown = 2f;
    public float firingAngleTolerance = 45f; // In degrees

    private float laserTimer = 0f;


    // Stuck detection variables
    private Vector3 lastPosition;
    private float stuckTimer = 0f;
    public float stuckThreshold = 0.1f;  // Minimum movement considered "moving"
    public float stuckDuration = 3f;     // Seconds stuck before forcing new point

    private Vector3 startPosition;
    private Vector3 currentPatrolPoint;
    private bool chasingPlayer = false;
    private Transform target; // Player transform


    void Start()
    {
        // Automatically find the player by tag
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null)
        {
            target = playerObj.transform;
        }
        else
        {
            Debug.LogWarning("FlyingDrone: No GameObject with tag 'Player' found in scene.");
        }

        normalSpeed = agent.speed;
        startPosition = transform.position;
        SetRandomPatrolPoint();
        lastPosition = transform.position;

        laserTimer = Random.Range(0f, laserCooldown);

    }

    void Update()
    {
        float speedFactor = Mathf.Lerp(1f, 0.5f, 1f - GameManager.gameplaySpeed);
        agent.speed = normalSpeed * speedFactor;

        if (target == null) return;

        float distanceToPlayer = Vector3.Distance(transform.position, target.position);

        if (distanceToPlayer <= detectionRadius)
        {
            chasingPlayer = true;
            agent.SetDestination(target.position);
        }
        else
        {
            if (chasingPlayer)
            {
                chasingPlayer = false;
                SetRandomPatrolPoint();
            }

            if (!agent.pathPending && agent.remainingDistance < waypointTolerance)
            {
                SetRandomPatrolPoint();
            }
        }

        // Fire if chasing player and facing them
        if (chasingPlayer && CanSeePlayer())
        {
            laserTimer += Time.deltaTime;
            if (laserTimer >= laserCooldown)
            {
                FireLaser();
                laserTimer = 0f;
            }
        }


        DetectIfStuck();
    }

    void FireLaser()
    {
        if (laserPrefab == null || laserSpawnPoint == null) return;

        Vector3 toPlayer = ((target.position + (Vector3.up * 0.5f)) - laserSpawnPoint.position).normalized;
        Quaternion rotationToPlayer = Quaternion.LookRotation(toPlayer);
        Instantiate(laserPrefab, laserSpawnPoint.position, rotationToPlayer);
    }

    bool CanSeePlayer()
    {
        if (target == null) return false;

        Vector3 toPlayer = (target.position - transform.position);
        float angle = Vector3.Angle(transform.forward, toPlayer.normalized);

        if (angle <= firingAngleTolerance)
        {
            return true;
        }

        return false;
    }



    void SetRandomPatrolPoint()
    {
        Vector3 randomDirection = Random.insideUnitSphere * patrolRadius;
        randomDirection += startPosition;

        if (NavMesh.SamplePosition(randomDirection, out NavMeshHit hit, patrolRadius, NavMesh.AllAreas))
        {
            currentPatrolPoint = hit.position;
            agent.SetDestination(currentPatrolPoint);
        }
        else
        {
            // Could not find a valid point, will try again next Update
        }
    }

    void DetectIfStuck()
    {
        float distanceMoved = Vector3.Distance(transform.position, lastPosition);

        if (agent.hasPath && agent.velocity.magnitude > 0.1f)
        {
            stuckTimer = 0f; // Agent moving, reset timer
        }
        else if (agent.hasPath && distanceMoved < stuckThreshold)
        {
            stuckTimer += Time.deltaTime;

            if (stuckTimer >= stuckDuration)
            {
                Debug.Log("Agent stuck, picking new patrol point.");
                stuckTimer = 0f;
                SetRandomPatrolPoint();
            }
        }
        else
        {
            stuckTimer = 0f; // Reset if no path or moving
        }

        lastPosition = transform.position;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Arrow"))
        {
            ExplodeAndDie();
        }
    }

    void ExplodeAndDie()
    {
        if (explosionEffectPrefab != null)
        {
            Instantiate(explosionEffectPrefab, transform.position, Quaternion.identity);
        }

        Destroy(gameObject);
    }
}
