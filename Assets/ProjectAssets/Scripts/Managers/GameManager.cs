using UnityEngine;

public class GameManager : MonoBehaviour
{
    public int lapsCompleted = 0;
    public CheckpointManager checkpointManager;

    private void OnEnable()
    {
        checkpointManager.OnLapCompleted += IncrementLap;
    }

    private void OnDisable()
    {
        checkpointManager.OnLapCompleted -= IncrementLap;
    }

    private void IncrementLap()
    {
        lapsCompleted++;
        Debug.Log($"Vueltas completadas: {lapsCompleted}");
    }
}
