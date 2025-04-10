using UnityEngine;

public class AppSettings : MonoBehaviour
{
    private static AppSettings _instance;
    public static AppSettings Instance => _instance ??= FindAnyObjectByType<AppSettings>() ?? new GameObject("AppSettings").AddComponent<AppSettings>();

    public static bool IsPlaying
    {
        get => PlayerPrefs.GetInt(nameof(IsPlaying)) == 1;
        set => PlayerPrefs.SetInt(nameof(IsPlaying), value ? 1 : 0);
    }

    public static bool DarkModeEnabled
    {
        get => PlayerPrefs.GetInt(nameof(DarkModeEnabled)) == 1;
        set => PlayerPrefs.SetInt(nameof(DarkModeEnabled), value ? 1 : 0);
    }

    public static float SFXVolume
    {
        get => PlayerPrefs.GetFloat(nameof(SFXVolume), 1f);
        set => PlayerPrefs.SetFloat(nameof(SFXVolume), value);
    }

    public static float OSTVolume
    {
        get => PlayerPrefs.GetFloat(nameof(OSTVolume), 1f);
        set => PlayerPrefs.SetFloat(nameof(OSTVolume), value);
    }


    public static string DialogueName
    {
        get => PlayerPrefs.GetString(nameof(DialogueName));
        set => PlayerPrefs.SetString(nameof(DialogueName), value);
    }

    public static int HighestUnlockedLevel
    {
        get => PlayerPrefs.GetInt(nameof(HighestUnlockedLevel));
        set => PlayerPrefs.SetInt(nameof(HighestUnlockedLevel), value);
    }

    public static int CurrentLevel
    {
        get => PlayerPrefs.GetInt(nameof(CurrentLevel));
        set => PlayerPrefs.SetInt(nameof(CurrentLevel), value);
    }

    private void Awake()
    {
        if (_instance != null && _instance != this) Destroy(gameObject);
        else { _instance = this; DontDestroyOnLoad(gameObject); }
    }
}