// AudioManager/AudioManager.cs
using UnityEngine;
using System.Collections.Generic;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; }

    [SerializeField] private Sound[] globalSounds;
    private Dictionary<string, Sound> soundDictionary;

    private void Awake()
    {

        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            InitializeAudioSources();
        }
        else
        {
            Debug.Log("AudioManager duplicado e destruído");
            Destroy(gameObject);
        }
    }

    private void InitializeAudioSources()
    {
        soundDictionary = new Dictionary<string, Sound>();

        foreach (Sound sound in globalSounds)
        {
            // Evita duplicação acidental de sons
            if (!soundDictionary.ContainsKey(sound.soundName))
            {
                sound.source = gameObject.AddComponent<AudioSource>();
                ConfigureAudioSource(sound);
                soundDictionary.Add(sound.soundName, sound);
            }
            else
            {
                Debug.LogWarning($"Sound '{sound.soundName}' is duplicated in global sounds!");
            }
        }
    }

    private void ConfigureAudioSource(Sound sound)
    {
        sound.source.clip = sound.clip;
        sound.source.volume = sound.volume;
        sound.source.pitch = sound.pitch;
        sound.source.loop = sound.loop;
        sound.source.playOnAwake = false;
    }

    public void Play(string soundName)
    {
        if (soundDictionary.TryGetValue(soundName, out Sound sound))
        {
            float volume = sound.soundType == Constants.SoundType.OST
                ? AppSettings.OSTVolume
                : AppSettings.SFXVolume;

            sound.source.volume = volume;
            sound.source.Play();
        }
        else
        {
            Debug.LogWarning($"Global sound '{soundName}' not found!");
        }
    }

    public void Stop(string soundName)
    {
        if (soundDictionary.TryGetValue(soundName, out Sound sound))
        {
            sound.source.Stop();
        }
        else
        {
            Debug.LogWarning($"Sound: {soundName} not found!");
        }
    }

    // Métodos adicionais que você pode precisar
    public bool IsPlaying(string soundName)
    {
        if (soundDictionary.TryGetValue(soundName, out Sound sound))
        {
            return sound.source.isPlaying;
        }

        Debug.LogWarning($"Sound: {soundName} not found!");
        return false;
    }

    public void SetVolume(string soundName, float volume)
    {
        if (soundDictionary.TryGetValue(soundName, out Sound sound))
        {
            sound.volume = volume;

            // Se for OST e estiver tocando, aplica em tempo real
            if (sound.soundType == Constants.SoundType.OST && sound.source.isPlaying)
            {
                sound.source.volume = volume;
            }
        }
        else
        {
            Debug.LogWarning($"Sound: {soundName} not found!");
        }
    }

    public void ApplyOSTVolume(float volume)
    {
        foreach (var pair in soundDictionary)
        {
            Sound sound = pair.Value;
            if (sound.soundType == Constants.SoundType.OST)
            {
                sound.volume = volume;
                if (sound.source.isPlaying)
                {
                    sound.source.volume = volume;
                }
            }
        }
    }
}