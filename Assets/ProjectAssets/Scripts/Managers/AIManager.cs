using UnityEngine;

public class AIManager : MonoBehaviour
{
    public GameObject[] aiPrefabs; // Prefabs de corredores IA
    public int quantityAICars; // Número de corredores IA a generar
    private GameObject[] aiCars; // Arreglo de corredores IA generados
    private GameObject[] gridCells; // Posiciones de las celdas de la parrilla de salida

    private void Start()
    {
        aiCars = new GameObject[quantityAICars];
        for (int i = 0; i < aiCars.Length; ++i)
        {
            GameObject selectedCar = aiPrefabs[Random.Range(0, aiPrefabs.Length)];
            GameObject aiCar = Instantiate(selectedCar, gridCells[Random.Range(0, gridCells.Length)].transform.position , selectedCar.transform.rotation);
            aiCars[i] = aiCar;
        }
    }
}