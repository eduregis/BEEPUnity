using UnityEngine;

public class AppSettings : MonoBehaviour
{
    public static AppSettings Instance { get; private set; }
    
    public static bool IsPlaying
    {
        get => Instance != null && Instance._isPlaying;
        set 
        {
            if (Instance != null)
            {
                Instance._isPlaying = value;
                PlayerPrefs.SetInt("IsPlaying", value ? 1 : 0);
            }
        }
    }

    private bool _isPlaying
    {
        get => PlayerPrefs.GetInt("IsPlaying", 0) == 1;
        set => PlayerPrefs.SetInt("IsPlaying", value ? 1 : 0);
    }
    
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
        }
    }
}