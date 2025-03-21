using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour
{
    [SerializeField]
    public InventoryGrid playerGrid, function1Grid, function2Grid, conditionalIfGrid, conditionalElseGrid;
    private bool isObjectiveCompleted = false; // Flag para verificar se o objetivo foi concluído

    void Start()
    {
        // Define a matriz do mapa
        int[,] mapMatrix = new int[,]
        {
            {1, 1, 1, 1, 0},
            {1, 1, 0, 1, 1},
        };

        // Define a posição inicial do robô
        Vector2Int initialPosition = new Vector2Int(0, 0);

        // Configura o mapa e a posição inicial do robô
        IsometricMapGenerator.Instance.SetMapMatrix(mapMatrix);
        RobotController.Instance.SetInitialPosition(initialPosition);
        RobotController.Instance.OnStepCompleted += HandleStepCompleted;
    }

    private IEnumerator ExecuteCommands()
    {
        // Obtém as sequências de comandos da "main" e da "função"
        List<string> playerCommands = playerGrid.GetCommandList();
        List<string> function1Commands = function1Grid.GetCommandList();

        yield return new WaitForSeconds(1f); // Espera inicial

        // Executa os comandos da "main", passando o playerGrid como grid atual
        yield return ExecuteCommandList(playerCommands, function1Commands, playerGrid);
    }

    private IEnumerator ExecuteCommandList(List<string> commands, List<string> functionCommands, InventoryGrid currentGrid)
    {
        for (int i = 0; i < commands.Count; i++)
        {
            string command = commands[i];

            // Verifica se o comando é "Function1"
            if (command == "Function1")
            {
                Debug.Log("Executando Function1...");

                // Executa os comandos da função, passando o function1Grid como grid atual
                yield return ExecuteCommandList(functionCommands, null, function1Grid);

                Debug.Log("Function1 concluída. Retomando main...");
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
        StartCoroutine(ExecuteCommands());
    }

    private void HandleStepCompleted(string step, InventoryGrid currentGrid)
    {
        // Destaca o passo atual no grid correto
        currentGrid.HighlightCurrentStep();
        Debug.Log("Passo concluído: " + step);

        // Verifica se o objetivo foi concluído após cada passo
        if (CheckObjective())
        {
            Debug.Log("Objetivo concluído! Parando execução.");
            isObjectiveCompleted = true;
            RobotController.Instance.StopExecution(); // Para a execução dos comandos
        }
    }

    private bool CheckObjective()
    {
        return false;
    }

    void OnDestroy()
    {
        RobotController.Instance.OnStepCompleted -= HandleStepCompleted;
    }
}