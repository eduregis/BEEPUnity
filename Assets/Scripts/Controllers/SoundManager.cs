// AudioManager/AudioManager.cs
using UnityEngine;
using System.Collections.Generic;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; }
    
    [SerializeField] private Sound[] sounds;
    private Dictionary<string, Sound> soundDictionary;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        soundDictionary = new Dictionary<string, Sound>();
        
        foreach (Sound sound in sounds)
        {
            sound.source = gameObject.AddComponent<AudioSource>();
            sound.source.clip = sound.clip;
            // Configurações para reduzir latência:
            sound.source.playOnAwake = false;
            sound.source.ignoreListenerPause = true; // Opcional
            sound.source.ignoreListenerVolume = true; // Opcional

            sound.source.volume = sound.volume;
            sound.source.pitch = sound.pitch;
            sound.source.loop = sound.loop;
            
            soundDictionary.Add(sound.soundName, sound);
        }
    }
    
    public void Play(string soundName)
    {
        if (soundDictionary.TryGetValue(soundName, out Sound sound))
        {
            sound.source.Play();
        }
        else
        {
            Debug.LogWarning($"Sound: {soundName} not found!");
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
            sound.source.volume = volume;
        }
        else
        {
            Debug.LogWarning($"Sound: {soundName} not found!");
        }
    }
}