using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SettingsController : MonoBehaviour
{
    [SerializeField] private Slider sfxSlider;
    [SerializeField] private Slider ostSlider;
    [SerializeField] private Toggle darkModeToggle;
    [SerializeField] private PlayerProgressSO playerProgress;

    private void Awake()
    {
        darkModeToggle.isOn = AppSettings.DarkModeEnabled;
    }
    private void Start()
    {
        sfxSlider.value = AppSettings.SFXVolume;
        ostSlider.value = AppSettings.OSTVolume;

        sfxSlider.onValueChanged.AddListener(OnSFXSliderChanged);
        ostSlider.onValueChanged.AddListener(OnOSTSliderChanged);
        darkModeToggle.onValueChanged.AddListener(OnToggleChanged);
    }

    private void OnSFXSliderChanged(float value)
    {
        AppSettings.SFXVolume = value;
    }

    private void OnOSTSliderChanged(float value)
    {
        AppSettings.OSTVolume = value;
        AudioManager.Instance?.ApplyOSTVolume(value);
    }

    private void OnDestroy()
    {
        sfxSlider.onValueChanged.RemoveListener(OnSFXSliderChanged);
        ostSlider.onValueChanged.RemoveListener(OnOSTSliderChanged);
        darkModeToggle.onValueChanged.RemoveListener(OnToggleChanged);
    }

    private void OnToggleChanged(bool isOn)
    {
        AudioManager.Instance.Play("defaultButton");
        ThemeManager.Instance.ToggleTheme();
    }

    public void ResetAll()
    {
        AppSettings.ResetAllPreferences();
        playerProgress.ResetProgress();
        SceneManager.LoadScene("HomeScene");
    }

    public void DismissButton()
    {
        CanvasFadeController.Instance.HideCanvas();
    }
}
