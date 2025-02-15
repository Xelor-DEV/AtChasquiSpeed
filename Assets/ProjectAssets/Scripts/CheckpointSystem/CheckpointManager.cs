using UnityEngine;
using NaughtyAttributes;
public class CheckpointManager : MonoBehaviour
{
    [BoxGroup("Checkpoints Array")]
    [Tooltip("All the main checkpoints are stored here.")]
    [SerializeField] private Checkpoint[] mainCheckpoints;

    [BoxGroup("Checkpoints Array")]
    [Tooltip("Auxiliary checkpoints are stored here.")]
    [SerializeField] private Checkpoint[] auxiliaryCheckpoints;

    [BoxGroup("Checkpoints Array")]
    [Tooltip("The goal checkpoint is stored here.")]
    [SerializeField] private Checkpoint goalCheckpoint;

    [BoxGroup("All Checkpoints In Order")]
    [Tooltip("All checkpoints are stored here in the order in which they appear on the track.")]
    [SerializeField] private Checkpoint[] allCheckpoints;

    public Checkpoint[] MainCheckpoints
    {
        get 
        { 
            return mainCheckpoints; 
        }
    }

    public Checkpoint[] AuxiliaryCheckpoints
    {
        get 
        { 
            return auxiliaryCheckpoints; 
        }
    }

    public Checkpoint GoalCheckpoint
    {
        get 
        { 
            return goalCheckpoint; 
        }
    }

    public Checkpoint[] AllCheckpoints
    {
        get 
        { 
            return allCheckpoints; 
        }
    }

    public int LastMainIndex
    {
        get 
        { 
            return mainCheckpoints.Length - 1; 
        }
    }
    private void Awake()
    {
        AssignCheckpointIndexes();
    }

    private void AssignCheckpointIndexes()
    {
        // Asigna el índice a cada checkpoint
        for (int i = 0; i < mainCheckpoints.Length; ++i)
        {
            mainCheckpoints[i].SetIndex(i);
        }

        for (int i = 0; i < auxiliaryCheckpoints.Length; ++i)
        {
            auxiliaryCheckpoints[i].SetIndex(i);
        }
    }
}
