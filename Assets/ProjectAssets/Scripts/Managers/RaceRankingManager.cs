using System.Collections;
using UnityEngine;

public class RaceRankingManager : MonoBehaviour
{
    [SerializeField] private GameObject[] allRacers; // All Racers
    [SerializeField] private GameObject[] players; // Players
    [SerializeField] private AIManager aiManager; // AI Manager
    [SerializeField] private float sortInterval = 0.5f; // Intervalo en segundos para llamar a SortRacers
    [SerializeField] private bool sortRacers = false; // Controla si se quiere ordenar a los corredores
    [SerializeField] private bool sortAtStart = true; // Controla si el sorting se activa automáticamente al inicializar

    private RaceTracker[] raceTrackers; // Arreglo privado de RaceTrackers

    public GameObject[] AllRacers
    {
        get
        {
            return allRacers;
        }
        set
        {
            allRacers = value;
        }
    }

    private void OnEnable()
    {
        aiManager.OnAICarsInitialized += InitializeAllRacers;
    }

    private void OnDisable()
    {
        aiManager.OnAICarsInitialized -= InitializeAllRacers;
        if (sortRacers == true)
        {
            StopCoroutine(SortRacers());
        }
    }

    private void InitializeAllRacers()
    {
        int totalRacers = players.Length + aiManager.AiCars.Length;
        allRacers = new GameObject[totalRacers];

        for (int i = 0; i < totalRacers; ++i)
        {
            if (i < players.Length)
            {
                allRacers[i] = players[i];
            }
            else
            {
                allRacers[i] = aiManager.AiCars[i - players.Length];
            }
        }

        // Inicializamos el arreglo de RaceTrackers una sola vez
        raceTrackers = new RaceTracker[allRacers.Length];
        for (int i = 0; i < allRacers.Length; ++i)
        {
            // Usamos GetChild(0) para obtener el primer hijo y luego buscamos el RaceTracker
            Transform childTransform = allRacers[i].transform.GetChild(0);
            raceTrackers[i] = childTransform.GetComponent<RaceTracker>();

            if (raceTrackers[i] == null)
            {
                Debug.Log($"RaceTracker no encontrado en el primer hijo de {allRacers[i].name}");
            }
        }

        if (sortAtStart == true)
        {
            StartCoroutine(SortRacers());
        }
    }

    private IEnumerator SortRacers()
    {
        // Usamos el arreglo privado de RaceTrackers para evitar crear uno nuevo
        QuickSort(raceTrackers, 0, raceTrackers.Length - 1);

        // Actualizamos la posición de los corredores
        for (int i = 0; i < raceTrackers.Length; ++i)
        {
            raceTrackers[i].RaceRanking = i + 1;
        }

        yield return new WaitForSeconds(sortInterval);

        if (sortRacers == true)
        {
            StartCoroutine(SortRacers());
        }
        else if(sortRacers == false)
        {
            StopCoroutine(SortRacers());
        }
    }

    private void QuickSort(RaceTracker[] array, int low, int high)
    {
        if (low < high)
        {
            int pi = DivideArray(array, low, high);
            QuickSort(array, low, pi - 1);
            QuickSort(array, pi + 1, high);
        }
    }

    private int DivideArray(RaceTracker[] array, int low, int high)
    {
        RaceTracker pivot = array[high];
        int i = (low - 1);
        for (int j = low; j <= high - 1; ++j)
        {
            if (CompareRaceTrackers(array[j], pivot) < 0)
            {
                i++;
                SwapElements(array, i, j);
            }
        }
        SwapElements(array, i + 1, high);
        return (i + 1);
    }

    private void SwapElements(RaceTracker[] array, int i, int j)
    {
        RaceTracker temp = array[i];
        array[i] = array[j];
        array[j] = temp;
    }

    private float CompareRaceTrackers(RaceTracker a, RaceTracker b)
    {
        // Comparamos por vueltas completadas
        if (a.LapsCompleted != b.LapsCompleted)
        {
            return b.LapsCompleted.CompareTo(a.LapsCompleted);
        }

        // Comparamos por checkpoint principal
        if (a.CurrentMain != b.CurrentMain)
        {
            return b.CurrentMain.CompareTo(a.CurrentMain);
        }

        // Comparamos por checkpoint auxiliar
        if (a.CurrentAuxiliary != b.CurrentAuxiliary)
        {
            return b.CurrentAuxiliary.CompareTo(a.CurrentAuxiliary);
        }

        // Si ambos están en el último checkpoint auxiliar, calculamos distancia al objetivo
        if (a.CurrentAuxiliary == a.CheckpointManager.auxiliaryCheckpoints.Length - 1 &&
            b.CurrentAuxiliary == b.CheckpointManager.auxiliaryCheckpoints.Length - 1)
        {
            float distanceToGoalA = Vector3.Distance(a.transform.parent.position, a.CheckpointManager.goalCheckpoint.transform.position);
            float distanceToGoalB = Vector3.Distance(b.transform.parent.position, b.CheckpointManager.goalCheckpoint.transform.position);
            return distanceToGoalA.CompareTo(distanceToGoalB);
        }

        // Distancia al siguiente checkpoint
        return a.GetDistanceToNextCheckpoint().CompareTo(b.GetDistanceToNextCheckpoint());
    }
}
