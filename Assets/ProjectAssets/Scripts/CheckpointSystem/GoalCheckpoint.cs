using UnityEngine;

public class GoalCheckpoint : Checkpoint
{
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

                    manager.CheckGoal(raceTracker.gameObject);
                }
                else
                {
                    Debug.Log("RaceTracker no encontrado entre los hijos del padre.");
                }
            }
            else
            {
                Debug.Log("El objeto colisionado no tiene un padre en la jerarqu�a.");
            }
        }
    }
}