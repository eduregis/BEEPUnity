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
        isPlaying = !isPlaying;
        iconImage.sprite = isPlaying ? stop : play;
    }
}
