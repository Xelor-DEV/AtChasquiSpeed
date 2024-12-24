using UnityEngine;

public class GoalCheckpoint : MonoBehaviour
{
    [SerializeField] private CheckpointManager manager;
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
    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            manager.CheckGoal();
        }
    }
}