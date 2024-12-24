using UnityEngine;

public class Checkpoint : MonoBehaviour
{
    [SerializeField] private int index;
    [SerializeField] private bool isPassed = false;
    [SerializeField] private CheckpointManager manager;
    public int Index 
    {
        get
        {
            return index;
        }
        set
        {
            index = value;
        }
    } 
    public bool IsPassed 
    {
        get
        {
            return isPassed;
        }
        set
        {
            isPassed = value;
        }
    }
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
        if (other.tag == "Player" && IsPassed == false)
        {
            manager.CheckCheckpointOrder(this);
        }
    }
}