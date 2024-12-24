using NaughtyAttributes;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; }

    [BoxGroup("Audio Mixer Settings")]
    [SerializeField, Required] private AudioMixer audioMixer;

    [BoxGroup("Audio Sources")]
    [SerializeField, Required] private AudioSource musicAudioSource;

    [BoxGroup("Audio Sources")]
    [SerializeField, Required] private AudioSource sfxAudioSource;

    [BoxGroup("Audio Clips")]
    [SerializeField] private AudioClip[] musicClips;

    [BoxGroup("Audio Clips")]
    [SerializeField] private AudioClip[] sfxClips;

    [BoxGroup("Audio Configuration")]
    [Expandable]
    [SerializeField] private AudioConfig audioConfig;

    [BoxGroup("Fade Settings")]
    [Range(0.1f, 5.0f)]
    [SerializeField, Tooltip("Music fade time in seconds")]
    private float fadeDuration = 1.0f;

    private Stack<AudioSource> sfxSourceStack = new Stack<AudioSource>();

    [BoxGroup("Fade Settings")]
    [ReadOnly] 
    [SerializeField] private bool isFading = false;

    public AudioConfig AudioConfig
    {
        get
        {
            return audioConfig;
        }
    }

    public AudioClip[] MusicClips
    {
        get
        {
            return musicClips;
        }
    }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            Instance = this;
        }
    }

    // Volume configuration
    public void SetVolumeOfMusic(Slider musicConfiguration)
    {
        audioMixer.SetFloat("MusicVolume", Mathf.Log10(musicConfiguration.value) * 20f);
    }

    public void SetVolumeOfSfx(Slider sfxConfiguration)
    {
        audioMixer.SetFloat("SFXVolume", Mathf.Log10(sfxConfiguration.value) * 20f);
    }

    public void SetVolumeOfMaster(Slider masterConfiguration)
    {
        audioMixer.SetFloat("MasterVolume", Mathf.Log10(masterConfiguration.value) * 20f);
    }

    // Play music without transition
    public void PlayMusic(int index)
    {
        // Do not allow music change if there is a fade in progress
        if (isFading == false)
        {
            musicAudioSource.Stop();
            musicAudioSource.clip = musicClips[index];
            musicAudioSource.Play();
        }

    }

    // Play music with transition (fade out)
    public void PlayMusicWithTransition(int index)
    {
        if (isFading == false)
        {
            StartCoroutine(FadeOutMusicAndPlayNew(index));
        }
    }

    private IEnumerator FadeOutMusicAndPlayNew(int newMusicIndex)
    {
        isFading = true; // It is marking the start of the fade

        // Perform fade out
        float currentVolume;
        audioMixer.GetFloat("MusicVolume", out currentVolume);
        float startVolume = Mathf.Pow(10f, currentVolume / 20f); // Convert dB to linear value

        for (float t = 0; t < fadeDuration; t += Time.deltaTime)
        {
            float normalizedTime = t / fadeDuration;
            audioMixer.SetFloat("MusicVolume", Mathf.Log10(Mathf.Lerp(startVolume, 0f, normalizedTime)) * 20f);
            yield return null;
        }

        // Stop the previous music and play the new one
        musicAudioSource.Stop();
        musicAudioSource.clip = musicClips[newMusicIndex];
        musicAudioSource.Play();

        // Perform fade in of new music
        for (float t = 0; t < fadeDuration; t += Time.deltaTime)
        {
            float normalizedTime = t / fadeDuration;
            audioMixer.SetFloat("MusicVolume", Mathf.Log10(Mathf.Lerp(0f, startVolume, normalizedTime)) * 20f);
            yield return null;
        }

        isFading = false; // The fade has ended
    }

    // Playing SFX using the AudioSource stack
    public void PlaySfx(int index)
    {
        if (sfxAudioSource.isPlaying == true)
        {
            // If a sound is already playing, create a new AudioSource.
            AudioSource newSfxSource = CreateNewSfxSource();
            newSfxSource.PlayOneShot(sfxClips[index]);
        }
        else
        {
            // If not, use the main one
            sfxAudioSource.PlayOneShot(sfxClips[index]);
        }
    }

    private AudioSource CreateNewSfxSource()
    {
        AudioSource newSfxSource = new GameObject("SfxAudioSource").AddComponent<AudioSource>();
        newSfxSource.outputAudioMixerGroup = sfxAudioSource.outputAudioMixerGroup;
        newSfxSource.volume = sfxAudioSource.volume;
        newSfxSource.playOnAwake = false;
        sfxSourceStack.Push(newSfxSource);
        return newSfxSource;
    }

    // Clean inactive audio sources
    private void Update()
    {
        if (sfxSourceStack.Count > 0)
        {
            AudioSource topSource = sfxSourceStack.Peek();
            if (topSource.isPlaying == false)
            {
                Destroy(topSource.gameObject);
                sfxSourceStack.Pop();
            }
        }
    }
}