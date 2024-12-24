using NaughtyAttributes;
using UnityEngine;

[CreateAssetMenu(fileName = "AudioConfig", menuName = "ScriptableObjects/Audio/AudioConfig")]
public class AudioConfig : ScriptableObject
{
    [BoxGroup("Volume Settings")]
    [Range(0.001f, 1f), Tooltip("Controls the music volume (0.001f = mute, 1 = max volume)")]
    [SerializeField] private float musicVolume = 0.5f;

    [BoxGroup("Volume Settings")]
    [Range(0.001f, 1f), Tooltip("Controls the sound effects volume (0.001f = mute, 1 = max volume)")]
    [SerializeField] private float sfxVolume = 0.5f;

    [BoxGroup("Volume Settings")]
    [Range(0.001f, 1f), Tooltip("Controls the overall audio volume (0.001f = mute, 1 = max volume)")]
    [SerializeField] private float masterVolume = 0.5f;

    public float MusicVolume
    {
        get
        {
            return musicVolume;
        }
        set
        {
            musicVolume = value;
        }
    }
    public float SfxVolume
    {
        get
        {
            return sfxVolume;
        }
        set
        {
            sfxVolume = value;
        }
    }
    public float MasterVolume
    {
        get
        {
            return masterVolume;
        }
        set
        {
            masterVolume = value;
        }
    }
}
