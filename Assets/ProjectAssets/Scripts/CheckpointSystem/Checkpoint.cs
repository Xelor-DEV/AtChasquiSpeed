using UnityEngine;

public class Checkpoint : MonoBehaviour
{
    public enum CheckpointType
    {
        Main,
        Auxiliary
    }

    [SerializeField] private int index;
    [SerializeField] private CheckpointManager manager;
    [SerializeField] private CheckpointType type;
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
    public CheckpointType Type
    {
        get 
        { 
            return type; 
        }
        set 
        { 
            type = value; 
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player" || other.tag == "AI")
        {
            manager.CheckCheckpointOrder(this, other.gameObject);
        }
    }
}