using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Image))]
public class ThemedImage : MonoBehaviour
{
    [SerializeField] private Sprite lightSprite;
    [SerializeField] private Sprite darkSprite;

    private Image image;

    private void Awake()
    {
        image = GetComponent<Image>();
    }

    private void OnEnable()
    {
        ThemeManager.Instance.OnThemeChanged += UpdateTheme;
        UpdateTheme();
    }

    private void OnDisable()
    {
        if (ThemeManager.Instance != null)
        {
            ThemeManager.Instance.OnThemeChanged -= UpdateTheme;
        }
    }

    private void UpdateTheme()
    {
        image.sprite = ThemeManager.Instance.IsDarkMode ? darkSprite : lightSprite;
    }
}
