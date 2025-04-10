using System;
using UnityEngine;

public class ThemeManager : MonoBehaviour
{
    public static ThemeManager Instance { get; private set; }

    public bool IsDarkMode { get; private set; } = false;

    public event Action OnThemeChanged;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject); // Opcional: se quiser manter entre cenas
    }

    public void SetDarkMode(bool darkMode)
    {
        if (IsDarkMode == darkMode) return;

        IsDarkMode = darkMode;
        OnThemeChanged?.Invoke();
    }

    public void ToggleTheme()
    {
        SetDarkMode(!IsDarkMode);
    }
}
