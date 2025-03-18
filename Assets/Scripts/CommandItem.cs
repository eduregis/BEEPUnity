using UnityEngine;
using UnityEngine.UI;

public class CommandItem : MonoBehaviour
{
    public Image iconImage; // Referência ao componente de imagem do ícone
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
}