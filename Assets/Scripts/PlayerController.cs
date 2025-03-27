using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour
{
    [SerializeField]
    public InventoryGrid playerGrid, function1Grid, function2Grid, conditionalIfGrid, conditionalElseGrid, loopGrid;
    public LoopBox loopBox;
    public PlayerButton playerButton;

    void Start()
    {
        AppSettings.IsPlaying = false;
        SetupPhase();
    }

    private void SetupPhase()
    {
        // Define a matriz do mapa
        int[,] mapMatrix = new int[,]
        {
            {1, 3, 1, 1, 0},
            {1, 2, 0, 1, 1},
        };

        // Define a posição inicial do robô
        Vector2Int initialPosition = new Vector2Int(1, 1);

        // Define as posições iniciais das caixas
        List<Vector2Int> initialBoxes = new List<Vector2Int>
        {
            new Vector2Int(4, 1),
        };

        // Configura o mapa e a posição inicial do robô
        IsometricMapGenerator.Instance.SetMapMatrix(mapMatrix, initialBoxes);
        RobotController.Instance.SetInitialPosition(initialPosition);
        RobotController.Instance.OnStepCompleted += HandleStepCompleted;

        ResetUI();
    }

    private void ResetUI() 
    {
        playerGrid.ResetHighlights();
        function1Grid.ResetHighlights(); 
        function2Grid.ResetHighlights();
        conditionalIfGrid.ResetHighlights();
        conditionalElseGrid.ResetHighlights();
        loopGrid.ResetHighlights();
    }

    private IEnumerator ExecuteCommands()
    {
        // Obtém as sequências de comandos da "main" e da "função"
        List<string> playerCommands = playerGrid.GetCommandList();
        List<string> function1Commands = function1Grid.GetCommandList();
        List<string> function2Commands = function2Grid.GetCommandList();

        yield return new WaitForSeconds(1f); // Espera inicial

        // Executa os comandos da "main", passando o playerGrid como grid atual
        yield return ExecuteCommandList(playerCommands, function1Commands, function2Commands, playerGrid);
    }

    private IEnumerator ExecuteCommandList(List<string> commands, List<string> function1Commands, List<string> function2Commands, InventoryGrid currentGrid)
    {
        for (int i = 0; i < commands.Count; i++)
        {
            string command = commands[i];

            // Verifica se o comando é "Function1"
            if (command == "Function1")
            {
                Debug.Log("Executando Function1...");
                function1Grid.ResetHighlights();
                currentGrid.HighlightCurrentStep(); // Atualiza o highlight da "main"
                // Executa os comandos da function1, passando o function1Grid como grid atual
                yield return ExecuteCommandList(function1Commands, function1Commands, function2Commands, function1Grid);
                Debug.Log("Function1 concluída. Retomando main...");
            }
            // Verifica se o comando é "Function2"
            else if (command == "Function2")
            {
                Debug.Log("Executando Function2...");
                function2Grid.ResetHighlights();
                currentGrid.HighlightCurrentStep(); // Atualiza o highlight da "main"
                // Executa os comandos da function2, passando o function2Grid como grid atual
                yield return ExecuteCommandList(function2Commands, function1Commands, function2Commands, function2Grid);               
                Debug.Log("Function2 concluída. Retomando main...");
            }
            // Verifica se o comando é "Conditional"
            else if (command == "Conditional")
            {
                Debug.Log("Executando Conditional...");
                currentGrid.HighlightCurrentStep(); // Atualiza o highlight da "main"

                // Verifica a condição para decidir qual grid executar
                if (CheckCondition())
                {
                    Debug.Log("Condição verdadeira. Executando If...");
                    yield return ExecuteCommandList(conditionalIfGrid.GetCommandList(), function1Commands, function2Commands, conditionalIfGrid);
                    conditionalIfGrid.ResetHighlights();
                }
                else
                {
                    Debug.Log("Condição falsa. Executando Else...");
                    yield return ExecuteCommandList(conditionalElseGrid.GetCommandList(), function1Commands, function2Commands, conditionalElseGrid);
                    conditionalElseGrid.ResetHighlights();
                }

                Debug.Log("Conditional concluída. Retomando main...");
            }
            else if (command == "Loop")
            {
                Debug.Log("Executando Loop...");
                for (int loopIndex = 1; loopIndex <= loopBox.counter; loopIndex++) 
                {
                    loopGrid.ResetHighlights();
                    currentGrid.HighlightCurrentStep(); // Atualiza o highlight da "main"
                    // Executa os comandos da function2, passando o function2Grid como grid atual
                    yield return ExecuteCommandList(loopGrid.GetCommandList(), function1Commands, function2Commands, loopGrid);
                }
                Debug.Log("Loop concluído. Retomando main...");
            }
            else
            {
                // Executa outros comandos normalmente, passando o grid atual
                yield return ExecuteSingleCommand(command, currentGrid);
            }
        }
    }

    private IEnumerator ExecuteSingleCommand(string command, InventoryGrid currentGrid)
    {
        // Ativa o highlight no grid atual
        currentGrid.HighlightCurrentStep();
        
        // Executa um único comando no RobotController, passando o grid atual
        RobotController.Instance.ExecuteSingleCommand(command, currentGrid);

        // Espera até que o comando seja concluído
        while (RobotController.Instance.isMoving)
        {
            yield return null;
        }
    }

    public void OnPlayerPressed()
    {
        playerButton.ToggleButton();
        if (playerButton.isPlaying) 
        {
            StartCoroutine(ExecuteCommands());  
        }
        else 
        {
            SetupPhase();
        }
    }

    private void HandleStepCompleted(string step, InventoryGrid currentGrid)
    {
        // Destaca o passo atual no grid correto
        Debug.Log("Passo concluído: " + step);

        // Verifica se o objetivo foi concluído após cada passo
        if (CheckObjective())
        {
            Debug.Log("Objetivo concluído! Parando execução.");
            RobotController.Instance.StopExecution(); // Para a execução dos comandos
        }
    }

    private bool CheckObjective()
    {
        // Verifica se todas as caixas estão nos encaixes (valores 2)
    int[,] map = IsometricMapGenerator.Instance.mapMatrix;
    Box[,] boxes = IsometricMapGenerator.Instance.boxesMatrix;
    
    for (int y = 0; y < map.GetLength(0); y++)
    {
        for (int x = 0; x < map.GetLength(1); x++)
        {
            if (map[y, x] == (int)Constants.TileType.Fitting) // É um encaixe
            {
                if (boxes[y, x] == null) // Não tem caixa
                {
                    return false;
                }
            }
        }
    }
    return true;
    }

    private bool CheckCondition()
    {
        return true;
    }

    void OnDestroy()
    {
        RobotController.Instance.OnStepCompleted -= HandleStepCompleted;
    }
}