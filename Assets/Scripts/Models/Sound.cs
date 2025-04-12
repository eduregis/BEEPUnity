// AudioManager/Sound.cs
using UnityEngine;

[CreateAssetMenu(fileName = "Sound", menuName = "MyAudios/Sound")]
public class Sound : ScriptableObject
{
    public string soundName;
    public AudioClip clip;

    [Range(0f, 1f)]
    public float volume = 1f;
    [Range(.1f, 3f)]
    public float pitch = 1f;

    public bool loop = false;
    public Constants.SoundType soundType = Constants.SoundType.SFX;


    [HideInInspector]
    public AudioSource source;
}