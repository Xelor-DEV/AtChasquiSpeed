using UnityEngine;

public class CheckpointManager : MonoBehaviour
{
    public RaceCheckpoint[] mainCheckpoints; // Asignar desde el inspector
    public RaceCheckpoint[] auxiliaryCheckpoints; // Asignar desde el inspector
    public GoalCheckpoint goalCheckpoint; // La meta, asignada desde el inspector

    private void Awake()
    {
        // Asigna el índice a cada checkpoint
        for (int i = 0; i < mainCheckpoints.Length; ++i)
        {
            mainCheckpoints[i].Index = i;
            mainCheckpoints[i].Manager = this;
        }
        for (int i = 0; i < auxiliaryCheckpoints.Length; ++i)
        {
            auxiliaryCheckpoints[i].Index = i;
            auxiliaryCheckpoints[i].Manager = this;
        }
        goalCheckpoint.Manager = this;
    }

    public void CheckCheckpointOrder(Checkpoint checkpoint, GameObject vehicle)
    {
        RaceTracker raceTracker = vehicle.GetComponent<RaceTracker>();
        if (raceTracker != null)
        {
            raceTracker.UpdateCheckpointIndex(checkpoint);
        }
        else
        {
            Debug.Log("No se encontró el componente RaceTracker en el vehículo.");
        }
    }

    public void CheckGoal(GameObject vehicle)
    {
        RaceTracker raceTracker = vehicle.GetComponent<RaceTracker>();
        if (raceTracker != null)
        {
            raceTracker.CompleteLap();
        }
        else
        {
            Debug.Log("No se encontró el componente RaceTracker en el vehículo.");
        }
    }
}
