using UnityEngine;

public class CommandGrid : MonoBehaviour
{
    [Header("Command Settings")]
    public GameObject commandPrefab; // Prefab do comando
    public int commandCount = 9; // Número de comandos
    public float spacing = 4f; // Espaçamento entre os comandos

    [Header("Command Size")]
    public float commandWidth = 30f; // Largura do comando
    public float commandHeight = 25f; // Altura do comando

    [Header("Command Icons")]
    public Sprite[] commandIcons; // Array de ícones para cada tipo de comando

    // Enum para os tipos de comandos
    public enum CommandType
    {
        Run,
        TurnRight,
        TurnLeft,
        Grab,
        Heal,
        Loop,
        Function1,
        Function2,
        Conditional
    }

    private void Start()
    {
        if (Application.isPlaying) // Gera comandos apenas durante a execução
        {
            GenerateCommands();
        }
    }

    private void OnDestroy()
    {
        // Limpa os comandos ao destruir o objeto (incluindo ao parar o jogo no Editor)
        ClearCommands();
    }

    // Gera os comandos programaticamente
    public void GenerateCommands()
    {
        if (commandPrefab == null)
        {
            Debug.LogError("Command Prefab não foi atribuído.");
            return;
        }

        if (commandIcons == null || commandIcons.Length != commandCount)
        {
            Debug.LogError("O array de ícones não está configurado corretamente.");
            return;
        }

        for (int i = 0; i < commandCount; i++)
        {
            // Instancia o comando
            GameObject command = Instantiate(commandPrefab, transform);

            // Define o nome do comando
            command.name = $"Command_{((CommandType)i).ToString()}";

            // Calcula a posição vertical
            float yPosition = i * (commandHeight + spacing);
            command.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, -yPosition);

            // Define o tamanho do comando
            command.GetComponent<RectTransform>().sizeDelta = new Vector2(commandWidth, commandHeight);

            // Define o ícone e o tipo do comando
            CommandItem commandItem = command.GetComponent<CommandItem>();
            if (commandItem != null)
            {
                commandItem.SetCommand((CommandType)i, commandIcons[i]);
            }
            else
            {
                Debug.LogError("O prefab do comando não contém o componente CommandItem.");
            }
        }
    }

    // Remove todos os comandos existentes
    public void ClearCommands()
    {
        // Destroi os comandos de trás para frente para evitar problemas de índice
        for (int i = transform.childCount - 1; i >= 0; i--)
        {
            DestroyImmediate(transform.GetChild(i).gameObject);
        }
    }
}