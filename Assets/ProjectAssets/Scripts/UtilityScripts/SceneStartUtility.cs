using NaughtyAttributes;
using UnityEngine;

public class SceneStartUtility : MonoBehaviour
{
    [BoxGroup("Startup Music")]
    [SerializeField, Tooltip("Play music at start without transition")]
    [Label("Play Music On Start")]
    private bool playMusic = false;

    [BoxGroup("Startup Music"), ShowIf("playMusic")]
    [SerializeField, Tooltip("Index of the music clip to play at start")]
    [Label("Start Music Index")]
    private int musicIndex = 0;

    private void Start()
    {
        if (playMusic == true && AudioManager.Instance.MusicClips.Length > 0 && musicIndex >= 0 && musicIndex < AudioManager.Instance.MusicClips.Length)
        {
            AudioManager.Instance.PlayMusic(musicIndex);
        }
    }
}
