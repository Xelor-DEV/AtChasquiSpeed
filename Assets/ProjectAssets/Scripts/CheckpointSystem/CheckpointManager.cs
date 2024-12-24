using System;
using UnityEngine;

public class CheckpointManager : MonoBehaviour
{
    public Checkpoint[] checkpoints; // Asignar desde el inspector
    public GoalCheckpoint goalCheckpoint; // La meta, asignada desde el inspector
    public Action OnLapCompleted; // Evento para notificar que se completó una vuelta

    private void Awake()
    {
        // Asigna el índice a cada checkpoint
        for (int i = 0; i < checkpoints.Length; ++i)
        {
            checkpoints[i].Index = i;
            checkpoints[i].Manager = this;
        }
        goalCheckpoint.Manager = this;
    }

    public void CheckCheckpointOrder(Checkpoint checkpoint)
    {
        int index = checkpoint.Index;

        // Verifica si el checkpoint actual y el anterior están pasados
        if (index == 0 || checkpoints[index - 1].IsPassed)
        {
            checkpoint.IsPassed = true;
            Debug.Log($"Checkpoint {index} registrado correctamente. Estado: {checkpoint.IsPassed}");
        }
        else
        {
            Debug.Log($"Checkpoint {index} no puede ser registrado porque el anterior no se ha pasado.");
        }
    }

    public void CheckGoal()
    {
        for (int i = 0; i < checkpoints.Length; ++i)
        {
            if (checkpoints[i].IsPassed == false)
            {
                Debug.Log("Aún no has pasado todos los checkpoints.");
                return;
            }
        }
        Debug.Log("¡Vuelta completada!");
        OnLapCompleted?.Invoke(); // Llamar al evento
        ResetCheckpoints(); // Reiniciar los checkpoints para la siguiente vuelta
    }

    private void ResetCheckpoints()
    {
        for (int i = 0; i < checkpoints.Length; ++i)
        {
            checkpoints[i].IsPassed = false;
        }
    }
}
