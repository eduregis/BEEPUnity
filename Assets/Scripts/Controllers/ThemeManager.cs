using System;
using UnityEngine;

public class ThemeManager : MonoBehaviour
{
    public static ThemeManager Instance { get; private set; }

    public bool IsDarkMode { get; private set; }

    public event Action OnThemeChanged;


    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        // Carrega o estado salvo
        IsDarkMode = AppSettings.DarkModeEnabled;
    }

    public void SetDarkMode(bool darkMode)
    {
        if (IsDarkMode == darkMode) return;

        IsDarkMode = darkMode;
        AppSettings.DarkModeEnabled = darkMode;

        OnThemeChanged?.Invoke();
    }

    public void ToggleTheme()
    {
        SetDarkMode(!IsDarkMode);
    }
}
