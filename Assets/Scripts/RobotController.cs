using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RobotController : MonoBehaviour
{
    // Instância única do RobotController
    public static RobotController Instance { get; private set; }

    private Animator animator;
    private bool isMoving = false; // Verifica se o robô está se movendo ou virando
    private string currentDirection = "Right"; // Direção inicial do robô

    // Velocidade de execução dos comandos (quanto maior, mais rápido)
    public float commandSpeed = 1.0f;

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
        // Inicializa a animação com a direção atual
        UpdateAnimator(currentDirection);
    }

    // Função para iniciar a sequência de comandos
    public void ExecuteCommands(List<string> commands)
    {
        if (!isMoving)
        {
            StartCoroutine(RunCommands(commands));
        }
    }

    // Corrotina para executar os comandos sequencialmente
    private IEnumerator RunCommands(List<string> commands)
    {
        isMoving = true;

        foreach (string command in commands)
        {
            switch (command)
            {
                case "Move":
                    // Atualiza a animação antes de mover
                    UpdateAnimator(currentDirection);
                    // Move o robô na direção atual
                    yield return MoveToDirection(currentDirection);
                    break;

                case "TurnLeft":
                    // Vira 90 graus para a esquerda
                    yield return Turn("Left");
                    break;

                case "TurnRight":
                    // Vira 90 graus para a direita
                    yield return Turn("Right");
                    break;

                default:
                    Debug.LogWarning("Comando inválido: " + command);
                    break;
            }
        }

        // Fallback: Notifica que a locomoção terminou
        Debug.Log("Locomoção concluída.");
        isMoving = false;
    }

    // Move o robô na direção especificada
    private IEnumerator MoveToDirection(string direction)
    {
        // Calcula a posição de destino
        Vector2 startPosition = transform.position;
        Vector2 targetPosition = startPosition + moveDirections[direction];

        // Move o robô suavemente até a posição de destino
        float elapsedTime = 0f;
        float duration = 1.0f / commandSpeed; // Ajusta a duração com base na velocidade

        while (elapsedTime < duration)
        {
            transform.position = Vector2.Lerp(startPosition, targetPosition, elapsedTime / duration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        // Garante que o robô chegue exatamente na posição de destino
        transform.position = targetPosition;
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