using UnityEngine;
using NaughtyAttributes;
public class Checkpoint : MonoBehaviour
{
    public enum CheckpointType
    { 
        Main, 
        Auxiliary, 
        Goal 
    }
    [BoxGroup("Checkpoint Type")]
    [SerializeField] private CheckpointType type;
    [BoxGroup("Checkpoint Index")]
    [SerializeField] private int index;
    public CheckpointType Type
    {
        get
        {
            return type;
        }
    }
    public int Index
    {
        get
        {
            return index;
        }
    }
    public void SetIndex(int newIndex)
    {
        index = newIndex;
    }
    private void OnTriggerEnter(Collider other)
    {
        Transform parent = other.transform.parent;

        if (parent != null)
        {
            Transform childTransform = parent.GetChild(0);
            Racer racer = childTransform.GetComponent<Racer>();

            if (racer != null)
            {
                racer.ProcessCheckpoint(this);
            }
            else
            {
                Debug.Log("Racer no encontrado entre los hijos del padre.");
            }
        }
        else
        {
            Debug.Log("El objeto colisionado no tiene un padre en la jerarquía.");
        }
    }
}