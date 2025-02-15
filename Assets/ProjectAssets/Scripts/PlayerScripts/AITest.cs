using UnityEngine;
using System.Collections;
public class AITest : Racer
{
    [Header("Movement Settings")]
    [SerializeField] private float maxSpeed = 20f;
    [SerializeField] private float acceleration = 8f;
    [SerializeField] private float deceleration = 10f;
    [SerializeField] private float rotationSpeed = 3f;
    [SerializeField] private float waypointThreshold = 2f;

    [Header("Advanced Settings")]
    [SerializeField] private float speedBoostMultiplier = 1.5f;
    [SerializeField] private float minSpeed = 5f;
    [SerializeField] private float obstacleDetectionDistance = 5f;
    [SerializeField] private LayerMask obstacleLayer;

    [SerializeField] private Rigidbody rb;
    private Vector3 targetDirection;
    private float currentSpeed;
    private bool isBoosting;

    void Start()
    {
        InitializeComponents();
        ConfigurePhysics();
    }

    private void InitializeComponents()
    {
        currentSpeed = minSpeed;
    }

    private void ConfigurePhysics()
    {
        rb.interpolation = RigidbodyInterpolation.Interpolate;
        rb.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;
    }

    private void FixedUpdate()
    {
        if (NextCheckpoint != null)
        {
            UpdateNavigation();
            ApplyMovement();
            HandleObstacleAvoidance();
        }
    }

    private void UpdateNavigation()
    {
        targetDirection = (NextCheckpoint.transform.position - transform.position).normalized;
        AdjustSpeedBasedOnDistance();
        ApplyRotation();
    }

    private void AdjustSpeedBasedOnDistance()
    {
        float distanceToTarget = Vector3.Distance(transform.position, NextCheckpoint.transform.position);

        if (distanceToTarget > waypointThreshold)
        {
            currentSpeed = Mathf.Lerp(currentSpeed, maxSpeed, acceleration * Time.fixedDeltaTime);
        }
        else
        {
            currentSpeed = Mathf.Lerp(currentSpeed, minSpeed, deceleration * Time.fixedDeltaTime);
        }
    }

    private void ApplyRotation()
    {
        if (targetDirection != Vector3.zero)
        {
            Quaternion targetRotation = Quaternion.LookRotation(targetDirection);
            transform.rotation = Quaternion.Slerp(
                transform.rotation,
                targetRotation,
                rotationSpeed * Time.fixedDeltaTime
            );
        }
    }

    private void ApplyMovement()
    {
        float finalSpeed = isBoosting ? currentSpeed * speedBoostMultiplier : currentSpeed;
        rb.velocity = Vector3.Lerp(
            rb.velocity,
            transform.forward * finalSpeed,
            acceleration * Time.fixedDeltaTime
        );
    }

    private void HandleObstacleAvoidance()
    {
        if (Physics.Raycast(transform.position, transform.forward, out var hit, obstacleDetectionDistance, obstacleLayer))
        {
            Vector3 avoidanceDirection = CalculateAvoidanceDirection(hit);
            ApplyAvoidanceRotation(avoidanceDirection);
            ReduceSpeedForObstacle();
        }
    }

    private Vector3 CalculateAvoidanceDirection(RaycastHit hit)
    {
        Vector3 hitNormal = hit.normal;
        Vector3 avoidanceDirection = Vector3.Cross(hitNormal, Vector3.up).normalized;
        return Vector3.Dot(avoidanceDirection, transform.forward) > 0 ? avoidanceDirection : -avoidanceDirection;
    }

    private void ApplyAvoidanceRotation(Vector3 direction)
    {
        Quaternion targetRotation = Quaternion.LookRotation(direction);
        transform.rotation = Quaternion.Slerp(
            transform.rotation,
            targetRotation,
            rotationSpeed * 2 * Time.fixedDeltaTime
        );
    }

    private void ReduceSpeedForObstacle()
    {
        currentSpeed = Mathf.Lerp(currentSpeed, minSpeed, deceleration * 2 * Time.fixedDeltaTime);
    }

    public void ActivateBoost(float duration)
    {
        if (!isBoosting)
        {
            StartCoroutine(BoostCoroutine(duration));
        }
    }

    private IEnumerator BoostCoroutine(float duration)
    {
        isBoosting = true;
        yield return new WaitForSeconds(duration);
        isBoosting = false;
    }

    protected override Checkpoint GetNextCheckpoint()
    {
        // Lógica especial para evitar loop en último checkpoint
        if (CurrentCheckpointIndex == checkpointManager.AllCheckpoints.Length - 1)
        {
            return checkpointManager.AllCheckpoints[1];
        }

        return base.GetNextCheckpoint();
    }
}
