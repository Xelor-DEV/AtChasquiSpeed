using UnityEngine;

public class Checkpoint : MonoBehaviour
{
    [SerializeField] protected CheckpointManager manager;
    public CheckpointManager Manager
    {
        get
        {
            return manager;
        }
        set
        {
            manager = value;
        }
    }
}
