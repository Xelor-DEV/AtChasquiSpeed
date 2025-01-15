using UnityEngine;
using NaughtyAttributes;

public class RaceTracker : MonoBehaviour
{
    [BoxGroup("Real Position")]
    [SerializeField] private GameObject parentObject;

    [BoxGroup("Checkpoint Index")]
    [SerializeField] private int currentMain = -1;
    [BoxGroup("Checkpoint Index")]
    [SerializeField] private int currentAuxiliary = -1;

    [BoxGroup("Race Stats")]
    [SerializeField] private int lapsCompleted = 0;
    [BoxGroup("Race Stats")]
    [SerializeField] private int raceRanking;

    [BoxGroup("Checkpoint Manager")]
    [SerializeField] private CheckpointManager checkpointManager;

    public int CurrentMain
    {
        get
        {
            return currentMain;
        }
    }

    public int CurrentAuxiliary
    {
        get
        {
            return currentAuxiliary;
        }
    }

    public int LapsCompleted
    {
        get
        {
            return lapsCompleted;
        }
    }

    public int RaceRanking
    {
        get
        {
            return raceRanking;
        }
        set
        {
            raceRanking = value;
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

    public void UpdateCheckpointIndex(Checkpoint checkpoint)
    {
        RaceCheckpoint raceCheckpoint = checkpoint.gameObject.GetComponent<RaceCheckpoint>();

        if (raceCheckpoint.Type == RaceCheckpoint.CheckpointType.Main)
        {
            // Verificar que el índice del checkpoint es el siguiente al actual
            if (currentMain + 1 == raceCheckpoint.Index)
            {
                currentMain = raceCheckpoint.Index;
            }
            else
            {
                Debug.Log("Error: El checkpoint principal no es el siguiente al actual");
            }
        }
        else if (raceCheckpoint.Type == RaceCheckpoint.CheckpointType.Auxiliary)
        {
            // Verificar que el índice del checkpoint auxiliar es el siguiente al actual
            if (currentAuxiliary + 1 == raceCheckpoint.Index)
            {
                currentAuxiliary = raceCheckpoint.Index;
            }
            else
            {
                Debug.Log("Error: El checkpoint auxiliar no es el siguiente al actual");
            }
        }
    }

    public void CompleteLap()
    {
        // Validar si se han completado todos los checkpoints principales
        if (currentMain == checkpointManager.mainCheckpoints.Length - 1)
        {
            // Si los checkpoints principales se han completado correctamente, incrementar las vueltas
            lapsCompleted = lapsCompleted + 1;
            Debug.Log($"Vueltas completadas: {lapsCompleted}");
            
            // Resetear los índices de los checkpoints para la siguiente vuelta
            currentMain = -1;  // Reiniciar a -1
            currentAuxiliary = -1;  // Reiniciar a -1
        }
        else
        {
            Debug.Log("Error: No se han completado todos los checkpoints principales");
        }
    }

    public float GetDistanceToNextCheckpoint()
    {
        Checkpoint nextCheckpoint = GetNextCheckpoint();
        return Vector3.Distance(transform.parent.position, nextCheckpoint.transform.position);
    }

    public Checkpoint GetNextCheckpoint()
    {
        // Lógica para obtener el siguiente checkpoint (main o auxiliary)
        Checkpoint nextMainCheckpoint = checkpointManager.mainCheckpoints[(currentMain + 1) % checkpointManager.mainCheckpoints.Length];
        Checkpoint nextAuxiliaryCheckpoint = checkpointManager.auxiliaryCheckpoints[(currentAuxiliary + 1) % checkpointManager.auxiliaryCheckpoints.Length];

        float distanceToMain = Vector3.Distance(parentObject.transform.position, nextMainCheckpoint.transform.position);
        float distanceToAuxiliary = Vector3.Distance(parentObject.transform.position, nextAuxiliaryCheckpoint.transform.position);
        float distanceToGoal = Vector3.Distance(parentObject.transform.position, checkpointManager.goalCheckpoint.transform.position);

        // Determinar cuál es el checkpoint más cercano
        if (distanceToMain <= distanceToAuxiliary && distanceToMain <= distanceToGoal)
        {
            // El checkpoint main es el más cercano
            return nextMainCheckpoint;
        }
        else if (distanceToAuxiliary <= distanceToMain && distanceToAuxiliary <= distanceToGoal)
        {
            // El checkpoint auxiliary es el más cercano
            return nextAuxiliaryCheckpoint;
        }
        else
        {
            // El goal checkpoint es el más cercano
            return checkpointManager.goalCheckpoint;
        }
    }
}
