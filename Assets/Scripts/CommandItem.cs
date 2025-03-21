using UnityEngine;
using UnityEngine.UI;

public class CommandItem : MonoBehaviour
{
    [Header("Images")]
    public Image iconImage; // Referência ao componente de imagem do ícone
    public Image bkgImage; // Referência ao componente de background do ícone

    [Header("Sprites")]
    public Sprite bkgHighlighted;
    public Sprite bkgNormal;
    public CommandGrid.CommandType commandType; // Tipo do comando

    // Define o tipo e o ícone do comando
    public void SetCommand(CommandGrid.CommandType type, Sprite icon)
    {
        commandType = type;
        if (iconImage != null)
        {
            iconImage.sprite = icon;
        }
    }

    public void HighlightItem(bool active) 
    {
        bkgImage.sprite = active ? bkgHighlighted : bkgNormal;
    } 
}