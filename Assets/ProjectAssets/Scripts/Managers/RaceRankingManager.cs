using System.Collections;
using UnityEngine;

public class RaceRankingManager : MonoBehaviour
{
    [SerializeField] private GameObject[] racers; // All Racers
    [SerializeField] private GameObject[] players; // Players
    [SerializeField] private AIManager aiManager; // AI Manager
    [SerializeField] private float sortInterval = 0.2f;
    private bool isSortingActive;

    private Racer[] allRacers;
    public GameObject[] Racers
    {
        get
        {
            return racers;
        }
        set
        {
            racers = value;
        }
    }

    private void OnEnable()
    {
        aiManager.OnAICarsInitialized += InitializeAllRacers;
    }

    private void OnDisable()
    {
        aiManager.OnAICarsInitialized -= InitializeAllRacers;
    }

    private void InitializeAllRacers()
    {
        int totalRacers = players.Length + aiManager.AiCars.Length;
        racers = new GameObject[totalRacers];

        for (int i = 0; i < totalRacers; ++i)
        {
            if (i < players.Length)
            {
                racers[i] = players[i];
            }
            else
            {
                racers[i] = aiManager.AiCars[i - players.Length];
            }
        }

        // Inicializamos el arreglo de RaceTrackers una sola vez
        allRacers = new Racer[allRacers.Length];
        for (int i = 0; i < allRacers.Length; ++i)
        {
            // Usamos GetChild(0) para obtener el primer hijo y luego buscamos el RaceTracker
            Transform childTransform = allRacers[i].transform.GetChild(0);
            allRacers[i] = childTransform.GetComponent<Racer>();

            if (allRacers[i] == null)
            {
                Debug.Log($"RaceTracker no encontrado en el primer hijo de {allRacers[i].name}");
            }
        }
        ActivateRankingSystem();
    }

    public void ActivateRankingSystem()
    {
        StartCoroutine(SortRacers());
    }
    public void StopRankingSystem()
    {
        isSortingActive = false;
    }
    private IEnumerator SortRacers()
    {
        isSortingActive = true;

        while (isSortingActive == true)
        {
            QuickSort(allRacers, 0, allRacers.Length - 1);
            UpdateRacePositions();
            yield return new WaitForSeconds(sortInterval);
        }
    }

    private void QuickSort(Racer[] racers, int low, int high)
    {
        if (low < high)
        {
            int partitionIndex = Partition(racers, low, high);
            QuickSort(racers, low, partitionIndex - 1);
            QuickSort(racers, partitionIndex + 1, high);
        }
    }

    private int Partition(Racer[] racers, int low, int high)
    {
        Racer pivot = racers[high];
        int i = low - 1;

        for (int j = low; j < high; j++)
        {
            if (CompareRacers(racers[j], pivot)) // racerJ es mejor que pivot
            {
                i++;
                Swap(racers, i, j);
            }
        }
        Swap(racers, i + 1, high);
        return i + 1;
    }

    private bool CompareRacers(Racer a, Racer b)
    {
        // Comparación de vueltas completadas
        if (a.LapsCompleted != b.LapsCompleted)
            return a.LapsCompleted > b.LapsCompleted;

        // Comparación de checkpoints principales
        if (a.CurrentMainIndex != b.CurrentMainIndex)
            return a.CurrentMainIndex > b.CurrentMainIndex;

        // Comparación de checkpoints auxiliares
        if (a.CurrentAuxiliaryIndex != b.CurrentAuxiliaryIndex)
            return a.CurrentAuxiliaryIndex > b.CurrentAuxiliaryIndex;

        // Comparación de distancia al siguiente checkpoint
        float distanceA = GetDistanceToTarget(a);
        float distanceB = GetDistanceToTarget(b);
        return distanceA < distanceB;
    }

    private float GetDistanceToTarget(Racer racer)
    {
        Checkpoint target = racer.NextCheckpoint;
        if (target == null) return Mathf.Infinity;

        return Vector3.Distance(
            racer.transform.position,
            target.transform.position
        );
    }

    private void Swap(Racer[] racers, int i, int j)
    {
        Racer temp = racers[i];
        racers[i] = racers[j];
        racers[j] = temp;
    }

    private void UpdateRacePositions()
    {
        for (int i = 0; i < allRacers.Length; ++i)
        {
            allRacers[i].RaceRank = i + 1;
        }
    }
}
