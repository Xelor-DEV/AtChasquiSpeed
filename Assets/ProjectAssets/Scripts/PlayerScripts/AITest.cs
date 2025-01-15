using UnityEngine;

public class AITest : RaceTracker
{
    [SerializeField] private float speed = 10f; // Velocidad del bot

    private void Update()
    {
        MoveTowardsNextCheckpoint();
    }

    private void MoveTowardsNextCheckpoint()
    {
        Checkpoint nextCheckpoint = GetNextCheckpoint();
        Vector3 direction = (nextCheckpoint.transform.position - transform.parent.position).normalized;
        transform.parent.position += direction * speed * Time.deltaTime;
    }
}
