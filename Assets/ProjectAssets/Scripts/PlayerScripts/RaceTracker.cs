using UnityEngine;
using NaughtyAttributes;
using System;

public class RaceTracker : MonoBehaviour
{
    [BoxGroup("Checkpoint Index")]
    [SerializeField] private int currentMain = -1;
    [BoxGroup("Checkpoint Index")]
    [SerializeField] private int currentAuxiliary = -1;

    [BoxGroup("Laps")]
    [SerializeField] private int lapsCompleted = 0;

    [BoxGroup("Checkpoint Manager")]
    [SerializeField] private CheckpointManager checkpointManager;

    public void UpdateCheckpointIndex(Checkpoint checkpoint)
    {
        if (checkpoint.Type == Checkpoint.CheckpointType.Main)
        {
            // Verificar que el índice del checkpoint es el siguiente al actual
            if (currentMain + 1 == checkpoint.Index)
            {
                currentMain = checkpoint.Index;
            }
        }
        else if (checkpoint.Type == Checkpoint.CheckpointType.Auxiliary)
        {
            // Verificar que el índice del checkpoint auxiliar es el siguiente al actual
            if (currentAuxiliary + 1 == checkpoint.Index)
            {
                currentAuxiliary = checkpoint.Index;
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
    }

    public float GetDistanceToNextCheckpoint()
    {
        Checkpoint nextCheckpoint = GetNextCheckpoint();
        return Vector3.Distance(transform.position, nextCheckpoint.transform.position);
    }

    private Checkpoint GetNextCheckpoint()
    {
        // Lógica para obtener el siguiente checkpoint (main o auxiliary)
        Checkpoint nextMainCheckpoint = checkpointManager.mainCheckpoints[(currentMain + 1) % checkpointManager.mainCheckpoints.Length];
        Checkpoint nextAuxiliaryCheckpoint = checkpointManager.auxiliaryCheckpoints[(currentAuxiliary + 1) % checkpointManager.auxiliaryCheckpoints.Length];

        float distanceToMain = Vector3.Distance(transform.position, nextMainCheckpoint.transform.position);
        float distanceToAuxiliary = Vector3.Distance(transform.position, nextAuxiliaryCheckpoint.transform.position);

        return distanceToMain < distanceToAuxiliary ? nextMainCheckpoint : nextAuxiliaryCheckpoint;
    }

    public static void CalculateRacePositions(RaceTracker[] raceTrackers)
    {
        Array.Sort(raceTrackers, (a, b) =>
        {
            // Comparar vueltas completadas
            if (a.lapsCompleted != b.lapsCompleted)
            {
                return b.lapsCompleted.CompareTo(a.lapsCompleted);
            }

            // Comparar el checkpoint principal más avanzado
            if (a.currentMain != b.currentMain)
            {
                return b.currentMain.CompareTo(a.currentMain);
            }

            // Comparar el checkpoint auxiliar más avanzado
            if (a.currentAuxiliary != b.currentAuxiliary)
            {
                return b.currentAuxiliary.CompareTo(a.currentAuxiliary);
            }

            // Si ambos están en el último checkpoint auxiliar, comparar la distancia a la meta
            if (a.currentAuxiliary == a.checkpointManager.auxiliaryCheckpoints.Length - 1 &&
                b.currentAuxiliary == b.checkpointManager.auxiliaryCheckpoints.Length - 1)
            {
                // Distancia a la meta (goal)
                float distanceToGoalA = Vector3.Distance(a.transform.position, a.checkpointManager.goalCheckpoint.transform.position);
                float distanceToGoalB = Vector3.Distance(b.transform.position, b.checkpointManager.goalCheckpoint.transform.position);
                return distanceToGoalA.CompareTo(distanceToGoalB);
            }

            // Si todo lo anterior es igual, usar la distancia al siguiente checkpoint
            return a.GetDistanceToNextCheckpoint().CompareTo(b.GetDistanceToNextCheckpoint());
        });
    }
}
