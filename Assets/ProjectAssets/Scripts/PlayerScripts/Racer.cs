using UnityEngine;
using NaughtyAttributes;

public class Racer : MonoBehaviour
{
    [BoxGroup("Real Position")]
    [SerializeField] private GameObject parentObject;

    [BoxGroup("Checkpoint Index")]
    [SerializeField] private int currentMainIndex = -1;
    [BoxGroup("Checkpoint Index")]
    [SerializeField] private int currentAuxiliaryIndex = -1;
    [BoxGroup("Checkpoint Index")]
    [SerializeField] private int currentCheckpointIndex = -1;

    [BoxGroup("Race Stats")]
    [SerializeField] private int lapsCompleted = 0;
    [BoxGroup("Race Stats")]
    [SerializeField] private int raceRank;

    [BoxGroup("Checkpoint Manager")]
    [SerializeField] protected CheckpointManager checkpointManager;

    public int CurrentMainIndex
    {
        get
        {
            return currentMainIndex;
        }
    }
    public int CurrentAuxiliaryIndex
    {
        get
        {
            return currentAuxiliaryIndex;
        }
    }
    public int CurrentCheckpointIndex
    {
        get
        {
            return currentCheckpointIndex;
        }
    }
    public int LapsCompleted
    {
        get
        {
            return lapsCompleted;
        }
    }
    public int RaceRank
    {
        get
        {
            return raceRank;
        }
        set
        {
            raceRank = value;
        }
    }
    public GameObject ParentObject
    {
        get
        {
            return parentObject;
        }
        set
        {
            parentObject = value;
        }
    }

    public CheckpointManager CheckpointManager
    {
        get
        {
            return checkpointManager;
        }
        set
        {
            checkpointManager = value;
        }
    }
    public Checkpoint NextCheckpoint
    {
        get
        {
            return GetNextCheckpoint();
        }
    }

    public void ProcessCheckpoint(Checkpoint checkpoint)
    {
        UpdateGlobalProgress(checkpoint);

        switch (checkpoint.Type)
        {
            case Checkpoint.CheckpointType.Main:
                UpdateMainProgress(checkpoint);
                break;

            case Checkpoint.CheckpointType.Auxiliary:
                UpdateAuxiliaryProgress(checkpoint);
                break;

            case Checkpoint.CheckpointType.Goal:
                ValidateLapCompletion();
                break;
        }
    }

    private void UpdateGlobalProgress(Checkpoint checkpoint)
    {
        // Sistema mejorado de detección de progreso
        for (int i = 0; i < checkpointManager.AllCheckpoints.Length; ++i)
        {
            if (checkpointManager.AllCheckpoints[i] == checkpoint)
            {
                if (i == currentCheckpointIndex + 1 ||
                   (currentCheckpointIndex == checkpointManager.AllCheckpoints.Length - 1 && i == 0))
                {
                    currentCheckpointIndex = i;
                }
                break;
            }
        }
    }

    private void UpdateMainProgress(Checkpoint checkpoint)
    {
        if (IsValidCheckpoint(checkpoint.Index, currentMainIndex))
        {
            currentMainIndex = checkpoint.Index;
        }
    }

    private void UpdateAuxiliaryProgress(Checkpoint checkpoint)
    {
        if (IsValidCheckpoint(checkpoint.Index, currentAuxiliaryIndex))
        {
            currentAuxiliaryIndex = checkpoint.Index;
        }
    }

    private bool IsValidCheckpoint(int checkpointIndex, int currentIndex)
    {
        return checkpointIndex == currentIndex + 1 ||
               checkpointIndex == currentIndex - 1;
    }

    private void ValidateLapCompletion()
    {
        if (currentMainIndex == checkpointManager.LastMainIndex)
        {
            lapsCompleted++;
            currentMainIndex = -1;
            currentAuxiliaryIndex = -1;
        }
    }
    
    protected virtual Checkpoint GetNextCheckpoint()
    {
        if (checkpointManager.AllCheckpoints.Length == 0) return null;

        int nextIndex = currentCheckpointIndex + 1;

        // Lógica mejorada de transición entre vueltas
        if (nextIndex >= checkpointManager.AllCheckpoints.Length)
        {
            nextIndex = 1; // Saltar al primer checkpoint después del Goal
        }
        else if (currentCheckpointIndex == -1)
        {
            nextIndex = 0; // Comportamiento inicial
        }

        return checkpointManager.AllCheckpoints[nextIndex];
    }
}
