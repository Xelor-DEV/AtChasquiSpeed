using UnityEngine;

public class RaceRankingManager : MonoBehaviour
{
    [SerializeField] private GameObject[] allRacers; // All Racers
    [SerializeField] private GameObject[] players; // Players
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

}
