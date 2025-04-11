using UnityEngine;
using UnityEngine.UI;

public class PlayerButton : MonoBehaviour
{
    [Header("Images")]
    public Image iconImage; // Referência ao componente de imagem do ícone

    [Header("Sprites")]
    public Sprite play;
    public Sprite stop;

    public bool isPlaying = false;

    public void ToggleButton()
    {
        AppSettings.IsPlaying = !AppSettings.IsPlaying;
        isPlaying = !isPlaying;
        AudioManager.Instance.Play(isPlaying ? "playButton" : "stopButton");
        iconImage.sprite = isPlaying ? stop : play;
    }
}
