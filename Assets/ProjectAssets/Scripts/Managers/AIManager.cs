using UnityEngine;
using System;

public class AIManager : MonoBehaviour
{
    [SerializeField] private GameObject[] aiPrefabs; // Prefabs de corredores IA
    [SerializeField] private int quantityAICars; // N�mero de corredores IA a generar
    [SerializeField] private GameObject[] aiCars; // Arreglo de corredores IA generados
    [SerializeField] private GameObject[] gridCells; // Posiciones de las celdas de la parrilla de salida
    [SerializeField] private CheckpointManager checkpointManager; // Checkpoint Manager

    public GameObject[] AiCars
    {
        get
        {
            return aiCars;
        }
        set
        {
            aiCars = value;
        }
    }

    public event Action OnAICarsInitialized;

    private void Start()
    {
        if (quantityAICars > gridCells.Length)
        {
            Debug.Log("La cantidad de corredores IA excede el n�mero de celdas disponibles.");
            return;
        }

        aiCars = new GameObject[quantityAICars];
        AssignUniquePositions();
        OnAICarsInitialized?.Invoke();
    }

    private void AssignUniquePositions()
    {
        // Creamos un arreglo de �ndices que representa las posiciones de las celdas
        int[] indices = new int[gridCells.Length];
        for (int i = 0; i < indices.Length; ++i)
        {
            indices[i] = i; // Inicializamos con �ndices secuenciales
        }

        // Barajamos los �ndices de manera l�gica
        ShuffleIndices(indices);

        // Usamos los primeros `quantityAICars` �ndices para asignar posiciones �nicas
        for (int i = 0; i < quantityAICars; ++i)
        {
            GameObject selectedCar = aiPrefabs[UnityEngine.Random.Range(0, aiPrefabs.Length)];
            int cellIndex = indices[i]; // Tomamos un �ndice �nico del arreglo barajado

            GameObject aiCar = Instantiate(selectedCar, gridCells[cellIndex].transform.position, selectedCar.transform.rotation);

            aiCars[i] = aiCar;
        }

        for (int i = 0; i < aiCars.Length; ++i)
        {
            Transform childTransform = aiCars[i].transform.GetChild(0);
            Racer script = childTransform.GetComponent<Racer>();
            script.CheckpointManager = checkpointManager;

            if (script == null)
            {
                Debug.Log("RaceTracker no encontrado en el primer hijo");
            }
        }
    }

    // Baraja un arreglo de enteros sin repetir valores.
    // Este m�todo organiza los �ndices aleatoriamente para asegurar posiciones �nicas.
    private void ShuffleIndices(int[] array)
    {
        // Ejemplo inicial:
        // Si array = [0, 1, 2] (3 celdas disponibles)
        for (int i = 0; i < array.Length; ++i) // Recorremos cada elemento del arreglo
        {
            // Elegimos un �ndice aleatorio entre el actual (`i`) y el final del arreglo
            int randomIndex = UnityEngine.Random.Range(i, array.Length);

            // Ejemplo: Supongamos que estamos en `i = 0` y el rango es [0, 2]
            // Si randomIndex = 2, intercambiamos array[0] con array[2]

            // Guardamos el valor actual en una variable temporal
            int temp = array[i];

            // Intercambiamos el valor actual con el valor en `randomIndex`
            array[i] = array[randomIndex];
            array[randomIndex] = temp;

            // Ejemplo visual despu�s del intercambio:
            // Iteraci�n 1 (i = 0, randomIndex = 2):
            // array pasa de [0, 1, 2] a [2, 1, 0]

            // Iteraci�n 2 (i = 1, randomIndex = 1):
            // array permanece [2, 1, 0] porque se intercambia consigo mismo

            // Iteraci�n 3 (i = 2, randomIndex = 2):
            // array permanece [2, 1, 0]
        }

        // Resultado: El arreglo `array` ahora est� barajado aleatoriamente
        // Ejemplo final: array = [2, 1, 0]
    }
}