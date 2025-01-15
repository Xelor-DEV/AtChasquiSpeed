using UnityEngine;

public class RaceCheckpoint : Checkpoint
{
    public enum CheckpointType
    {
        Main,
        Auxiliary
    }

    [SerializeField] private int index;
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
            // Subir al objeto padre desde el objeto colisionado
            Transform parent = other.transform.parent;

            if (parent != null)
            {
                Transform childTransform = parent.GetChild(0);
                RaceTracker raceTracker = childTransform.GetComponent<RaceTracker>();

                if (raceTracker != null)
                {
                    // Pasar el objeto con RaceTracker al CheckpointManager
                    manager.CheckCheckpointOrder(this, raceTracker.gameObject);
                }
                else
                {
                    Debug.Log("RaceTracker no encontrado entre los hijos del padre.");
                }
            }
            else
            {
                Debug.Log("El objeto colisionado no tiene un padre en la jerarquía.");
            }
        }
    }
}