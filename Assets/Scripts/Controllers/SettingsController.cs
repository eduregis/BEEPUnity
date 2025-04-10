using UnityEngine;
using UnityEngine.UI;

public class SettingsController : MonoBehaviour
{
    [SerializeField] private Slider sfxSlider;
    [SerializeField] private Slider ostSlider;

    private void Start()
    {
        sfxSlider.value = AppSettings.SFXVolume;
        ostSlider.value = AppSettings.OSTVolume;

        sfxSlider.onValueChanged.AddListener(OnSFXSliderChanged);
        ostSlider.onValueChanged.AddListener(OnOSTSliderChanged);
    }

    private void OnSFXSliderChanged(float value)
    {
        AppSettings.SFXVolume = value;
        // Apenas salva — AudioManager usará esse valor ao dar Play()
    }

    private void OnOSTSliderChanged(float value)
    {
        AppSettings.OSTVolume = value;
        AudioManager.Instance?.ApplyOSTVolume(value);
    }
}
