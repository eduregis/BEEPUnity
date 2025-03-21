using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RobotController : MonoBehaviour
{
    // Instância única do RobotController
    public static RobotController Instance { get; private set; }

    // Delegate para notificar a conclusão de um passo
    public delegate void StepCompletedHandler(string step, InventoryGrid currentGrid);
    public event StepCompletedHandler OnStepCompleted;

    private Animator animator;
    public bool isMoving = false; // Verifica se o robô está se movendo ou virando
    private string currentDirection = "Right"; // Direção inicial do robô
    private Vector2Int currentPosition; // Posição atual do robô na matriz

    // Velocidade de execução dos comandos (quanto maior, mais rápido)
    public float commandSpeed = 1.0f;
    private float assetAjust = 38f;
    private bool shouldStopExecution = false; // Flag para controlar a interrupção

    // Deslocamentos isométricos para cada direção
    private Dictionary<string, Vector2> moveDirections = new Dictionary<string, Vector2>
    {
        { "Right", new Vector2(30, -15) },
        { "Down", new Vector2(-30, -15) },
        { "Left", new Vector2(-30, 15) },
        { "Up", new Vector2(30, 15) }
    };

    // Ordem das direções ao virar para a esquerda ou direita
    private List<string> directionOrder = new List<string> { "Right", "Down", "Left", "Up" };

    // Dimensões dos tiles (deve ser igual ao IsometricMapGenerator)
    public float tileWidth = 64f;
    public float tileHeight = 48f;

    private void Awake()
    {
        // Configura a instância única do RobotController
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            // Se já existir uma instância, destrói esta nova instância
            Debug.LogWarning("Já existe uma instância do RobotController. Destruindo esta nova instância.");
            Destroy(gameObject);
        }

        // Obtém o componente Animator no Awake para garantir que ele seja atribuído corretamente
        animator = GetComponent<Animator>();

        if (animator == null)
        {
            Debug.LogError("Animator não encontrado no objeto!");
        }
    }

    // Define a posição inicial do robô na matriz e o posiciona no mundo isométrico
    public void SetInitialPosition(Vector2Int position)
    {
        currentPosition = position;
        UpdatePositionInWorld();
    }

    // Atualiza a posição do robô no mundo isométrico com base na posição na matriz
    private void UpdatePositionInWorld()
    {
        // Obtém a posição do GameObject que contém o script
        Vector3 originPosition = IsometricMapGenerator.Instance.transform.position;

        // Calcula o deslocamento para centralizar o robô
        int mapWidth = IsometricMapGenerator.Instance.mapMatrix.GetLength(1);
        int mapHeight = IsometricMapGenerator.Instance.mapMatrix.GetLength(0);
        Vector3 offset = new Vector3(
            (mapWidth - mapHeight) * ((tileWidth / 2) - 2),
            -((mapWidth + mapHeight) / 2) * ((tileHeight / 2) - 9),
            0
        );

        // Calcula a posição isométrica relativa ao centro
        Vector3 tilePosition = new Vector3(
            (currentPosition.x - currentPosition.y) * ((tileWidth / 2) - 2),
            -(currentPosition.x + currentPosition.y) * ((tileHeight / 2) - 9) + assetAjust,
            0
        );

        // Ajusta a posição para considerar o centro do mapa
        tilePosition += originPosition - offset;

        // Define a posição do robô no mundo
        transform.position = tilePosition;
    }

    public void StopExecution()
    {
        shouldStopExecution = true; // Sinaliza para a corrotina parar
    }

    // Corrotina para executar os comandos sequencialmente
    public void ExecuteSingleCommand(string command, InventoryGrid currentGrid)
    {
        if (!isMoving)
        {
            StartCoroutine(RunSingleCommand(command, currentGrid));
        }
    }

   private IEnumerator RunSingleCommand(string command, InventoryGrid currentGrid)
    {
        isMoving = true;

        if (shouldStopExecution)
        {
            OnStepCompleted?.Invoke("Concluded", currentGrid);
        }
        else 
        {
            switch (command)
            {
                case "Run":
                    if (CanMove(currentDirection))
                    {
                        yield return MoveToDirection(currentDirection);
                        OnStepCompleted?.Invoke("Run", currentGrid); // Passa o grid atual
                    }
                    else
                    {
                        yield return new WaitForSeconds(1.0f / commandSpeed);
                        OnStepCompleted?.Invoke("Run blocked", currentGrid); // Passa o grid atual
                    }
                    break;

                case "TurnLeft":
                    yield return Turn("Left");
                    OnStepCompleted?.Invoke("TurnLeft", currentGrid); // Passa o grid atual
                    break;

                case "TurnRight":
                    yield return Turn("Right");
                    OnStepCompleted?.Invoke("TurnRight", currentGrid); // Passa o grid atual
                    break;

                default:
                    Debug.LogWarning("Comando inválido: " + command);
                    break;
            }
        isMoving = false;
        }
    }

    // Verifica se o movimento é possível
    private bool CanMove(string direction)
    {
        Vector2Int newPosition = currentPosition + DirectionToVector(direction);
        int[,] mapMatrix = IsometricMapGenerator.Instance.mapMatrix;

        // Verifica se a nova posição está dentro dos limites da matriz e é um tile válido (1)
        return newPosition.x >= 0 && newPosition.x < mapMatrix.GetLength(1) &&
               newPosition.y >= 0 && newPosition.y < mapMatrix.GetLength(0) &&
               mapMatrix[newPosition.y, newPosition.x] != 0;
    }

    // Converte a direção atual para um vetor de movimento na matriz
    private Vector2Int DirectionToVector(string direction)
    {
        switch (direction)
        {
            case "Right": return new Vector2Int(1, 0);
            case "Down": return new Vector2Int(0, 1);
            case "Left": return new Vector2Int(-1, 0);
            case "Up": return new Vector2Int(0, -1);
            default: return Vector2Int.zero;
        }
    }

    // Move o robô na direção especificada
    private IEnumerator MoveToDirection(string direction)
    {
        // Atualiza a animação antes de mover
        UpdateAnimator(direction);

        // Calcula a nova posição na matriz
        Vector2Int newPosition = currentPosition + DirectionToVector(direction);

        // Calcula a posição de destino no mundo isométrico
        Vector3 startPosition = transform.position;
        Vector3 targetPosition = CalculateWorldPosition(newPosition);

        // Move o robô suavemente até a posição de destino
        float elapsedTime = 0f;
        float duration = 1.0f / commandSpeed; // Ajusta a duração com base na velocidade

        while (elapsedTime < duration)
        {
            transform.position = Vector3.Lerp(startPosition, targetPosition, elapsedTime / duration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        // Garante que o robô chegue exatamente na posição de destino
        transform.position = targetPosition;

        // Atualiza a posição atual na matriz
        currentPosition = newPosition;
    }

    // Calcula a posição no mundo isométrico com base na posição na matriz
    private Vector3 CalculateWorldPosition(Vector2Int position)
    {
        // Obtém a posição do GameObject que contém o script
        Vector3 originPosition = IsometricMapGenerator.Instance.transform.position;

        // Calcula o deslocamento para centralizar o robô
        int mapWidth = IsometricMapGenerator.Instance.mapMatrix.GetLength(1);
        int mapHeight = IsometricMapGenerator.Instance.mapMatrix.GetLength(0);
        Vector3 offset = new Vector3(
            (mapWidth - mapHeight) * ((tileWidth / 2) - 2),
            -((mapWidth + mapHeight) / 2) * ((tileHeight / 2) - 9),
            0
        );

        // Calcula a posição isométrica relativa ao centro
        Vector3 tilePosition = new Vector3(
            (position.x - position.y) * ((tileWidth / 2) - 2),
            -(position.x + position.y) * ((tileHeight / 2) - 9) + assetAjust,
            0
        );

        // Ajusta a posição para considerar o centro do mapa
        tilePosition += originPosition - offset;

        return tilePosition;
    }

    // Vira o robô para a esquerda ou direita
    private IEnumerator Turn(string turnDirection)
    {
        // Calcula a nova direção
        int currentIndex = directionOrder.IndexOf(currentDirection);
        int newIndex;

        if (turnDirection == "Left")
        {
            newIndex = (currentIndex - 1 + directionOrder.Count) % directionOrder.Count;
        }
        else // "Right"
        {
            newIndex = (currentIndex + 1) % directionOrder.Count;
        }

        string newDirection = directionOrder[newIndex];

        // Atualiza a animação imediatamente
        UpdateAnimator(newDirection);

        // Força a atualização do Animator no primeiro frame
        animator.Update(0);

        // Aguarda o tempo de virada ajustado pela velocidade
        float elapsedTime = 0f;
        float duration = 1.0f / commandSpeed; // Ajusta a duração com base na velocidade

        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        // Atualiza a direção atual
        currentDirection = newDirection;
    }

    // Atualiza os parâmetros do Animator com base na direção
    private void UpdateAnimator(string direction)
    {
        if (animator == null)
        {
            Debug.LogError("Animator não foi atribuído!");
            return;
        }

        // Atualiza os parâmetros do Animator com base na direção
        switch (direction)
        {
            case "Right":
                animator.SetFloat("DirectionX", 1);
                animator.SetFloat("DirectionY", 0);
                break;
            case "Down":
                animator.SetFloat("DirectionX", 0);
                animator.SetFloat("DirectionY", -1);
                break;
            case "Left":
                animator.SetFloat("DirectionX", -1);
                animator.SetFloat("DirectionY", 0);
                break;
            case "Up":
                animator.SetFloat("DirectionX", 0);
                animator.SetFloat("DirectionY", 1);
                break;
        }

        // Indica que o robô está se movendo (opcional)
        animator.SetBool("IsMoving", true);
    }
}