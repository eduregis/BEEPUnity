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
    public bool isHoldingBox = false; // Verifica se o robô está segurando uma caixa
    private string currentDirection = "Right"; // Direção inicial do robô
    private Vector2Int currentPosition; // Posição atual do robô na matriz

    // Velocidade de execução dos comandos (quanto maior, mais rápido)
    public float commandSpeed = 1.0f;
    private float assetAdjust = 48f;
    private bool shouldStopExecution = false; // Flag para controlar a interrupção

    // Ordem das direções ao virar para a esquerda ou direita
    private List<string> directionOrder = new() { "Right", "Down", "Left", "Up" };

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

    void Start() 
    {
        Debug.Log("Valores iniciais do Animator:");
        Debug.Log($"DirX: {animator.GetFloat("DirectionX")}");
        Debug.Log($"DirY: {animator.GetFloat("DirectionY")}");
        
        // Força direção inicial para Right
        UpdateAnimator("Right");
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
        Vector3 offset = Utils.CalculateOffset();

        // Calcula a posição isométrica relativa ao centro
        Vector3 tilePosition = Utils.CalculateTilePosition(currentPosition.x, currentPosition.y, assetAdjust);

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
                    }
                    else
                    {
                        yield return new WaitForSeconds(1.0f / commandSpeed);
                        OnStepCompleted?.Invoke("Run blocked", currentGrid); // Passa o grid atual
                    }
                    break;

                case "TurnLeft":
                    yield return Turn("Left");
                    break;

                case "TurnRight":
                    yield return Turn("Right");
                    break;

                case "Grab":
                    yield return Grab();
                    break;

                default:
                    Debug.LogWarning("Comando inválido: " + command);
                    break;
            }
            OnStepCompleted?.Invoke(command, currentGrid);
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
               mapMatrix[newPosition.y, newPosition.x] != 0 && 
               !IsometricMapGenerator.Instance.HasBoxAt(newPosition);
    }

    // Converte a direção atual para um vetor de movimento na matriz
    private Vector2Int DirectionToVector(string direction)
    {
        return direction switch
        {
            "Right" => new Vector2Int(1, 0),
            "Down" => new Vector2Int(0, 1),
            "Left" => new Vector2Int(-1, 0),
            "Up" => new Vector2Int(0, -1),
            _ => Vector2Int.zero,
        };
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
        Vector3 offset = Utils.CalculateOffset();

        // Calcula a posição isométrica relativa ao centro
        Vector3 tilePosition = Utils.CalculateTilePosition(position.x, position.y, assetAdjust);

        // Ajusta a posição para considerar o centro do mapa
        tilePosition += originPosition - offset;

        return tilePosition;
    }

    private IEnumerator Turn(string turnDirection)
    {
        int currentIndex = directionOrder.IndexOf(currentDirection);
        int newIndex = turnDirection == "Left" 
            ? (currentIndex - 1 + directionOrder.Count) % directionOrder.Count
            : (currentIndex + 1) % directionOrder.Count;

        string newDirection = directionOrder[newIndex];
        currentDirection = newDirection; // Atualiza primeiro a direção
        
        UpdateAnimator(newDirection);
        
        yield return new WaitForSeconds(1.0f / commandSpeed);
    }

    private IEnumerator Grab()
{
    Vector2Int frontPosition = currentPosition + DirectionToVector(currentDirection);
    
    if (isHoldingBox)
    {
        // SOLTAR CAIXA (Drop)
        if (CanDropBox(frontPosition))
        {
            // Cria uma nova caixa no destino
            IsometricMapGenerator.Instance.CreateBoxAtPosition(frontPosition);
            isHoldingBox = false;
            UpdateAnimator(currentDirection);
        }
    }
    else
    {
        // PEGAR CAIXA (Grab)
        if (IsometricMapGenerator.Instance.HasBoxAt(frontPosition))
        {
            // Remove e destrói a caixa do mapa
            IsometricMapGenerator.Instance.RemoveBox(frontPosition);
            isHoldingBox = true;
            UpdateAnimator(currentDirection);
        }
    }
    
    yield return new WaitForSeconds(1.0f / commandSpeed);
}

    private bool CanDropBox(Vector2Int position)
    {
        // Verifica se a posição está dentro dos limites e é um tile válido
        if (!IsometricMapGenerator.Instance.IsValidPosition(position))
            return false;
        
        // Verifica se já tem uma caixa no destino
        if (IsometricMapGenerator.Instance.HasBoxAt(position))
            return false;
        
        return true;
    }

    private bool HasBoxInFront()
    {
        Vector2Int frontPosition = currentPosition + DirectionToVector(currentDirection);
        int[,] mapMatrix = IsometricMapGenerator.Instance.mapMatrix;
        
        // Verifica se a posição à frente está dentro dos limites do mapa
        if (frontPosition.x >= 0 && frontPosition.x < mapMatrix.GetLength(1) &&
            frontPosition.y >= 0 && frontPosition.y < mapMatrix.GetLength(0))
        {
            // Verifica se há uma caixa na posição à frente (valor 2 no mapa)
            return mapMatrix[frontPosition.y, frontPosition.x] == 2;
        }
        
        return false;
    }

    // Atualiza os parâmetros do Animator com base na direção
    private void UpdateAnimator(string direction) {
        Vector2 dir = direction switch {
            "Right" => new Vector2(1, 0),
            "Left"  => new Vector2(-1, 0),
            "Up"    => new Vector2(0, 1),
            "Down"  => new Vector2(0, -1),
            _ => Vector2.zero
        };

        animator.SetFloat("DirectionX", dir.x);
        animator.SetFloat("DirectionY", dir.y);
        animator.SetBool("IsHoldingBox", isHoldingBox);
        
        Debug.Log(dir);
        // Força atualização imediata (opcional, mas recomendado para viradas)
        animator.Update(0);
    }
}